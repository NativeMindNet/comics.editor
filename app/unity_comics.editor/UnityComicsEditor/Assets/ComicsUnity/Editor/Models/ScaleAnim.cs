namespace ComicsUnity.Models
{
	public class ScaleAnim : PivotAnim
	{
		private double _scaleX;
		private double _scaleY;

		public override AnimTypes Type => AnimTypes.Scale;

		public double ScaleX
		{
			get => _scaleX;
			set
			{
				if (_scaleX == value) return;
				_scaleX = value;
				OnPropertyChanged(nameof(ScaleX));
			}
		}

		public double ScaleY
		{
			get => _scaleY;
			set
			{
				if (_scaleY == value) return;
				_scaleY = value;
				OnPropertyChanged(nameof(ScaleY));
			}
		}

		public override Anim Interpolate(Anim current, double scroll)
		{
			var scale = (ScaleAnim)current;
			return new ScaleAnim
			{
				ScaleX = ScaleX + (scale.ScaleX - ScaleX) * scale.Factor(scroll),
				ScaleY = ScaleY + (scale.ScaleY - ScaleY) * scale.Factor(scroll),
				PivotX = scale.PivotX,
				PivotY = scale.PivotY
			};
		}

		protected override void Init()
		{
			base.Init();
			_scaleX = 1;
			_scaleY = 1;
		}
	}
}
