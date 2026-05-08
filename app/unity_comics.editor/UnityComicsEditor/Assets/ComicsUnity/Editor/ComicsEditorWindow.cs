using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ComicsUnity.Models;
using ComicsUnity.Preview;

namespace ComicsUnity
{
	public sealed class ComicsEditorWindow : EditorWindow
	{
		ComicsEditorSession _session = new ComicsEditorSession();
		Vector2 _leftScroll;
		Vector2 _previewScroll;
		readonly Dictionary<int, Texture2D> _previewCache = new Dictionary<int, Texture2D>();

		// Composed preview mode
		enum PreviewMode { Stacked, Composed }
		PreviewMode _previewMode = PreviewMode.Composed;
		ComicsStagePreview _stagePreview;

		[MenuItem("Window/Comics/Comics Editor")]
		public static void ShowWindow()
		{
			var w = GetWindow<ComicsEditorWindow>();
			w.titleContent = new GUIContent("Comics Editor");
			w.minSize = new Vector2(800, 500);
		}

		void OnDisable()
		{
			ClearPreviewCache();
			_stagePreview?.Dispose();
			_stagePreview = null;
		}

		void ClearPreviewCache()
		{
			foreach (var kv in _previewCache)
				if (kv.Value != null)
					Object.DestroyImmediate(kv.Value);
			_previewCache.Clear();
		}

		void InvalidatePreviews()
		{
			ClearPreviewCache();
			_stagePreview?.Refresh();
		}

		void EnsureStagePreview()
		{
			if (_stagePreview == null)
			{
				_stagePreview = new ComicsStagePreview();
			}

			if (!System.IO.Directory.Exists(FileManagerUnity.TempFolder))
				return;

			_stagePreview.Initialize(FileManagerUnity.TempFolder);
		}

		void OnGUI()
		{
			titleContent = new GUIContent(_session.Title);

			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
			if (GUILayout.Button("New Comics", EditorStyles.toolbarButton))
			{
				_session.New(false);
				InvalidatePreviews();
			}
			if (GUILayout.Button("New Puzzle", EditorStyles.toolbarButton))
			{
				_session.New(true);
				InvalidatePreviews();
			}
			if (GUILayout.Button("Open…", EditorStyles.toolbarButton))
			{
				var path = EditorUtility.OpenFilePanel("Open document", "", "");
				if (!string.IsNullOrEmpty(path))
				{
					_session.Open(path);
					InvalidatePreviews();
				}
			}
			if (GUILayout.Button("Save", EditorStyles.toolbarButton))
				_session.Save();
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();

			_leftScroll = EditorGUILayout.BeginScrollView(_leftScroll, GUILayout.Width(300));
			DrawLeftPanel();
			EditorGUILayout.EndScrollView();

			_previewScroll = EditorGUILayout.BeginScrollView(_previewScroll);
			DrawPreviewPanel();
			EditorGUILayout.EndScrollView();

			EditorGUILayout.EndHorizontal();
		}

		void DrawLeftPanel()
		{
			EditorGUI.BeginChangeCheck();
			_session.Document.Width = EditorGUILayout.IntField("Canvas width", _session.Document.Width);
			_session.Document.Height = EditorGUILayout.IntField("Canvas height", _session.Document.Height);
			if (EditorGUI.EndChangeCheck())
				InvalidatePreviews();

			var sc = EditorGUILayout.Slider("Scroll (scene)", (float)_session.Scroll, 0f, 12000f);
			if (!Mathf.Approximately((float)_session.Scroll, sc))
			{
				_session.Scroll = sc;
				InvalidatePreviews();
			}

			var cul = (Cultures)EditorGUILayout.EnumPopup("Culture", _session.Culture);
			if (cul != _session.Culture)
			{
				_session.Culture = cul;
				InvalidatePreviews();
			}

			EditorGUILayout.Space(8);
			if (GUILayout.Button("Add layer (image)…"))
			{
				var path = EditorUtility.OpenFilePanel("Image", "", "png,jpg,jpeg");
				if (!string.IsNullOrEmpty(path))
				{
					_session.AddLayer(path);
					InvalidatePreviews();
				}
			}
			if (GUILayout.Button("Add sound (mp3)…"))
			{
				var path = EditorUtility.OpenFilePanel("Audio", "", "mp3");
				if (!string.IsNullOrEmpty(path))
				{
					_session.AddSound(path);
					InvalidatePreviews();
				}
			}

			EditorGUILayout.LabelField("Layers", EditorStyles.boldLabel);
			for (var i = 0; i < _session.Document.Layers.Count; i++)
			{
				EditorGUILayout.BeginHorizontal();
				var sel = _session.SelectedLayerIndex == i;
				var n = GUILayout.Toggle(sel, $"Layer {i}", "Button");
				if (n && !sel) _session.SelectedLayerIndex = i;
				if (GUILayout.Button("↑", GUILayout.Width(24)))
				{
					_session.MoveLayer(i, -1);
					InvalidatePreviews();
				}
				if (GUILayout.Button("↓", GUILayout.Width(24)))
				{
					_session.MoveLayer(i, 1);
					InvalidatePreviews();
				}
				if (GUILayout.Button("✕", GUILayout.Width(24)))
				{
					_session.DeleteLayer(i);
					InvalidatePreviews();
					break;
				}
				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.Space(6);
			EditorGUILayout.LabelField($"Sounds: {_session.Document.Sounds.Count}", EditorStyles.miniLabel);

			if (_session.SelectedLayerIndex >= 0 &&
			    _session.SelectedLayerIndex < _session.Document.Layers.Count)
			{
				EditorGUILayout.Space(8);
				EditorGUILayout.LabelField("Selected layer (at scroll)", EditorStyles.boldLabel);
				_session.EvaluateLayer(_session.SelectedLayerIndex, out var tr, out var rot, out var sca, out var al);
				EditorGUILayout.LabelField($"Translate", $"{tr?.X ?? 0}, {tr?.Y ?? 0}");
				EditorGUILayout.LabelField($"Rotate", $"{rot?.Angle ?? 0:F1}°");
				EditorGUILayout.LabelField($"Scale", $"{sca?.ScaleX ?? 1:F2}, {sca?.ScaleY ?? 1:F2}");
				EditorGUILayout.LabelField($"Alpha", $"{al?.Alpha ?? 1:F2}");

				if (GUILayout.Button("Add translate key segment"))
				{
					var layer = _session.Document.Layers[_session.SelectedLayerIndex];
					Anim.Add(layer.Animations, AnimTypes.Translate, _session.Scroll);
					InvalidatePreviews();
				}
			}
		}

		void DrawPreviewPanel()
		{
			// Preview mode toggle
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel, GUILayout.Width(60));
			if (GUILayout.Toggle(_previewMode == PreviewMode.Composed, "Composed", EditorStyles.miniButtonLeft, GUILayout.Width(70)))
				_previewMode = PreviewMode.Composed;
			if (GUILayout.Toggle(_previewMode == PreviewMode.Stacked, "Stacked", EditorStyles.miniButtonRight, GUILayout.Width(70)))
				_previewMode = PreviewMode.Stacked;
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space(4);

			if (_previewMode == PreviewMode.Composed)
			{
				DrawComposedPreview();
			}
			else
			{
				DrawStackedPreview();
			}
		}

		void DrawComposedPreview()
		{
			EnsureStagePreview();

			if (_session.Document.Layers.Count == 0)
			{
				EditorGUILayout.HelpBox("Add a layer or open a .comics / .puzzle document.", MessageType.Info);
				return;
			}

			// Calculate preview rect maintaining document aspect ratio
			float docW = _session.Document.Width;
			float docH = _session.Document.Height;
			if (docW <= 0 || docH <= 0)
			{
				EditorGUILayout.HelpBox("Invalid document dimensions.", MessageType.Warning);
				return;
			}

			float maxW = Mathf.Min(600f, position.width - 320);
			float maxH = position.height - 100;
			float aspect = docW / docH;

			float previewW, previewH;
			if (maxW / maxH > aspect)
			{
				previewH = maxH;
				previewW = previewH * aspect;
			}
			else
			{
				previewW = maxW;
				previewH = previewW / aspect;
			}

			var rect = GUILayoutUtility.GetRect(previewW, previewH);
			_stagePreview?.Draw(rect, (float)_session.Scroll, _session.Culture);

			// Show current scroll info
			EditorGUILayout.LabelField($"Scroll: {_session.Scroll:F0} | Document: {docW}×{docH}", EditorStyles.miniLabel);
		}

		void DrawStackedPreview()
		{
			EditorGUILayout.LabelField("Layers (back → front)", EditorStyles.miniLabel);
			for (var i = 0; i < _session.Document.Layers.Count; i++)
			{
				var layer = _session.Document.Layers[i];
				var image = layer.GetImage(_session.Culture);
				if (image == null || string.IsNullOrEmpty(image.File))
					continue;

				if (!_previewCache.TryGetValue(i, out var tex) || tex == null)
				{
					tex = PreviewTextureBuilder.Build(image, FileManagerUnity.FolderLayers);
					_previewCache[i] = tex;
				}

				_session.EvaluateLayer(i, out var translate, out var rotate, out var scale, out var alpha);
				var dx = translate?.X ?? 0;
				var dy = translate?.Y ?? 0;

				EditorGUILayout.LabelField($"Layer {i} — pos ({dx}, {dy}) rot {rotate?.Angle ?? 0:F0}° scale ({scale?.ScaleX ?? 1:F2},{scale?.ScaleY ?? 1:F2}) α {alpha?.Alpha ?? 1:F2}");
				if (tex != null)
				{
					var maxW = Mathf.Min(500f, position.width - 320);
					var ratio = tex.height > 0 ? (float)tex.width / tex.height : 1f;
					var h = maxW / Mathf.Max(0.01f, ratio);
					h = Mathf.Min(h, 400f);
					var r = GUILayoutUtility.GetRect(maxW, h);
					var prev = GUI.color;
					GUI.color = new Color(1f, 1f, 1f, (float)(alpha?.Alpha ?? 1));
					GUI.DrawTexture(r, tex, ScaleMode.ScaleToFit);
					GUI.color = prev;
				}

				EditorGUILayout.Space(4);
			}

			if (_session.Document.Layers.Count == 0)
				EditorGUILayout.HelpBox("Add a layer or open a .comics / .puzzle document.", MessageType.Info);
		}
	}
}
