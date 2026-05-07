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
			translate = Anim.Interpolate<TranslateAnim>(layer.Animations, null, Scroll);
			rotate = Anim.Interpolate<RotateAnim>(layer.Animations, null, Scroll);
			scale = Anim.Interpolate<ScaleAnim>(layer.Animations, null, Scroll);
			alpha = Anim.Interpolate<AlphaAnim>(layer.Animations, null, Scroll);
		}
	}
}
