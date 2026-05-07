using System.IO;
using Newtonsoft.Json;
using UnityEditor;

namespace ComicsUnity.Models
{
	public class ImageModel : NotifyPropertyChanged
	{
		private string _file;
		private string _popup;

		public string File
		{
			get => _file;
			set
			{
				if (_file == value) return;
				_file = value;
				OnPropertyChanged(nameof(File));
			}
		}

		public string Popup
		{
			get => _popup;
			set
			{
				if (_popup == value) return;
				_popup = value;
				OnPropertyChanged(nameof(Popup));
			}
		}

		public int Width { get; set; }
		public int Height { get; set; }

		[JsonIgnore]
		public bool IsTiles => !string.IsNullOrEmpty(File) && File.Contains("{0}");

		public void Update(string folder, string file, bool puzzle, bool popup)
		{
			if (!FileManagerUnity.CheckFile(folder, popup ? Popup : File, file))
			{
				EditorUtility.DisplayDialog("Error", "File with this name already exists.", "OK");
				return;
			}

			if (popup)
			{
				Popup = FileManagerUnity.Update(folder, Popup, file);
				return;
			}

			File = TileGeneratorUnity.UpdateTiles(folder, File, file, puzzle, out var size);
			Width = size.x;
			Height = size.y;
		}

		public void Delete(string folder)
		{
			if (IsTiles)
				FileManagerUnity.DeleteTiles(folder, File);
			else
				FileManagerUnity.Delete(folder, File);
		}
	}
}
