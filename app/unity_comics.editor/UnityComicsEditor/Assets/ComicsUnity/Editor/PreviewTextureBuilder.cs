using System.IO;
using UnityEngine;
using ComicsUnity.Models;

namespace ComicsUnity
{
	/// <summary>
	/// Build preview textures for <see cref="ImageModel"/> (single file or tiled), similar to legacy ImagePathConverter.
	/// </summary>
	public static class PreviewTextureBuilder
	{
		private const int PreviewScaleDivisor = 2;

		public static Texture2D Build(ImageModel image, string folderRelative)
		{
			if (image == null || string.IsNullOrEmpty(image.File)) return null;

			var folder = Path.Combine(FileManagerUnity.TempFolder, folderRelative);
			if (!image.IsTiles)
			{
				var path = Path.Combine(folder, image.File);
				return LoadTexture(path);
			}

			return BuildTiled(image, folder);
		}

		private static Texture2D BuildTiled(ImageModel image, string folder)
		{
			var scaleInt = (int)(FileManagerUnity.PuzzleScales[0] * 1000);
			var searchPattern = string.Format(image.File, scaleInt, "*", "*");
			var files = Directory.GetFiles(folder, Path.GetFileName(searchPattern));
			var w = Mathf.Max(1, image.Width / PreviewScaleDivisor);
			var h = Mathf.Max(1, image.Height / PreviewScaleDivisor);
			var canvas = new Texture2D(w, h, TextureFormat.RGBA32, false);
			var clear = new Color[w * h];
			for (var i = 0; i < clear.Length; i++) clear[i] = new Color(0, 0, 0, 0);
			canvas.SetPixels(clear);

			foreach (var file in files)
			{
				var parts = Path.GetFileNameWithoutExtension(file).Split('_');
				if (parts.Length < 4) continue;
				if (!int.TryParse(parts[parts.Length - 2], out var col)) continue;
				if (!int.TryParse(parts[parts.Length - 1], out var row)) continue;

				var tile = LoadTexture(file);
				if (tile == null) continue;

				var tileDisplay = FileManagerUnity.TileSize / PreviewScaleDivisor;
				var x0 = col * tileDisplay;
				var y0 = row * tileDisplay;
				var srcW = Mathf.Min(tile.width, w - x0);
				var srcH = Mathf.Min(tile.height, h - y0);
				if (srcW <= 0 || srcH <= 0)
				{
					Object.DestroyImmediate(tile);
					continue;
				}

				var srcPx = tile.GetPixels(0, 0, srcW, srcH);
				canvas.SetPixels(x0, y0, srcW, srcH, srcPx);
				Object.DestroyImmediate(tile);
			}

			canvas.Apply();
			return canvas;
		}

		private static Texture2D LoadTexture(string path)
		{
			if (!File.Exists(path)) return null;
			var bytes = File.ReadAllBytes(path);
			var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
			if (!tex.LoadImage(bytes))
			{
				Object.DestroyImmediate(tex);
				return null;
			}
			return tex;
		}
	}
}
