using System;
using System.IO;
using UnityEditor;
using ComicsUnity.Models;

namespace ComicsUnity
{
	public sealed class ComicsEditorSession
	{
		public string FilePath { get; private set; }
		public ComicsDocument Document { get; private set; } = new ComicsDocument();
		public double Scroll { get; set; }
		public Cultures Culture { get; set; } = Cultures.En;
		public int SelectedLayerIndex { get; set; } = -1;
		public int SelectedSoundIndex { get; set; } = -1;

		// Animation selection
		private Anim _selectedAnim;
		public Anim SelectedAnim
		{
			get => _selectedAnim;
			set
			{
				if (_selectedAnim == value) return;
				_selectedAnim = value;
				if (SyncScrollToSelection && _selectedAnim != null)
					ApplyAutoSeek();
			}
		}

		public bool SyncScrollToSelection { get; set; } = true;

		private void ApplyAutoSeek()
		{
			if (_selectedAnim == null) return;
			// For default translate (End=0), seek to layer Y - 1000
			// Otherwise seek to anim.End
			if (_selectedAnim.End == 0 && _selectedAnim is TranslateAnim t)
				Scroll = Math.Max(t.Y - 1000, 0);
			else
				Scroll = _selectedAnim.End;
		}

		public bool IsPuzzle =>
			!string.IsNullOrEmpty(FilePath) &&
			Path.GetExtension(FilePath).Equals(".puzzle", StringComparison.OrdinalIgnoreCase);

		public string Title =>
			string.IsNullOrEmpty(FilePath) ? "Comics Editor — Untitled" :
			$"Comics Editor — {Path.GetFileName(FilePath)}";

		public void New(bool puzzle)
		{
			FileManagerUnity.DeleteFolder();
			FilePath = puzzle ? "<new>.puzzle" : "<new>.comics";
			FileManagerUnity.CreateFolders();
			Document = new ComicsDocument { Width = 1080, Height = 2160 };
			SelectedLayerIndex = -1;
			Scroll = 0;
		}

		public void Open(string path)
		{
			FilePath = path;
			FileManagerUnity.DeleteFolder();
			if (!string.IsNullOrEmpty(path) && File.Exists(path))
				ZipUtility.ExtractToFolder(path, FileManagerUnity.TempFolder);
			FileManagerUnity.CreateFolders();
			Document = ComicsDocument.Load();
			SelectedLayerIndex = Document.Layers.Count > 0 ? 0 : -1;
		}

		public void Save()
		{
			var path = FilePath;
			if (string.IsNullOrEmpty(path) || path.StartsWith("<"))
			{
				path = EditorUtility.SaveFilePanel(
					"Save",
					"",
					"document",
					IsPuzzle ? "puzzle" : "comics");
				if (string.IsNullOrEmpty(path)) return;
				if (!path.EndsWith(IsPuzzle ? ".puzzle" : ".comics", StringComparison.OrdinalIgnoreCase))
					path += IsPuzzle ? ".puzzle" : ".comics";
				FilePath = path;
			}

			Document.Save();
			if (File.Exists(FilePath))
				File.Delete(FilePath);
			ZipUtility.ZipFromFolder(FileManagerUnity.TempFolder, FilePath);
		}

		public void AddLayer(string imagePath)
		{
			var layer = LayerModel.Create(imagePath, Scroll, IsPuzzle);
			if (layer == null) return;
			Document.Layers.Add(layer);
			SelectedLayerIndex = Document.Layers.Count - 1;
		}

		public void AddSound(string audioPath)
		{
			var sound = SoundModel.Create(audioPath, Scroll);
			Document.Sounds.Add(sound);
		}

		public void MoveLayer(int index, int delta)
		{
			var newIndex = index + delta;
			if (index < 0 || index >= Document.Layers.Count) return;
			if (newIndex < 0 || newIndex >= Document.Layers.Count) return;
			var l = Document.Layers[index];
			Document.Layers.RemoveAt(index);
			Document.Layers.Insert(newIndex, l);
			SelectedLayerIndex = newIndex;
		}

		public void DeleteLayer(int index)
		{
			if (index < 0 || index >= Document.Layers.Count) return;
			Document.Layers[index].Delete();
			Document.Layers.RemoveAt(index);
			if (SelectedLayerIndex >= Document.Layers.Count)
				SelectedLayerIndex = Document.Layers.Count - 1;
		}

		public void EvaluateLayer(int index, out TranslateAnim translate, out RotateAnim rotate,
			out ScaleAnim scale, out AlphaAnim alpha)
		{
			translate = rotate = null;
			scale = null;
			alpha = null;
			if (index < 0 || index >= Document.Layers.Count) return;
			var layer = Document.Layers[index];
			translate = Anim.Interpolate<TranslateAnim>(layer.Animations, SelectedAnim, Scroll);
			rotate = Anim.Interpolate<RotateAnim>(layer.Animations, SelectedAnim, Scroll);
			scale = Anim.Interpolate<ScaleAnim>(layer.Animations, SelectedAnim, Scroll);
			alpha = Anim.Interpolate<AlphaAnim>(layer.Animations, SelectedAnim, Scroll);
		}

		#region Layer Animation CRUD

		public Anim AddLayerAnim(int layerIndex, AnimTypes type)
		{
			if (layerIndex < 0 || layerIndex >= Document.Layers.Count) return null;
			var layer = Document.Layers[layerIndex];
			var anim = Anim.Add(layer.Animations, type, Scroll);
			SelectedAnim = anim;
			return anim;
		}

		public void RemoveLayerAnim(int layerIndex, Anim anim)
		{
			if (layerIndex < 0 || layerIndex >= Document.Layers.Count) return;
			if (anim == null) return;
			var layer = Document.Layers[layerIndex];
			layer.Animations.Remove(anim);
			if (SelectedAnim == anim)
				SelectedAnim = null;
		}

		public void RemoveSelectedAnim()
		{
			if (SelectedAnim == null) return;

			// Check if it's in a layer
			if (SelectedLayerIndex >= 0 && SelectedLayerIndex < Document.Layers.Count)
			{
				var layer = Document.Layers[SelectedLayerIndex];
				if (layer.Animations.Contains(SelectedAnim))
				{
					layer.Animations.Remove(SelectedAnim);
					SelectedAnim = null;
					return;
				}
			}

			// Check if it's in a sound
			if (SelectedSoundIndex >= 0 && SelectedSoundIndex < Document.Sounds.Count)
			{
				var sound = Document.Sounds[SelectedSoundIndex];
				if (sound.Animations.Contains(SelectedAnim))
				{
					sound.Animations.Remove(SelectedAnim);
					SelectedAnim = null;
					return;
				}
			}
		}

		#endregion

		#region Sound Animation CRUD

		public Anim AddSoundAnim(int soundIndex)
		{
			if (soundIndex < 0 || soundIndex >= Document.Sounds.Count) return null;
			var sound = Document.Sounds[soundIndex];
			var anim = new SoundAnim { Start = (int)Scroll, End = (int)Scroll + 200 };
			sound.Animations.Add(anim);
			SelectedAnim = anim;
			return anim;
		}

		public void RemoveSoundAnim(int soundIndex, Anim anim)
		{
			if (soundIndex < 0 || soundIndex >= Document.Sounds.Count) return;
			if (anim == null) return;
			var sound = Document.Sounds[soundIndex];
			sound.Animations.Remove(anim);
			if (SelectedAnim == anim)
				SelectedAnim = null;
		}

		#endregion

		#region Layer Image/Popup

		public void SetLayerImage(int layerIndex, Cultures culture, string filePath)
		{
			if (layerIndex < 0 || layerIndex >= Document.Layers.Count) return;
			if (string.IsNullOrEmpty(filePath)) return;
			var layer = Document.Layers[layerIndex];
			layer.SetImage(culture, filePath, IsPuzzle, false);
		}

		public void SetLayerPopup(int layerIndex, Cultures culture, string filePath)
		{
			if (layerIndex < 0 || layerIndex >= Document.Layers.Count) return;
			if (string.IsNullOrEmpty(filePath)) return;
			var layer = Document.Layers[layerIndex];
			layer.SetImage(culture, filePath, IsPuzzle, true);
		}

		#endregion

		#region Sound Management

		public void MoveSound(int index, int delta)
		{
			var newIndex = index + delta;
			if (index < 0 || index >= Document.Sounds.Count) return;
			if (newIndex < 0 || newIndex >= Document.Sounds.Count) return;
			var s = Document.Sounds[index];
			Document.Sounds.RemoveAt(index);
			Document.Sounds.Insert(newIndex, s);
			SelectedSoundIndex = newIndex;
		}

		public void DeleteSound(int index)
		{
			if (index < 0 || index >= Document.Sounds.Count) return;
			Document.Sounds[index].Delete();
			Document.Sounds.RemoveAt(index);
			if (SelectedSoundIndex >= Document.Sounds.Count)
				SelectedSoundIndex = Document.Sounds.Count - 1;
		}

		#endregion
	}
}
