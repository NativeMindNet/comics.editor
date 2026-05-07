using System.IO;
using System.IO.Compression;

namespace ComicsUnity
{
	public static class ZipUtility
	{
		public static void ExtractToFolder(string zipPath, string destinationFolder)
		{
			if (Directory.Exists(destinationFolder))
				Directory.Delete(destinationFolder, true);
			Directory.CreateDirectory(destinationFolder);
			ZipFile.ExtractToDirectory(zipPath, destinationFolder);
		}

		public static void ZipFromFolder(string sourceFolder, string zipPath)
		{
			if (File.Exists(zipPath))
				File.Delete(zipPath);
			ZipFile.CreateFromDirectory(sourceFolder, zipPath, CompressionLevel.Fastest, false);
		}
	}
}
