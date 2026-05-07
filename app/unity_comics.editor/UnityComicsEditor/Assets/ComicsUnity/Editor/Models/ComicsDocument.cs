using System.Collections.Generic;
using System.IO;
using System.Text;
using ComicsUnity;

namespace ComicsUnity.Models
{
	public class ComicsDocument
	{
		private const string DataFileName = "data.json";

		public int Width { get; set; }
		public int Height { get; set; }
		public List<LayerModel> Layers { get; set; } = new List<LayerModel>();
		public List<SoundModel> Sounds { get; set; } = new List<SoundModel>();

		public void Save()
		{
			var path = Path.Combine(FileManagerUnity.TempFolder, DataFileName);
			File.WriteAllText(path, this.ToJson(), Encoding.UTF8);
		}

		public static ComicsDocument Load()
		{
			var path = Path.Combine(FileManagerUnity.TempFolder, DataFileName);
			if (!File.Exists(path)) return new ComicsDocument { Width = 1080, Height = 2160 };
			var json = File.ReadAllText(path, Encoding.UTF8);
			var doc = json.FromJson<ComicsDocument>() ?? new ComicsDocument { Width = 1080, Height = 2160 };
			doc.Layers ??= new List<LayerModel>();
			doc.Sounds ??= new List<SoundModel>();
			foreach (var layer in doc.Layers)
			{
				layer.Images ??= new List<ImageModel>();
				layer.Animations ??= new List<Anim>();
			}
			foreach (var sound in doc.Sounds)
				sound.Animations ??= new List<Anim>();
			return doc;
		}
	}
}
