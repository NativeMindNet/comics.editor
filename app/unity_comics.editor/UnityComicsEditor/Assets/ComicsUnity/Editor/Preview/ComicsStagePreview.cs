using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NativeMind.ComicsViewer.Core;
using NativeMind.ComicsViewer.IO;
using ComicsUnity.Models;

namespace ComicsUnity.Preview
{
    /// <summary>
    /// Renders composed layer preview with transforms in IMGUI
    /// Uses AnimationProcessor from comics.engine for accurate transforms
    /// </summary>
    public class ComicsStagePreview
    {
        private FolderSource _source;
        private NativeMind.ComicsViewer.Models.Comics _comics;
        private AnimationProcessor _animationProcessor;

        private readonly Dictionary<string, Texture2D> _textureCache = new Dictionary<string, Texture2D>();
        private bool _isInitialized;

        /// <summary>
        /// Initialize with folder path (temp workspace)
        /// </summary>
        public bool Initialize(string folderPath)
        {
            try
            {
                if (!System.IO.Directory.Exists(folderPath))
                    return false;

                var dataJsonPath = System.IO.Path.Combine(folderPath, "data.json");
                if (!System.IO.File.Exists(dataJsonPath))
                    return false;

                _source = new FolderSource(folderPath);
                _source.Prepare();

                var json = _source.ReadDataJson();
                _comics = NativeMind.ComicsViewer.IO.ComicsParser.Parse(json);

                _animationProcessor = new AnimationProcessor(_comics);
                _isInitialized = true;

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to initialize stage preview: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Invalidate and reload from source
        /// </summary>
        public void Refresh()
        {
            if (_source == null) return;

            _source.Invalidate();
            ClearTextureCache();

            try
            {
                var json = _source.ReadDataJson();
                _comics = NativeMind.ComicsViewer.IO.ComicsParser.Parse(json);
                _animationProcessor = new AnimationProcessor(_comics);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to refresh stage preview: {e.Message}");
            }
        }

        /// <summary>
        /// Draw composed preview to rect
        /// </summary>
        public void Draw(Rect rect, float scroll, Cultures culture)
        {
            if (!_isInitialized || _comics == null)
            {
                EditorGUI.HelpBox(rect, "Preview not initialized", MessageType.Warning);
                return;
            }

            // Get transforms from animation processor
            var transforms = _animationProcessor.Process(scroll);

            // Calculate viewport scale to fit document in rect
            float docWidth = _comics.width;
            float docHeight = _comics.height;
            float scaleX = rect.width / docWidth;
            float scaleY = rect.height / docHeight;
            float viewScale = Mathf.Min(scaleX, scaleY);

            // Center offset
            float offsetX = rect.x + (rect.width - docWidth * viewScale) / 2f;
            float offsetY = rect.y + (rect.height - docHeight * viewScale) / 2f;

            // Draw background
            EditorGUI.DrawRect(rect, new Color(0.1f, 0.1f, 0.1f, 1f));

            // Draw document bounds
            var docRect = new Rect(offsetX, offsetY, docWidth * viewScale, docHeight * viewScale);
            EditorGUI.DrawRect(docRect, Color.black);

            // Draw layers back to front
            for (int i = 0; i < _comics.layers.Count && i < transforms.Length; i++)
            {
                var layer = _comics.layers[i];
                var transform = transforms[i];

                // Skip fully transparent
                if (transform.alpha <= 0.001f) continue;

                // Get texture
                var texture = GetLayerTexture(layer, (int)culture);
                if (texture == null) continue;

                // Calculate layer rect in document space
                float layerX = layer.x + transform.translation.x;
                float layerY = layer.y + transform.translation.y;
                float layerW = layer.width * transform.scale.x;
                float layerH = layer.height * transform.scale.y;

                // Transform to screen space
                var layerRect = new Rect(
                    offsetX + layerX * viewScale,
                    offsetY + layerY * viewScale,
                    layerW * viewScale,
                    layerH * viewScale
                );

                // Apply alpha
                var prevColor = GUI.color;
                GUI.color = new Color(1f, 1f, 1f, transform.alpha);

                // Apply rotation if any
                if (Mathf.Abs(transform.rotation) > 0.01f)
                {
                    // Calculate pivot in screen space
                    float pivotX = offsetX + (layer.x + transform.pivot.x) * viewScale;
                    float pivotY = offsetY + (layer.y + transform.pivot.y) * viewScale;
                    var pivotPoint = new Vector2(pivotX, pivotY);

                    // Save matrix
                    var prevMatrix = GUI.matrix;

                    // Apply rotation around pivot
                    GUIUtility.RotateAroundPivot(-transform.rotation, pivotPoint);

                    // Draw
                    GUI.DrawTexture(layerRect, texture, ScaleMode.StretchToFill);

                    // Restore matrix
                    GUI.matrix = prevMatrix;
                }
                else
                {
                    // No rotation - simple draw
                    GUI.DrawTexture(layerRect, texture, ScaleMode.StretchToFill);
                }

                // Restore color
                GUI.color = prevColor;
            }
        }

        /// <summary>
        /// Draw with document from editor session (for quick integration)
        /// </summary>
        public void DrawFromSession(Rect rect, ComicsEditorSession session)
        {
            Draw(rect, (float)session.Scroll, session.Culture);
        }

        private Texture2D GetLayerTexture(NativeMind.ComicsViewer.Models.Layer layer, int cultureIndex)
        {
            if (layer.images == null || layer.images.Count == 0)
                return null;

            // Find localized image
            string imageSrc = null;
            foreach (var img in layer.images)
            {
                if (img.locale == cultureIndex.ToString())
                {
                    imageSrc = img.src;
                    break;
                }
            }

            // Fallback to first
            if (imageSrc == null && layer.images.Count > 0)
            {
                imageSrc = layer.images[0].src;
            }

            if (string.IsNullOrEmpty(imageSrc))
                return null;

            // Cache key
            string cacheKey = $"{layer.id}_{cultureIndex}_{imageSrc}";

            if (_textureCache.TryGetValue(cacheKey, out var cached))
                return cached;

            // Load texture - try first tile or full image
            string tilePath = _source.GetTilePath(imageSrc, 0, 0, 1000);
            if (tilePath != null)
            {
                var tex = _source.LoadTileTexture(tilePath);
                if (tex != null)
                {
                    _textureCache[cacheKey] = tex;
                    return tex;
                }
            }

            return null;
        }

        public void ClearTextureCache()
        {
            foreach (var tex in _textureCache.Values)
            {
                if (tex != null)
                    Object.DestroyImmediate(tex);
            }
            _textureCache.Clear();
        }

        public void Dispose()
        {
            ClearTextureCache();
            _source?.Dispose();
            _source = null;
            _comics = null;
            _animationProcessor = null;
            _isInitialized = false;
        }
    }
}
