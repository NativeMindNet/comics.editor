using UnityEngine;
using UnityEditor;
using ComicsUnity.Models;

namespace ComicsUnity.Inspector
{
    /// <summary>
    /// IMGUI inspector for animation segments
    /// Draws type-specific fields based on selected anim
    /// </summary>
    public class AnimationInspector
    {
        private static readonly Color HeaderColor = new Color(0.2f, 0.2f, 0.2f, 1f);

        /// <summary>
        /// Draw inspector for the given animation
        /// Returns true if any value changed
        /// </summary>
        public bool Draw(Anim anim)
        {
            if (anim == null)
            {
                EditorGUILayout.HelpBox("No animation selected", MessageType.Info);
                return false;
            }

            bool changed = false;

            // Header with type name
            DrawHeader(anim.Type.ToString());

            EditorGUILayout.Space(4);

            // Base fields: Start, End
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Range", EditorStyles.boldLabel, GUILayout.Width(50));
            int newStart = EditorGUILayout.IntField(anim.Start, GUILayout.Width(80));
            EditorGUILayout.LabelField("to", GUILayout.Width(20));
            int newEnd = EditorGUILayout.IntField(anim.End, GUILayout.Width(80));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                // Ensure Start <= End
                if (newStart > newEnd)
                {
                    var tmp = newStart;
                    newStart = newEnd;
                    newEnd = tmp;
                }
                anim.Start = Mathf.Max(0, newStart);
                anim.End = Mathf.Max(0, newEnd);
                changed = true;
            }

            EditorGUILayout.Space(4);

            // Type-specific fields
            switch (anim)
            {
                case TranslateAnim t:
                    changed |= DrawTranslateFields(t);
                    break;
                case RotateAnim r:
                    changed |= DrawRotateFields(r);
                    break;
                case ScaleAnim s:
                    changed |= DrawScaleFields(s);
                    break;
                case AlphaAnim a:
                    changed |= DrawAlphaFields(a);
                    break;
                case SoundAnim:
                    // Sound anim only has Start/End
                    EditorGUILayout.LabelField("Start=End: play once, else loop", EditorStyles.miniLabel);
                    break;
            }

            return changed;
        }

        private void DrawHeader(string title)
        {
            var rect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.boldLabel, GUILayout.Height(22));
            EditorGUI.DrawRect(rect, HeaderColor);
            rect.x += 8;
            GUI.Label(rect, title, EditorStyles.boldLabel);
        }

        private bool DrawTranslateFields(TranslateAnim anim)
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("X", GUILayout.Width(20));
            int newX = EditorGUILayout.IntField(anim.X, GUILayout.Width(80));
            EditorGUILayout.LabelField("Y", GUILayout.Width(20));
            int newY = EditorGUILayout.IntField(anim.Y, GUILayout.Width(80));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                anim.X = newX;
                anim.Y = newY;
                return true;
            }
            return false;
        }

        private bool DrawRotateFields(RotateAnim anim)
        {
            EditorGUI.BeginChangeCheck();

            // Pivot
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Pivot X", GUILayout.Width(50));
            double newPivotX = EditorGUILayout.DoubleField(anim.PivotX, GUILayout.Width(60));
            EditorGUILayout.LabelField("Y", GUILayout.Width(20));
            double newPivotY = EditorGUILayout.DoubleField(anim.PivotY, GUILayout.Width(60));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            // Angle
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Angle", GUILayout.Width(50));
            double newAngle = EditorGUILayout.DoubleField(anim.Angle, GUILayout.Width(80));
            EditorGUILayout.LabelField("deg", EditorStyles.miniLabel, GUILayout.Width(30));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                anim.PivotX = newPivotX;
                anim.PivotY = newPivotY;
                anim.Angle = newAngle;
                return true;
            }
            return false;
        }

        private bool DrawScaleFields(ScaleAnim anim)
        {
            EditorGUI.BeginChangeCheck();

            // Pivot
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Pivot X", GUILayout.Width(50));
            double newPivotX = EditorGUILayout.DoubleField(anim.PivotX, GUILayout.Width(60));
            EditorGUILayout.LabelField("Y", GUILayout.Width(20));
            double newPivotY = EditorGUILayout.DoubleField(anim.PivotY, GUILayout.Width(60));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            // Scale
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Scale X", GUILayout.Width(50));
            double newScaleX = EditorGUILayout.DoubleField(anim.ScaleX, GUILayout.Width(60));
            EditorGUILayout.LabelField("Y", GUILayout.Width(20));
            double newScaleY = EditorGUILayout.DoubleField(anim.ScaleY, GUILayout.Width(60));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                anim.PivotX = newPivotX;
                anim.PivotY = newPivotY;
                anim.ScaleX = newScaleX;
                anim.ScaleY = newScaleY;
                return true;
            }
            return false;
        }

        private bool DrawAlphaFields(AlphaAnim anim)
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Alpha", GUILayout.Width(50));
            double newAlpha = EditorGUILayout.Slider((float)anim.Alpha, 0f, 1f, GUILayout.Width(150));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                anim.Alpha = newAlpha;
                return true;
            }
            return false;
        }
    }
}
