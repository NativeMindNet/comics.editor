# Specifications: Comics Editor Engine Preview

> Version: 1.0
> Status: DRAFT
> Last Updated: 2026-05-08

## Overview

Provide a "Preview as Player" mode that instantiates the full `ComicsViewer` runtime engine within the editor, connected to the current document via `FolderSource`.

---

## 1. Architecture

```
┌─────────────────────────────────────────────────────────────┐
│  Preview Window (EditorWindow)                              │
│  ┌───────────────────────────────────────────────────────┐  │
│  │  Viewport (RenderTexture)                             │  │
│  │  ┌─────────────────────────────────────────────────┐  │  │
│  │  │                                                 │  │  │
│  │  │     ComicsViewer (hidden GameObject)            │  │  │
│  │  │     - FolderSource → temp workspace             │  │  │
│  │  │     - Camera → RenderTexture                    │  │  │
│  │  │     - TileRenderer, AnimationProcessor          │  │  │
│  │  │     - SoundManager (editor audio)               │  │  │
│  │  │                                                 │  │  │
│  │  └─────────────────────────────────────────────────┘  │  │
│  └───────────────────────────────────────────────────────┘  │
│  ┌───────────────────────────────────────────────────────┐  │
│  │  Controls: [Scroll Slider] [Size: 1080x1920 ▼] [×]   │  │
│  └───────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

---

## 2. PreviewWindow Class

```csharp
namespace Comics.Editor
{
    public class ComicsPreviewWindow : EditorWindow
    {
        private ComicsViewer _viewer;
        private RenderTexture _renderTexture;
        private Camera _previewCamera;
        private float _scrollPosition;
        private Vector2Int _viewportSize = new(1080, 1920);

        [MenuItem("Comics/Preview as Player %#p")]
        public static void ShowWindow()
        {
            var window = GetWindow<ComicsPreviewWindow>("Preview");
            window.Initialize(ComicsEditorSession.Current.TempFolderPath);
        }

        private void Initialize(string folderPath)
        {
            // Create hidden viewer
            var go = new GameObject("PreviewViewer");
            go.hideFlags = HideFlags.HideAndDontSave;
            _viewer = go.AddComponent<ComicsViewer>();

            // Connect to folder source
            _viewer.LoadFolder(folderPath);

            // Setup camera + render texture
            _previewCamera = go.AddComponent<Camera>();
            _previewCamera.orthographic = true;
            _previewCamera.clearFlags = CameraClearFlags.SolidColor;
            _previewCamera.backgroundColor = Color.black;

            _renderTexture = new RenderTexture(_viewportSize.x, _viewportSize.y, 24);
            _previewCamera.targetTexture = _renderTexture;

            // Enable sound
            _viewer.SetSoundEnabled(true);
        }

        private void OnGUI()
        {
            // Toolbar
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                _scrollPosition = EditorGUILayout.Slider("Scroll", _scrollPosition, 0, _viewer.MaxScroll);

                if (GUILayout.Button("1080x1920", EditorStyles.toolbarButton))
                    SetViewportSize(1080, 1920);
                if (GUILayout.Button("1920x1080", EditorStyles.toolbarButton))
                    SetViewportSize(1920, 1080);
            }
            EditorGUILayout.EndHorizontal();

            // Viewport
            var viewportRect = GUILayoutUtility.GetRect(
                _viewportSize.x,
                _viewportSize.y,
                GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true)
            );

            // Update viewer
            _viewer.SetScrollOffset(_scrollPosition);

            // Draw render texture
            GUI.DrawTexture(viewportRect, _renderTexture, ScaleMode.ScaleToFit);

            // Handle scroll input
            if (Event.current.type == EventType.ScrollWheel && viewportRect.Contains(Event.current.mousePosition))
            {
                _scrollPosition = Mathf.Clamp(
                    _scrollPosition + Event.current.delta.y * 50,
                    0,
                    _viewer.MaxScroll
                );
                Event.current.Use();
                Repaint();
            }

            // Escape to close
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
            {
                Close();
            }
        }

        private void OnDestroy()
        {
            if (_viewer != null)
                DestroyImmediate(_viewer.gameObject);
            if (_renderTexture != null)
                DestroyImmediate(_renderTexture);
        }
    }
}
```

---

## 3. Integration with ComicsViewer

### 3.1 Required Additions to ComicsViewer

```csharp
// In ComicsViewer.cs:

/// <summary>
/// Get maximum scroll value for this document.
/// </summary>
public float MaxScroll => _comics?.MaxScroll ?? 0;

/// <summary>
/// Enable/disable sound playback.
/// </summary>
public void SetSoundEnabled(bool enabled)
{
    _soundEnabled = enabled;
    if (_soundManager != null)
        _soundManager.SetEnabled(enabled);
}

/// <summary>
/// Set scroll position programmatically.
/// </summary>
public void SetScrollOffset(float offset)
{
    _scrollOffset = Mathf.Clamp(offset, 0, MaxScroll);
    UpdateViewport();
}
```

---

## 4. Audio in Preview

For editor audio playback:

```csharp
// Editor-compatible SoundManager
public class EditorSoundManager : ISoundManager
{
    private readonly Dictionary<string, AudioClip> _clips = new();

    public void Play(string soundPath, float volume)
    {
        if (!_clips.TryGetValue(soundPath, out var clip))
        {
            clip = _source.LoadSound(soundPath);
            _clips[soundPath] = clip;
        }

        // Use Editor audio API
        #if UNITY_EDITOR
        AudioSource.PlayClipAtPoint(clip, Vector3.zero, volume);
        // Or use EditorAudioUtility for more control
        #endif
    }
}
```

---

## 5. Viewport Size Presets

| Preset | Resolution | Aspect |
|--------|------------|--------|
| Phone Portrait | 1080 x 1920 | 9:16 |
| Phone Landscape | 1920 x 1080 | 16:9 |
| Tablet Portrait | 1536 x 2048 | 3:4 |
| Tablet Landscape | 2048 x 1536 | 4:3 |

---

## 6. Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| Ctrl+Shift+P | Open Preview |
| Escape | Close Preview |
| Space | Play/Pause auto-scroll |
| Arrow Up/Down | Step scroll |
| Home/End | Jump to start/end |

---

## 7. Differences from Inline Preview

| Aspect | Inline Preview | Engine Preview |
|--------|----------------|----------------|
| Purpose | Editing feedback | Validation |
| Transform source | Shared AnimationProcessor | Full ComicsViewer |
| Audio | No | Yes |
| Selection | Yes | No |
| Viewport | Editor panel size | Configurable |
| Performance | Optimized for scrub | Full quality |

---

## Dependencies

- `sdd-comics.engine-shared-core`: FolderSource
- `sdd-comics.engine-csharp-unity`: ComicsViewer
- `sdd-comics.editor-audio-preview`: EditorSoundManager

---

## Approval

- [x] Reviewed by: Anton
- [x] Approved on: 2026-05-08
