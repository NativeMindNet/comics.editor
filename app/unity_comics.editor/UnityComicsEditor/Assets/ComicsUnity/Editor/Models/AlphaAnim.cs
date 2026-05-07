namespace ComicsUnity.Models
{
	public class AlphaAnim : Anim
	{
		private double _alpha;

		public override AnimTypes Type => AnimTypes.Alpha;

		public double Alpha
		{
			get => _alpha;
			set
			{
				if (_alpha == value) return;
				_alpha = value;
				OnPropertyChanged(nameof(Alpha));
			}
		}

		public override Anim Interpolate(Anim current, double scroll)
		{
			var alpha = (AlphaAnim)current;
			return new AlphaAnim
			{
				Alpha = Alpha + (alpha.Alpha - Alpha) * alpha.Factor(scroll)
			};
		}

		protected override void Init()
		{
			base.Init();
			_alpha = 1;
		}
	}
}
