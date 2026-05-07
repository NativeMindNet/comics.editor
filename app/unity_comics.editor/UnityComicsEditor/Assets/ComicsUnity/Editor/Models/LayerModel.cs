using System.Collections.Generic;
using System.Linq;

namespace ComicsUnity.Models
{
	public class LayerModel
	{
		public bool Preview { get; set; }
		public List<ImageModel> Images { get; set; } = new List<ImageModel>();
		public List<Anim> Animations { get; set; } = new List<Anim>();

		public ImageModel GetImage(Cultures culture, bool returnDefault = true)
		{
			var index = CulturesHelper.All.IndexOf(culture);
			var image = index >= 0 && index < Images.Count ? Images[index] : null;
			if (image == null)
				return returnDefault ? Images.FirstOrDefault() : null;
			return string.IsNullOrEmpty(image.File) && returnDefault ? Images.FirstOrDefault() : image;
		}

		public void SetImage(Cultures culture, string file, bool puzzle, bool popup)
		{
			var idx = CulturesHelper.All.IndexOf(culture);
			if (idx < 0 || idx >= Images.Count) return;
			Images[idx].Update(FileManagerUnity.FolderLayers, file, puzzle, popup);
		}

		public void Delete()
		{
			foreach (var x in Images)
				x.Delete(FileManagerUnity.FolderLayers);
		}

		public static LayerModel Create(string file, double scroll, bool puzzle)
		{
			var layer = new LayerModel();
			for (int i = 0; i < CulturesHelper.All.Count; i++)
			{
				var image = new ImageModel();
				layer.Images.Add(image);
				if (i == 0)
					image.Update(FileManagerUnity.FolderLayers, file, puzzle, false);
			}

			if (layer.Images.All(x => string.IsNullOrEmpty(x.File)))
				return null;

			layer.Animations.Add(new TranslateAnim { Y = (int)scroll });
			return layer;
		}
	}
}
