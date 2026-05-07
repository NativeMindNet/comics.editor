using System.IO;
using UnityEngine;

namespace ComicsUnity
{
	/// <summary>
	/// Replaces legacy ImageMagick tiling using Texture2D (Editor-only).
	/// </summary>
	public static class TileGeneratorUnity
	{
		public static string UpdateTiles(string folder, string oldFile, string newFile, bool puzzle, out Vector2Int size)
		{
			FileManagerUnity.DeleteTiles(folder, oldFile);

			var destRoot = Path.Combine(FileManagerUnity.TempFolder, folder);
			Directory.CreateDirectory(destRoot);

			var name = Path.GetFileNameWithoutExtension(newFile);
			var ext = Path.GetExtension(newFile);
			if (string.IsNullOrEmpty(ext)) ext = ".jpg";

			var bytes = File.ReadAllBytes(newFile);
			var src = new Texture2D(2, 2, TextureFormat.RGBA32, false);
			if (!src.LoadImage(bytes))
			{
				Debug.LogError($"Could not load image: {newFile}");
				size = Vector2Int.zero;
				Object.DestroyImmediate(src);
				return name + "_{0}_{1}_{2}" + ext;
			}

			size = new Vector2Int(src.width, src.height);
			var scales = puzzle ? FileManagerUnity.PuzzleScales : FileManagerUnity.ComicsScales;

			try
			{
				foreach (var tileScale in scales)
				{
					var scaleInt = (int)(tileScale * 1000);
					var tw = Mathf.Max(1, Mathf.RoundToInt(src.width * tileScale));
					var th = Mathf.Max(1, Mathf.RoundToInt(src.height * tileScale));
					var scaled = ScaleTexture(src, tw, th);
					try
					{
						WriteTiles(scaled, destRoot, name, scaleInt, ext);
					}
					finally
					{
						Object.DestroyImmediate(scaled);
					}
				}

				if (puzzle)
				{
					var fullName = name + "_{0}_{1}_{2}" + ext;
					var phPath = Path.Combine(destRoot, string.Format(fullName, "ph", 0, 0));
					if (src.width > FileManagerUnity.PlaceholderSize || src.height > FileManagerUnity.PlaceholderSize)
					{
						var ph = ScaleTexture(src, FileManagerUnity.PlaceholderSize, FileManagerUnity.PlaceholderSize);
						try
						{
							File.WriteAllBytes(phPath, ph.EncodeToPNG());
						}
						finally
						{
							Object.DestroyImmediate(ph);
						}
					}
					else
					{
						File.Copy(newFile, phPath, true);
					}
				}

				return name + "_{0}_{1}_{2}" + ext;
			}
			finally
			{
				Object.DestroyImmediate(src);
			}
		}

		private static void WriteTiles(Texture2D scaled, string destRoot, string name, int scaleInt, string ext)
		{
			var tw = scaled.width;
			var th = scaled.height;
			var tile = FileManagerUnity.TileSize;
			for (var row = 0; row * tile < th; row++)
			{
				for (var col = 0; col * tile < tw; col++)
				{
					var x = col * tile;
					var y = row * tile;
					var cw = Mathf.Min(tile, tw - x);
					var ch = Mathf.Min(tile, th - y);
					var tileTex = new Texture2D(cw, ch, TextureFormat.RGBA32, false);
					var pixels = scaled.GetPixels(x, y, cw, ch);
					tileTex.SetPixels(pixels);
					tileTex.Apply();
					var outName = $"{name}_{scaleInt}_{col}_{row}{ext}";
					var outPath = Path.Combine(destRoot, outName);
					File.WriteAllBytes(outPath, tileTex.EncodeToPNG());
					Object.DestroyImmediate(tileTex);
				}
			}
		}

		private static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
		{
			var rt = RenderTexture.GetTemporary(targetWidth, targetHeight, 0, RenderTextureFormat.ARGB32);
			Graphics.Blit(source, rt);
			var prev = RenderTexture.active;
			RenderTexture.active = rt;
			var dest = new Texture2D(targetWidth, targetHeight, TextureFormat.RGBA32, false);
			dest.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
			dest.Apply();
			RenderTexture.active = prev;
			RenderTexture.ReleaseTemporary(rt);
			return dest;
		}
	}
}
