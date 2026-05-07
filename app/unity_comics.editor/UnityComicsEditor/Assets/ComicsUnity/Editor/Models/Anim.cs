using System;
using System.Collections.Generic;
using System.Linq;

using ComicsUnity;

namespace ComicsUnity.Models
{
	public enum AnimTypes
	{
		Translate,
		Rotate,
		Scale,
		Alpha,
		Sound
	}

	public abstract class Anim : NotifyPropertyChanged
	{
		private int _start;
		private int _end;

		public int Start
		{
			get => _start;
			set
			{
				if (_start == value) return;
				_start = value;
				OnPropertyChanged(nameof(Start));
			}
		}

		public int End
		{
			get => _end;
			set
			{
				if (_end == value) return;
				_end = value;
				OnPropertyChanged(nameof(End));
			}
		}

		public abstract AnimTypes Type { get; }

		public abstract Anim Interpolate(Anim current, double scroll);

		protected virtual void Init() { }

		public double Factor(double scroll)
		{
			var t = (scroll - Start) / (End - Start);
			return (--t) * t * t + 1;
		}

		public override string ToString() => Type.GetEnumName();

		private static (T prev, T curr) FindNearest<T>(IList<Anim> anims, double scroll) where T : Anim, new()
		{
			T prev = null;
			T curr = null;
			foreach (var anim in anims.OfType<T>().OrderBy(x => x.Start))
			{
				if (anim.End <= scroll)
					prev = anim;
				else
				{
					if (anim.Start < scroll)
						curr = anim;
					break;
				}
			}

			if (prev == null)
			{
				prev = new T();
				prev.Init();
			}

			return (prev, curr);
		}

		public static T Interpolate<T>(IList<Anim> anims, Anim selected, double scroll) where T : Anim, new()
		{
			if (selected is T selectedT)
				return selectedT;

			var pair = FindNearest<T>(anims, scroll);
			return pair.curr != null ? (T)pair.prev.Interpolate(pair.curr, scroll) : pair.prev;
		}

		public static T Add<T>(IList<Anim> anims, double scroll) where T : Anim, new()
		{
			var anim = (T)FindNearest<T>(anims, double.MaxValue).prev.MemberwiseClone();
			anim.Start = scroll > anim.End ? (int)scroll : anim.End + 1;
			anim.End = anim.Start + 200;
			anims.Add(anim);
			return anim;
		}

		public static Anim Add(IList<Anim> anims, AnimTypes type, double scroll)
		{
			switch (type)
			{
				case AnimTypes.Translate: return Add<TranslateAnim>(anims, scroll);
				case AnimTypes.Rotate: return Add<RotateAnim>(anims, scroll);
				case AnimTypes.Scale: return Add<ScaleAnim>(anims, scroll);
				case AnimTypes.Alpha: return Add<AlphaAnim>(anims, scroll);
				default: return null;
			}
		}
	}
}
