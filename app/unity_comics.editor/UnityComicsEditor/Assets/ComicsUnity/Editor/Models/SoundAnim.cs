using System.Collections.Generic;
using System.Linq;

namespace ComicsUnity.Models
{
	public class SoundAnim : Anim
	{
		public override AnimTypes Type => AnimTypes.Sound;

		public override Anim Interpolate(Anim current, double scroll) => current;

		public static SoundAnim FindCurrent(IList<Anim> anims, double prevScroll, double scroll) =>
			anims.OfType<SoundAnim>().FirstOrDefault(x =>
				x.Start <= scroll && x.End >= scroll ||
				x.Start == x.End && prevScroll < scroll && prevScroll <= x.Start && x.Start <= scroll);
	}
}
