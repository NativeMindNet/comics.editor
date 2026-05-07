using System;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace ComicsUnity
{
	/// <summary>
	/// Mirrors legacy Comics.Editor.Utils.FileManager paths and naming; temp dir is under Unity temporaryCachePath.
	/// </summary>
	public static class FileManagerUnity
	{
		public static readonly string TempFolder = Path.Combine(Application.temporaryCachePath, "ComicsUnityEditor");

		public const string FolderLayers = "layers";
		public const string FolderSounds = "sounds";
		public const int TileSize = 512;
		public const int PlaceholderSize = 512;
		public static readonly float[] ComicsScales = { 1.0f };
		public static readonly float[] PuzzleScales = { 1.0f, 0.5f, 0.25f, 0.125f };

		private static string GetFileExt(string name)
		{
			var ext = Path.GetExtension(name);
			return !string.IsNullOrEmpty(ext) ? ext : ".jpg";
		}

		public static bool CheckFile(string folder, string oldFile, string newFile)
		{
			var name = Path.GetFileNameWithoutExtension(newFile);
			var ext = GetFileExt(newFile);
			var singleName = name + ext;
			var tileName = name + "_{0}_{1}_{2}" + ext;
			var basePath = Path.Combine(TempFolder, folder);
			return oldFile == singleName || oldFile == tileName ||
			       (!File.Exists(Path.Combine(basePath, singleName)) &&
			        !Directory.GetFiles(basePath, string.Format(tileName, "*", "*", "*")).Any());
		}

		public static string Update(string folder, string oldFile, string newFile)
		{
			Delete(folder, oldFile);
			var name = Path.GetFileNameWithoutExtension(newFile) + GetFileExt(newFile);
			var dest = Path.Combine(TempFolder, folder);
			Directory.CreateDirectory(dest);
			File.Copy(newFile, Path.Combine(dest, name), true);
			return name;
		}

		public static void Delete(string folder, string oldFile)
		{
			if (string.IsNullOrEmpty(oldFile)) return;
			var path = Path.Combine(TempFolder, folder, oldFile);
			if (File.Exists(path))
				File.Delete(path);
		}

		public static void DeleteTiles(string folder, string oldFile)
		{
			if (string.IsNullOrEmpty(oldFile)) return;
			var pattern = Path.Combine(TempFolder, folder, string.Format(oldFile, "*", "*", "*"));
			var dir = Path.GetDirectoryName(pattern);
			var filePattern = Path.GetFileName(pattern);
			if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir)) return;
			foreach (var file in Directory.GetFiles(dir, filePattern))
				File.Delete(file);
		}

		public static void DeleteFolder(int errorCount = 0)
		{
			try
			{
				if (Directory.Exists(TempFolder))
					Directory.Delete(TempFolder, true);
			}
			catch
			{
				if (errorCount > 10)
					throw;
				Thread.Sleep(100);
				DeleteFolder(errorCount + 1);
			}
		}

		public static void CreateFolders()
		{
			foreach (var folder in new[] { FolderLayers, FolderSounds })
			{
				var path = Path.Combine(TempFolder, folder);
				if (!Directory.Exists(path))
					Directory.CreateDirectory(path);
			}
		}
	}
}
