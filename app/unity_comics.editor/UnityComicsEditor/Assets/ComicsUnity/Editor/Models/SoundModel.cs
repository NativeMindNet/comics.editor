using System.Collections.Generic;

namespace ComicsUnity.Models
{
	public class SoundModel : NotifyPropertyChanged
	{
		private string _file;

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

		public List<Anim> Animations { get; set; } = new List<Anim>();

		public void Delete()
		{
			FileManagerUnity.Delete(FileManagerUnity.FolderSounds, File);
		}

		public static SoundModel Create(string file, double scroll)
		{
			var sound = new SoundModel
			{
				File = FileManagerUnity.Update(FileManagerUnity.FolderSounds, null, file)
			};
			sound.Animations.Add(new SoundAnim { Start = (int)scroll, End = (int)scroll });
			return sound;
		}
	}
}
