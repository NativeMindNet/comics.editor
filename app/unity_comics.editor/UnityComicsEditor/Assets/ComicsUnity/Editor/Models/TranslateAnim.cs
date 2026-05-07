using System;

namespace ComicsUnity.Models
{
	public class TranslateAnim : Anim
	{
		private int _x;
		private int _y;

		public override AnimTypes Type => AnimTypes.Translate;

		public int X
		{
			get => _x;
			set
			{
				if (_x == value) return;
				_x = value;
				OnPropertyChanged(nameof(X));
			}
		}

		public int Y
		{
			get => _y;
			set
			{
				if (_y == value) return;
				_y = value;
				OnPropertyChanged(nameof(Y));
			}
		}

		public override Anim Interpolate(Anim current, double scroll)
		{
			var translate = (TranslateAnim)current;
			return new TranslateAnim
			{
				X = (int)Math.Round(X + (translate.X - X) * translate.Factor(scroll)),
				Y = (int)Math.Round(Y + (translate.Y - Y) * translate.Factor(scroll))
			};
		}
	}
}
