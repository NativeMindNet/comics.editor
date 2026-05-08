using UnityEngine;
using UnityEditor;
using ComicsUnity.Models;

namespace ComicsUnity.Inspector
{
    /// <summary>
    /// IMGUI inspector for layer properties (non-animation)
    /// Handles image/popup per culture
    /// </summary>
    public class LayerInspector
    {
        /// <summary>
        /// Draw layer inspector
        /// Returns (imageChanged, popupChanged) paths or null if not changed
        /// </summary>
        public (string imagePath, string popupPath) Draw(LayerModel layer, Cultures culture)
        {
            string newImagePath = null;
            string newPopupPath = null;

            if (layer == null)
            {
                EditorGUILayout.HelpBox("No layer selected", MessageType.Info);
                return (null, null);
            }

            EditorGUILayout.LabelField("Layer Image", EditorStyles.boldLabel);

            var image = layer.GetImage(culture, false);
            var currentFile = image?.File ?? "(none)";

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Image:", GUILayout.Width(50));
            EditorGUILayout.LabelField(currentFile, EditorStyles.miniLabel);
            if (GUILayout.Button("Change", GUILayout.Width(60)))
            {
                var path = EditorUtility.OpenFilePanel($"Select image for {culture}", "", "png,jpg,jpeg");
                if (!string.IsNullOrEmpty(path))
                    newImagePath = path;
            }
            EditorGUILayout.EndHorizontal();

            // Popup
            var popupFile = image?.Popup ?? "(none)";
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Popup:", GUILayout.Width(50));
            EditorGUILayout.LabelField(popupFile, EditorStyles.miniLabel);
            if (GUILayout.Button("Change", GUILayout.Width(60)))
            {
                var path = EditorUtility.OpenFilePanel($"Select popup for {culture}", "", "png,jpg,jpeg");
                if (!string.IsNullOrEmpty(path))
                    newPopupPath = path;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField($"Culture: {culture}", EditorStyles.miniLabel);

            return (newImagePath, newPopupPath);
        }
    }
}
