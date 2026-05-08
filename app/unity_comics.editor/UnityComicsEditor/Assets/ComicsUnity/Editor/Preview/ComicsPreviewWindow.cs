using UnityEngine;
using UnityEditor;
using NativeMind.ComicsViewer;
using NativeMind.ComicsViewer.IO;

namespace ComicsUnity.Preview
{
    /// <summary>
    /// "Preview as Player" window - full runtime preview with audio
    /// </summary>
    public class ComicsPreviewWindow : EditorWindow
    {
        private ComicsViewer _viewer;
        private GameObject _viewerObject;
        private RenderTexture _renderTexture;
        private Camera _previewCamera;

        private float _scrollPosition;
        private Vector2Int _viewportSize = new Vector2Int(1080, 1920);
        private bool _isInitialized;

        // Viewport presets
        private static readonly (string name, int w, int h)[] Presets = new[]
        {
            ("Phone 9:16", 1080, 1920),
            ("Phone 16:9", 1920, 1080),
            ("Tablet 3:4", 1536, 2048),
            ("Tablet 4:3", 2048, 1536),
        };

        [MenuItem("Comics/Preview as Player %#p")]
        public static void ShowWindow()
        {
            var window = GetWindow<ComicsPreviewWindow>("Preview as Player");
            window.minSize = new Vector2(400, 600);
            window.Initialize();
        }

        private void Initialize()
        {
            if (_isInitialized) return;

            var folderPath = FileManagerUnity.TempFolder;
            if (!System.IO.Directory.Exists(folderPath) ||
                !System.IO.File.Exists(System.IO.Path.Combine(folderPath, "data.json")))
            {
                Debug.LogWarning("No document loaded. Open a document first.");
                return;
            }

            // Create hidden viewer GameObject
            _viewerObject = new GameObject("PreviewViewer");
            _viewerObject.hideFlags = HideFlags.HideAndDontSave;

            // Add ComicsViewer component
            _viewer = _viewerObject.AddComponent<ComicsViewer>();

            // Setup camera
            _previewCamera = _viewerObject.AddComponent<Camera>();
            _previewCamera.orthographic = true;
            _previewCamera.clearFlags = CameraClearFlags.SolidColor;
            _previewCamera.backgroundColor = Color.black;
            _previewCamera.enabled = false; // Manual rendering

            // Create render texture
            CreateRenderTexture();

            // Load document from folder
            _viewer.LoadFolder(folderPath);

            // Disable automatic sound (we'll handle it manually if needed)
            _viewer.SetSoundEnabled(false);

            _isInitialized = true;
        }

        private void CreateRenderTexture()
        {
            if (_renderTexture != null)
            {
                DestroyImmediate(_renderTexture);
            }

            _renderTexture = new RenderTexture(_viewportSize.x, _viewportSize.y, 24);
            _renderTexture.Create();

            if (_previewCamera != null)
            {
                _previewCamera.targetTexture = _renderTexture;
            }
        }

        private void SetViewportSize(int width, int height)
        {
            _viewportSize = new Vector2Int(width, height);
            CreateRenderTexture();
            Repaint();
        }

        private void OnGUI()
        {
            // Toolbar
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                // Scroll slider
                EditorGUILayout.LabelField("Scroll:", GUILayout.Width(40));
                float maxScroll = _viewer != null ? _viewer.MaxScroll : 10000f;
                _scrollPosition = EditorGUILayout.Slider(_scrollPosition, 0, maxScroll, GUILayout.Width(200));

                GUILayout.FlexibleSpace();

                // Viewport presets
                foreach (var preset in Presets)
                {
                    if (GUILayout.Button(preset.name, EditorStyles.toolbarButton, GUILayout.Width(80)))
                    {
                        SetViewportSize(preset.w, preset.h);
                    }
                }

                // Refresh button
                if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(60)))
                {
                    RefreshFromSource();
                }
            }
            EditorGUILayout.EndHorizontal();

            // Info bar
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField($"Viewport: {_viewportSize.x}x{_viewportSize.y}", EditorStyles.miniLabel);
                EditorGUILayout.LabelField($"Scroll: {_scrollPosition:F0} / {(_viewer?.MaxScroll ?? 0):F0}", EditorStyles.miniLabel);
            }
            EditorGUILayout.EndHorizontal();

            // Viewport area
            var viewportRect = GUILayoutUtility.GetRect(
                _viewportSize.x,
                _viewportSize.y,
                GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true)
            );

            // Update viewer if initialized
            if (_isInitialized && _viewer != null)
            {
                _viewer.SetScrollOffset(_scrollPosition);

                // Manual camera render
                if (_previewCamera != null && _renderTexture != null)
                {
                    _previewCamera.Render();
                }
            }

            // Draw render texture
            if (_renderTexture != null)
            {
                // Calculate centered rect maintaining aspect ratio
                float aspect = (float)_viewportSize.x / _viewportSize.y;
                float rectAspect = viewportRect.width / viewportRect.height;

                Rect drawRect;
                if (rectAspect > aspect)
                {
                    // Window is wider - fit to height
                    float w = viewportRect.height * aspect;
                    float x = viewportRect.x + (viewportRect.width - w) / 2f;
                    drawRect = new Rect(x, viewportRect.y, w, viewportRect.height);
                }
                else
                {
                    // Window is taller - fit to width
                    float h = viewportRect.width / aspect;
                    float y = viewportRect.y + (viewportRect.height - h) / 2f;
                    drawRect = new Rect(viewportRect.x, y, viewportRect.width, h);
                }

                GUI.DrawTexture(drawRect, _renderTexture, ScaleMode.StretchToFill);

                // Handle scroll wheel in viewport
                if (Event.current.type == EventType.ScrollWheel && drawRect.Contains(Event.current.mousePosition))
                {
                    _scrollPosition = Mathf.Clamp(
                        _scrollPosition + Event.current.delta.y * 50f,
                        0,
                        _viewer?.MaxScroll ?? 10000f
                    );
                    Event.current.Use();
                    Repaint();
                }
            }
            else
            {
                EditorGUI.HelpBox(viewportRect, "No document loaded. Open a document first.", MessageType.Info);
            }

            // Keyboard shortcuts
            HandleKeyboardInput();
        }

        private void HandleKeyboardInput()
        {
            if (Event.current.type != EventType.KeyDown) return;

            float maxScroll = _viewer?.MaxScroll ?? 10000f;

            switch (Event.current.keyCode)
            {
                case KeyCode.Escape:
                    Close();
                    Event.current.Use();
                    break;

                case KeyCode.Home:
                    _scrollPosition = 0;
                    Event.current.Use();
                    Repaint();
                    break;

                case KeyCode.End:
                    _scrollPosition = maxScroll;
                    Event.current.Use();
                    Repaint();
                    break;

                case KeyCode.UpArrow:
                    _scrollPosition = Mathf.Max(0, _scrollPosition - 100);
                    Event.current.Use();
                    Repaint();
                    break;

                case KeyCode.DownArrow:
                    _scrollPosition = Mathf.Min(maxScroll, _scrollPosition + 100);
                    Event.current.Use();
                    Repaint();
                    break;

                case KeyCode.PageUp:
                    _scrollPosition = Mathf.Max(0, _scrollPosition - 500);
                    Event.current.Use();
                    Repaint();
                    break;

                case KeyCode.PageDown:
                    _scrollPosition = Mathf.Min(maxScroll, _scrollPosition + 500);
                    Event.current.Use();
                    Repaint();
                    break;
            }
        }

        private void RefreshFromSource()
        {
            if (_viewer != null)
            {
                _viewer.RefreshFromSource();
                Repaint();
            }
        }

        private void Update()
        {
            // Continuous repaint for smooth preview
            if (_isInitialized)
            {
                Repaint();
            }
        }

        private void OnEnable()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            if (_viewer != null)
            {
                _viewer.Unload();
            }

            if (_viewerObject != null)
            {
                DestroyImmediate(_viewerObject);
                _viewerObject = null;
            }

            if (_renderTexture != null)
            {
                DestroyImmediate(_renderTexture);
                _renderTexture = null;
            }

            _isInitialized = false;
        }
    }
}
