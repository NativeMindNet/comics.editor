using System;
using System.IO;
using ComicsUnity.Models;

namespace ComicsUnity.Commands
{
	/// <summary>
	/// Command to set a layer's image with file backup for undo.
	/// </summary>
	public class SetLayerImageCommand : IEditCommand, IDisposable
	{
		private readonly LayerModel _layer;
		private readonly Cultures _culture;
		private readonly string _oldFile;
		private readonly int _oldWidth;
		private readonly int _oldHeight;
		private readonly string _newFilePath;
		private readonly string _backupPath;
		private readonly bool _isPuzzle;
		private readonly bool _isPopup;
		private readonly int _imageIndex;

		public string Description => _isPopup ? $"Change layer popup ({_culture})" : $"Change layer image ({_culture})";

		public SetLayerImageCommand(LayerModel layer, Cultures culture, string newFilePath, bool isPuzzle, bool isPopup)
		{
			_layer = layer;
			_culture = culture;
			_isPuzzle = isPuzzle;
			_isPopup = isPopup;
			_newFilePath = newFilePath;

			// Get image index for this culture
			_imageIndex = CulturesHelper.All.IndexOf(culture);
			if (_imageIndex < 0 || _imageIndex >= layer.Images.Count)
				return;

			// Capture old state
			var image = layer.Images[_imageIndex];
			_oldFile = _isPopup ? image.Popup : image.File;
			_oldWidth = image.Width;
			_oldHeight = image.Height;

			// Backup old file if exists
			if (!string.IsNullOrEmpty(_oldFile))
			{
				var oldPath = Path.Combine(FileManagerUnity.TempFolder, FileManagerUnity.FolderLayers, _oldFile);
				if (File.Exists(oldPath))
				{
					var undoDir = Path.Combine(FileManagerUnity.TempFolder, ".undo");
					Directory.CreateDirectory(undoDir);
					_backupPath = Path.Combine(undoDir, Guid.NewGuid() + Path.GetExtension(_oldFile));
					File.Copy(oldPath, _backupPath);
				}
			}
		}

		public void Execute()
		{
			if (_imageIndex < 0 || _imageIndex >= _layer.Images.Count)
				return;

			_layer.SetImage(_culture, _newFilePath, _isPuzzle, _isPopup);
		}

		public void Undo()
		{
			if (_imageIndex < 0 || _imageIndex >= _layer.Images.Count)
				return;

			// Restore backup file
			if (_backupPath != null && File.Exists(_backupPath) && !string.IsNullOrEmpty(_oldFile))
			{
				var destPath = Path.Combine(FileManagerUnity.TempFolder, FileManagerUnity.FolderLayers, _oldFile);
				File.Copy(_backupPath, destPath, true);
			}

			// Restore model values
			var image = _layer.Images[_imageIndex];
			if (_isPopup)
			{
				image.Popup = _oldFile;
			}
			else
			{
				image.File = _oldFile;
				image.Width = _oldWidth;
				image.Height = _oldHeight;
			}
		}

		public void Dispose()
		{
			// Cleanup backup file
			if (_backupPath != null && File.Exists(_backupPath))
				File.Delete(_backupPath);
		}

		public bool CanMergeWith(IEditCommand other) => false;
		public IEditCommand MergeWith(IEditCommand other) => this;
	}
}
