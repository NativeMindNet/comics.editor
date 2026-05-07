using Newtonsoft.Json;
using UnityEngine;

namespace ComicsUnity.Models
{
	public abstract class PivotAnim : Anim
	{
		private double _pivotX;
		private double _pivotY;

		public double PivotX
		{
			get => _pivotX;
			set
			{
				if (_pivotX == value) return;
				_pivotX = value;
				OnPropertyChanged(nameof(PivotX));
				OnPropertyChanged(nameof(Pivot));
			}
		}

		public double PivotY
		{
			get => _pivotY;
			set
			{
				if (_pivotY == value) return;
				_pivotY = value;
				OnPropertyChanged(nameof(PivotY));
				OnPropertyChanged(nameof(Pivot));
			}
		}

		[JsonIgnore]
		public Vector2 Pivot => new Vector2((float)PivotX, (float)PivotY);

		protected override void Init()
		{
			base.Init();
			_pivotX = 0.5;
			_pivotY = 0.5;
		}
	}
}
