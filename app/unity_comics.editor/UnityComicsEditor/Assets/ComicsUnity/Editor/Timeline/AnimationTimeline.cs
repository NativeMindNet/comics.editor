using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ComicsUnity.Models;

namespace ComicsUnity.Timeline
{
    /// <summary>
    /// IMGUI dual-rail timeline for layer and sound animations
    /// </summary>
    public class AnimationTimeline
    {
        // Segment colors by type
        private static readonly Dictionary<AnimTypes, Color> SegmentColors = new Dictionary<AnimTypes, Color>
        {
            { AnimTypes.Translate, new Color(0.3f, 0.5f, 0.9f, 0.9f) },  // Blue
            { AnimTypes.Rotate,    new Color(0.3f, 0.8f, 0.4f, 0.9f) },  // Green
            { AnimTypes.Scale,     new Color(0.9f, 0.6f, 0.2f, 0.9f) },  // Orange
            { AnimTypes.Alpha,     new Color(0.7f, 0.4f, 0.9f, 0.9f) },  // Purple
            { AnimTypes.Sound,     new Color(0.9f, 0.3f, 0.3f, 0.9f) },  // Red
        };

        private static readonly Color RulerBgColor = new Color(0.15f, 0.15f, 0.15f, 1f);
        private static readonly Color RailBgColor = new Color(0.18f, 0.18f, 0.18f, 1f);
        private static readonly Color PlayheadColor = new Color(1f, 1f, 1f, 0.8f);
        private static readonly Color SelectionColor = new Color(1f, 1f, 1f, 0.3f);

        private const float RulerHeight = 20f;
        private const float RailHeight = 24f;
        private const float SegmentHeight = 18f;
        private const float HandleWidth = 6f;

        // Zoom: pixels per scroll unit
        private float _pixelsPerScroll = 0.1f;
        private float _scrollOffset = 0f; // horizontal scroll offset in scroll units

        // Drag state
        private enum DragMode { None, MoveSegment, ResizeStart, ResizeEnd }
        private DragMode _dragMode = DragMode.None;
        private Anim _dragAnim;
        private int _dragStartValue;
        private int _dragEndValue;
        private float _dragStartMouseX;

        // Segment rects for hit testing
        private readonly List<(Rect rect, Anim anim)> _segmentRects = new List<(Rect, Anim)>();

        /// <summary>
        /// Draw timeline for given session
        /// Returns selected anim if changed, null otherwise
        /// </summary>
        public Anim Draw(Rect rect, ComicsEditorSession session)
        {
            Anim newSelection = null;
            _segmentRects.Clear();

            // Layout
            var rulerRect = new Rect(rect.x, rect.y, rect.width, RulerHeight);
            var layerRailRect = new Rect(rect.x, rect.y + RulerHeight, rect.width, RailHeight);
            var soundRailRect = new Rect(rect.x, rect.y + RulerHeight + RailHeight, rect.width, RailHeight);

            // Calculate visible range
            float visibleStart = _scrollOffset;
            float visibleEnd = _scrollOffset + rect.width / _pixelsPerScroll;

            // Draw backgrounds
            EditorGUI.DrawRect(rulerRect, RulerBgColor);
            EditorGUI.DrawRect(layerRailRect, RailBgColor);
            EditorGUI.DrawRect(soundRailRect, new Color(RailBgColor.r - 0.02f, RailBgColor.g - 0.02f, RailBgColor.b - 0.02f, 1f));

            // Draw ruler
            DrawRuler(rulerRect, visibleStart, visibleEnd);

            // Draw rail labels
            GUI.Label(new Rect(rect.x + 4, layerRailRect.y + 2, 50, 20), "Layer", EditorStyles.miniLabel);
            GUI.Label(new Rect(rect.x + 4, soundRailRect.y + 2, 50, 20), "Sound", EditorStyles.miniLabel);

            // Draw layer segments
            if (session.SelectedLayerIndex >= 0 && session.SelectedLayerIndex < session.Document.Layers.Count)
            {
                var layer = session.Document.Layers[session.SelectedLayerIndex];
                DrawSegments(layerRailRect, layer.Animations, session.SelectedAnim, visibleStart);
            }

            // Draw sound segments
            if (session.SelectedSoundIndex >= 0 && session.SelectedSoundIndex < session.Document.Sounds.Count)
            {
                var sound = session.Document.Sounds[session.SelectedSoundIndex];
                DrawSegments(soundRailRect, sound.Animations, session.SelectedAnim, visibleStart);
            }

            // Draw playhead
            float playheadX = rect.x + ((float)session.Scroll - _scrollOffset) * _pixelsPerScroll;
            if (playheadX >= rect.x && playheadX <= rect.xMax)
            {
                EditorGUI.DrawRect(new Rect(playheadX - 1, rect.y, 2, rect.height), PlayheadColor);
            }

            // Handle input
            newSelection = HandleInput(rect, session);

            return newSelection;
        }

        private void DrawRuler(Rect rect, float visibleStart, float visibleEnd)
        {
            // Calculate tick interval based on zoom
            float tickInterval = CalculateTickInterval();

            // Draw ticks
            float startTick = Mathf.Floor(visibleStart / tickInterval) * tickInterval;
            for (float tick = startTick; tick <= visibleEnd; tick += tickInterval)
            {
                float x = rect.x + (tick - _scrollOffset) * _pixelsPerScroll;
                if (x < rect.x || x > rect.xMax) continue;

                // Draw tick line
                EditorGUI.DrawRect(new Rect(x, rect.y + 14, 1, 6), Color.gray);

                // Draw label
                string label = tick >= 1000 ? $"{tick / 1000:F0}k" : $"{tick:F0}";
                GUI.Label(new Rect(x + 2, rect.y, 40, 14), label, EditorStyles.miniLabel);
            }
        }

        private float CalculateTickInterval()
        {
            // Aim for ticks every ~80 pixels
            float idealInterval = 80f / _pixelsPerScroll;

            // Round to nice values: 100, 200, 500, 1000, 2000, 5000...
            float[] niceIntervals = { 50, 100, 200, 500, 1000, 2000, 5000, 10000 };
            foreach (var interval in niceIntervals)
            {
                if (interval >= idealInterval * 0.5f)
                    return interval;
            }
            return 10000;
        }

        private void DrawSegments(Rect railRect, IList<Anim> anims, Anim selectedAnim, float visibleStart)
        {
            if (anims == null) return;

            float segmentY = railRect.y + (railRect.height - SegmentHeight) / 2f;

            foreach (var anim in anims)
            {
                float startX = railRect.x + (anim.Start - _scrollOffset) * _pixelsPerScroll;
                float endX = railRect.x + (anim.End - _scrollOffset) * _pixelsPerScroll;
                float width = Mathf.Max(endX - startX, 4f); // Minimum width for visibility

                // Skip if completely outside view
                if (endX < railRect.x || startX > railRect.xMax) continue;

                var segmentRect = new Rect(startX, segmentY, width, SegmentHeight);
                _segmentRects.Add((segmentRect, anim));

                // Get color
                var color = SegmentColors.TryGetValue(anim.Type, out var c) ? c : Color.gray;

                // Draw segment
                EditorGUI.DrawRect(segmentRect, color);

                // Draw selection highlight
                if (anim == selectedAnim)
                {
                    EditorGUI.DrawRect(new Rect(segmentRect.x - 2, segmentRect.y - 2, segmentRect.width + 4, segmentRect.height + 4), SelectionColor);
                    // Border
                    DrawRectOutline(segmentRect, Color.white, 1);
                }

                // Draw resize handles
                if (anim == selectedAnim && width > HandleWidth * 2)
                {
                    // Left handle
                    EditorGUI.DrawRect(new Rect(segmentRect.x, segmentRect.y, HandleWidth, SegmentHeight), new Color(1, 1, 1, 0.3f));
                    // Right handle
                    EditorGUI.DrawRect(new Rect(segmentRect.xMax - HandleWidth, segmentRect.y, HandleWidth, SegmentHeight), new Color(1, 1, 1, 0.3f));
                }

                // Draw type label
                string typeLabel = anim.Type.ToString().Substring(0, 1); // T, R, S, A, S
                GUI.Label(new Rect(segmentRect.x + 4, segmentRect.y + 1, 20, SegmentHeight), typeLabel, EditorStyles.miniLabel);
            }
        }

        private void DrawRectOutline(Rect rect, Color color, float thickness)
        {
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, thickness), color); // Top
            EditorGUI.DrawRect(new Rect(rect.x, rect.yMax - thickness, rect.width, thickness), color); // Bottom
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, thickness, rect.height), color); // Left
            EditorGUI.DrawRect(new Rect(rect.xMax - thickness, rect.y, thickness, rect.height), color); // Right
        }

        private Anim HandleInput(Rect rect, ComicsEditorSession session)
        {
            Anim newSelection = null;
            var evt = Event.current;

            // Zoom with mouse wheel
            if (evt.type == EventType.ScrollWheel && rect.Contains(evt.mousePosition))
            {
                float zoomFactor = evt.delta.y > 0 ? 0.9f : 1.1f;
                float mouseScrollPos = _scrollOffset + (evt.mousePosition.x - rect.x) / _pixelsPerScroll;

                _pixelsPerScroll = Mathf.Clamp(_pixelsPerScroll * zoomFactor, 0.01f, 1f);

                // Adjust offset to keep mouse position stable
                _scrollOffset = mouseScrollPos - (evt.mousePosition.x - rect.x) / _pixelsPerScroll;
                _scrollOffset = Mathf.Max(0, _scrollOffset);

                evt.Use();
            }

            // Handle drag
            if (_dragMode != DragMode.None)
            {
                if (evt.type == EventType.MouseDrag)
                {
                    float deltaScroll = (evt.mousePosition.x - _dragStartMouseX) / _pixelsPerScroll;

                    switch (_dragMode)
                    {
                        case DragMode.MoveSegment:
                            int newStart = Mathf.Max(0, _dragStartValue + (int)deltaScroll);
                            int newEnd = Mathf.Max(0, _dragEndValue + (int)deltaScroll);
                            _dragAnim.Start = newStart;
                            _dragAnim.End = newEnd;
                            break;

                        case DragMode.ResizeStart:
                            int resizedStart = Mathf.Max(0, _dragStartValue + (int)deltaScroll);
                            if (resizedStart < _dragAnim.End)
                                _dragAnim.Start = resizedStart;
                            break;

                        case DragMode.ResizeEnd:
                            int resizedEnd = Mathf.Max(0, _dragEndValue + (int)deltaScroll);
                            if (resizedEnd > _dragAnim.Start)
                                _dragAnim.End = resizedEnd;
                            break;
                    }
                    evt.Use();
                }
                else if (evt.type == EventType.MouseUp)
                {
                    _dragMode = DragMode.None;
                    _dragAnim = null;
                    evt.Use();
                }
            }
            else
            {
                // Click to select
                if (evt.type == EventType.MouseDown && evt.button == 0 && rect.Contains(evt.mousePosition))
                {
                    // Check segment hit
                    foreach (var (segmentRect, anim) in _segmentRects)
                    {
                        if (segmentRect.Contains(evt.mousePosition))
                        {
                            newSelection = anim;

                            // Check if clicking on resize handles
                            float localX = evt.mousePosition.x - segmentRect.x;
                            if (localX < HandleWidth && segmentRect.width > HandleWidth * 2)
                            {
                                // Start resize
                                _dragMode = DragMode.ResizeStart;
                            }
                            else if (localX > segmentRect.width - HandleWidth && segmentRect.width > HandleWidth * 2)
                            {
                                // End resize
                                _dragMode = DragMode.ResizeEnd;
                            }
                            else
                            {
                                // Move
                                _dragMode = DragMode.MoveSegment;
                            }

                            _dragAnim = anim;
                            _dragStartValue = anim.Start;
                            _dragEndValue = anim.End;
                            _dragStartMouseX = evt.mousePosition.x;

                            evt.Use();
                            break;
                        }
                    }

                    // Click on empty space - could set playhead
                    if (newSelection == null && !evt.alt)
                    {
                        float clickScroll = _scrollOffset + (evt.mousePosition.x - rect.x) / _pixelsPerScroll;
                        session.Scroll = Mathf.Max(0, clickScroll);
                        evt.Use();
                    }
                }
            }

            // Pan with middle mouse or Alt+drag
            if (evt.type == EventType.MouseDrag && (evt.button == 2 || (evt.button == 0 && evt.alt)))
            {
                _scrollOffset -= evt.delta.x / _pixelsPerScroll;
                _scrollOffset = Mathf.Max(0, _scrollOffset);
                evt.Use();
            }

            return newSelection;
        }

        /// <summary>
        /// Total height needed for timeline
        /// </summary>
        public float GetHeight()
        {
            return RulerHeight + RailHeight * 2;
        }

        /// <summary>
        /// Reset zoom and scroll
        /// </summary>
        public void Reset()
        {
            _pixelsPerScroll = 0.1f;
            _scrollOffset = 0f;
        }

        /// <summary>
        /// Focus on scroll position
        /// </summary>
        public void FocusOn(float scroll, float viewWidth)
        {
            _scrollOffset = scroll - viewWidth / _pixelsPerScroll / 2f;
            _scrollOffset = Mathf.Max(0, _scrollOffset);
        }
    }
}
