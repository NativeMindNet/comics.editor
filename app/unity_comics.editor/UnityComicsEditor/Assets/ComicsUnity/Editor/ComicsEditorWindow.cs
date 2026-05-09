using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using ComicsUnity.Models;
using ComicsUnity.Preview;
using ComicsUnity.Inspector;
using ComicsUnity.Timeline;
using ComicsUnity.Audio;

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

		// Timeline and Inspector
		AnimationTimeline _timeline = new AnimationTimeline();
		AnimationInspector _animInspector = new AnimationInspector();
		LayerInspector _layerInspector = new LayerInspector();

		// Audio preview
		EditorAudioManager _audioManager;

		[MenuItem("Window/Comics/Comics Editor")]
		public static void ShowWindow()
		{
			var w = GetWindow<ComicsEditorWindow>();
			w.titleContent = new GUIContent("Comics Editor");
			w.minSize = new Vector2(900, 600);
		}

		void OnEnable()
		{
			_audioManager = new EditorAudioManager();
			InitializeAudioManager();
		}

		void InitializeAudioManager()
		{
			if (_audioManager == null) return;
			var soundsPath = Path.Combine(FileManagerUnity.TempFolder, FileManagerUnity.FolderSounds);
			if (Directory.Exists(soundsPath))
				_audioManager.Initialize(soundsPath);
		}

		void OnDisable()
		{
			ClearPreviewCache();
			_stagePreview?.Dispose();
			_stagePreview = null;
			_audioManager?.Dispose();
			_audioManager = null;
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

			// Toolbar
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
			if (GUILayout.Button("New Comics", EditorStyles.toolbarButton))
			{
				_session.New(false);
				InvalidatePreviews();
				InitializeAudioManager();
			}
			if (GUILayout.Button("New Puzzle", EditorStyles.toolbarButton))
			{
				_session.New(true);
				InvalidatePreviews();
				InitializeAudioManager();
			}
			if (GUILayout.Button("Open…", EditorStyles.toolbarButton))
			{
				var path = EditorUtility.OpenFilePanel("Open document", "", "");
				if (!string.IsNullOrEmpty(path))
				{
					_session.Open(path);
					InvalidatePreviews();
					InitializeAudioManager();
				}
			}
			if (GUILayout.Button("Save", EditorStyles.toolbarButton))
				_session.Save();

			GUILayout.Space(10);

			// Undo/Redo buttons
			GUI.enabled = _session.UndoStack.CanUndo;
			if (GUILayout.Button("Undo", EditorStyles.toolbarButton, GUILayout.Width(50)))
			{
				_session.Undo();
				InvalidatePreviews();
			}
			GUI.enabled = _session.UndoStack.CanRedo;
			if (GUILayout.Button("Redo", EditorStyles.toolbarButton, GUILayout.Width(50)))
			{
				_session.Redo();
				InvalidatePreviews();
			}
			GUI.enabled = true;

			GUILayout.FlexibleSpace();

			// Sound toggle
			var soundEnabled = !_session.DisableSound;
			var newSoundEnabled = GUILayout.Toggle(soundEnabled, "Sound", EditorStyles.toolbarButton, GUILayout.Width(60));
			if (newSoundEnabled != soundEnabled)
			{
				_session.DisableSound = !newSoundEnabled;
				_audioManager?.SetEnabled(newSoundEnabled);
			}

			// Sync scroll toggle
			_session.SyncScrollToSelection = GUILayout.Toggle(_session.SyncScrollToSelection, "Sync Scroll", EditorStyles.toolbarButton, GUILayout.Width(80));

			EditorGUILayout.EndHorizontal();

			// Main layout: Left panel | Right panel (Preview + Timeline + Inspector)
			EditorGUILayout.BeginHorizontal();

			// Left panel
			_leftScroll = EditorGUILayout.BeginScrollView(_leftScroll, GUILayout.Width(280));
			DrawLeftPanel();
			EditorGUILayout.EndScrollView();

			// Right panel
			EditorGUILayout.BeginVertical();
			DrawRightPanel();
			EditorGUILayout.EndVertical();

			EditorGUILayout.EndHorizontal();

			// Handle keyboard shortcuts
			HandleKeyboard();
		}

		void DrawLeftPanel()
		{
			// Document settings
			EditorGUILayout.LabelField("Document", EditorStyles.boldLabel);
			EditorGUI.BeginChangeCheck();
			_session.Document.Width = EditorGUILayout.IntField("Width", _session.Document.Width);
			_session.Document.Height = EditorGUILayout.IntField("Height", _session.Document.Height);
			if (EditorGUI.EndChangeCheck())
				InvalidatePreviews();

			var sc = EditorGUILayout.Slider("Scroll", (float)_session.Scroll, 0f, 12000f);
			if (!Mathf.Approximately((float)_session.Scroll, sc))
			{
				var prevScroll = _session.PreviousScroll;
				_session.Scroll = sc;
				InvalidatePreviews();

				// Audio preview on scroll change
				if (Math.Abs(sc - prevScroll) > 0.1)
				{
					_audioManager?.ProcessScroll(_session.Document.Sounds, prevScroll, sc);
					_session.PreviousScroll = sc;
				}
			}

			var cul = (Cultures)EditorGUILayout.EnumPopup("Culture", _session.Culture);
			if (cul != _session.Culture)
			{
				_session.Culture = cul;
				InvalidatePreviews();
			}

			EditorGUILayout.Space(8);

			// Add buttons
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("+ Layer"))
			{
				var path = EditorUtility.OpenFilePanel("Image", "", "png,jpg,jpeg");
				if (!string.IsNullOrEmpty(path))
				{
					_session.AddLayer(path);
					InvalidatePreviews();
				}
			}
			if (GUILayout.Button("+ Sound"))
			{
				var path = EditorUtility.OpenFilePanel("Audio", "", "mp3");
				if (!string.IsNullOrEmpty(path))
				{
					_session.AddSound(path);
					InvalidatePreviews();
				}
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space(4);

			// Layers list
			EditorGUILayout.LabelField("Layers", EditorStyles.boldLabel);
			for (var i = 0; i < _session.Document.Layers.Count; i++)
			{
				EditorGUILayout.BeginHorizontal();
				var sel = _session.SelectedLayerIndex == i;
				if (GUILayout.Toggle(sel, $"Layer {i}", "Button"))
				{
					if (!sel)
					{
						_session.SelectedLayerIndex = i;
						_session.SelectedSoundIndex = -1;
						_session.SelectedAnim = null;
					}
				}
				if (GUILayout.Button("↑", GUILayout.Width(22)))
				{
					_session.MoveLayer(i, -1);
					InvalidatePreviews();
				}
				if (GUILayout.Button("↓", GUILayout.Width(22)))
				{
					_session.MoveLayer(i, 1);
					InvalidatePreviews();
				}
				if (GUILayout.Button("✕", GUILayout.Width(22)))
				{
					_session.DeleteLayer(i);
					InvalidatePreviews();
					break;
				}
				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.Space(4);

			// Sounds list
			EditorGUILayout.LabelField("Sounds", EditorStyles.boldLabel);
			for (var i = 0; i < _session.Document.Sounds.Count; i++)
			{
				EditorGUILayout.BeginHorizontal();
				var sel = _session.SelectedSoundIndex == i;
				var sound = _session.Document.Sounds[i];
				if (GUILayout.Toggle(sel, sound.File ?? $"Sound {i}", "Button"))
				{
					if (!sel)
					{
						_session.SelectedSoundIndex = i;
						_session.SelectedLayerIndex = -1;
						_session.SelectedAnim = null;
					}
				}
				if (GUILayout.Button("↑", GUILayout.Width(22)))
				{
					_session.MoveSound(i, -1);
				}
				if (GUILayout.Button("↓", GUILayout.Width(22)))
				{
					_session.MoveSound(i, 1);
				}
				if (GUILayout.Button("✕", GUILayout.Width(22)))
				{
					_session.DeleteSound(i);
					break;
				}
				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.Space(8);

			// Animation buttons
			DrawAnimationButtons();
		}

		void DrawAnimationButtons()
		{
			EditorGUILayout.LabelField("Add Animation", EditorStyles.boldLabel);

			if (_session.SelectedLayerIndex >= 0)
			{
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Translate", GUILayout.Height(24)))
				{
					_session.AddLayerAnim(_session.SelectedLayerIndex, AnimTypes.Translate);
					InvalidatePreviews();
				}
				if (GUILayout.Button("Rotate", GUILayout.Height(24)))
				{
					_session.AddLayerAnim(_session.SelectedLayerIndex, AnimTypes.Rotate);
					InvalidatePreviews();
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Scale", GUILayout.Height(24)))
				{
					_session.AddLayerAnim(_session.SelectedLayerIndex, AnimTypes.Scale);
					InvalidatePreviews();
				}
				if (GUILayout.Button("Alpha", GUILayout.Height(24)))
				{
					_session.AddLayerAnim(_session.SelectedLayerIndex, AnimTypes.Alpha);
					InvalidatePreviews();
				}
				EditorGUILayout.EndHorizontal();
			}
			else if (_session.SelectedSoundIndex >= 0)
			{
				if (GUILayout.Button("Add Sound Trigger", GUILayout.Height(24)))
				{
					_session.AddSoundAnim(_session.SelectedSoundIndex);
				}
			}
			else
			{
				EditorGUILayout.HelpBox("Select a layer or sound to add animations", MessageType.Info);
			}

			// Delete selected anim
			if (_session.SelectedAnim != null)
			{
				EditorGUILayout.Space(4);
				if (GUILayout.Button("Delete Selected Anim", GUILayout.Height(24)))
				{
					_session.RemoveSelectedAnim();
					InvalidatePreviews();
				}
			}
		}

		void DrawRightPanel()
		{
			// Preview panel (top)
			_previewScroll = EditorGUILayout.BeginScrollView(_previewScroll, GUILayout.Height(position.height * 0.5f));
			DrawPreviewPanel();
			EditorGUILayout.EndScrollView();

			// Timeline panel (middle)
			EditorGUILayout.LabelField("Timeline", EditorStyles.boldLabel);
			var timelineRect = GUILayoutUtility.GetRect(100, _timeline.GetHeight(), GUILayout.ExpandWidth(true));
			var newSelection = _timeline.Draw(timelineRect, _session);
			if (newSelection != null)
			{
				_session.SelectedAnim = newSelection;
				InvalidatePreviews();
			}

			EditorGUILayout.Space(4);

			// Inspector panel (bottom)
			EditorGUILayout.LabelField("Inspector", EditorStyles.boldLabel);
			DrawInspectorPanel();
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

		void DrawInspectorPanel()
		{
			// Animation inspector
			if (_session.SelectedAnim != null)
			{
				if (_animInspector.Draw(_session.SelectedAnim))
					InvalidatePreviews();
			}
			// Layer inspector
			else if (_session.SelectedLayerIndex >= 0 && _session.SelectedLayerIndex < _session.Document.Layers.Count)
			{
				var layer = _session.Document.Layers[_session.SelectedLayerIndex];
				var (imagePath, popupPath) = _layerInspector.Draw(layer, _session.Culture);

				if (!string.IsNullOrEmpty(imagePath))
				{
					_session.SetLayerImage(_session.SelectedLayerIndex, _session.Culture, imagePath);
					InvalidatePreviews();
				}
				if (!string.IsNullOrEmpty(popupPath))
				{
					_session.SetLayerPopup(_session.SelectedLayerIndex, _session.Culture, popupPath);
					InvalidatePreviews();
				}

				// Show evaluated transforms
				EditorGUILayout.Space(4);
				EditorGUILayout.LabelField("Current Transform (at scroll)", EditorStyles.miniLabel);
				_session.EvaluateLayer(_session.SelectedLayerIndex, out var tr, out var rot, out var sca, out var al);
				EditorGUILayout.LabelField($"  Translate: ({tr?.X ?? 0}, {tr?.Y ?? 0})");
				EditorGUILayout.LabelField($"  Rotate: {rot?.Angle ?? 0:F1}°");
				EditorGUILayout.LabelField($"  Scale: ({sca?.ScaleX ?? 1:F2}, {sca?.ScaleY ?? 1:F2})");
				EditorGUILayout.LabelField($"  Alpha: {al?.Alpha ?? 1:F2}");
			}
			else
			{
				EditorGUILayout.HelpBox("Select a layer, sound, or animation to inspect", MessageType.Info);
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

			float docW = _session.Document.Width;
			float docH = _session.Document.Height;
			if (docW <= 0 || docH <= 0)
			{
				EditorGUILayout.HelpBox("Invalid document dimensions.", MessageType.Warning);
				return;
			}

			float maxW = Mathf.Min(500f, position.width - 300);
			float maxH = position.height * 0.4f;
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
					var maxW = Mathf.Min(400f, position.width - 300);
					var ratio = tex.height > 0 ? (float)tex.width / tex.height : 1f;
					var h = maxW / Mathf.Max(0.01f, ratio);
					h = Mathf.Min(h, 300f);
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

		void HandleKeyboard()
		{
			var evt = Event.current;
			if (evt.type != EventType.KeyDown) return;

			// Undo: Ctrl+Z (Cmd+Z on macOS)
			if ((evt.control || evt.command) && evt.keyCode == KeyCode.Z && !evt.shift)
			{
				if (_session.UndoStack.CanUndo)
				{
					_session.Undo();
					InvalidatePreviews();
					evt.Use();
				}
				return;
			}

			// Redo: Ctrl+Y or Ctrl+Shift+Z
			if ((evt.control || evt.command) && evt.keyCode == KeyCode.Y ||
			    (evt.control || evt.command) && evt.shift && evt.keyCode == KeyCode.Z)
			{
				if (_session.UndoStack.CanRedo)
				{
					_session.Redo();
					InvalidatePreviews();
					evt.Use();
				}
				return;
			}

			switch (evt.keyCode)
			{
				case KeyCode.Delete:
				case KeyCode.Backspace:
					if (_session.SelectedAnim != null)
					{
						_session.RemoveSelectedAnim();
						InvalidatePreviews();
						evt.Use();
					}
					break;
			}
		}
	}
}
