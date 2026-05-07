using System.Collections.Generic;
using System.Linq;

namespace ComicsUnity.Models
{
	public enum Cultures
	{
		En,
		Ru,
		Hi
	}

	public static class CulturesHelper
	{
		public static readonly List<Cultures> All = System.Enum.GetValues(typeof(Cultures)).Cast<Cultures>().ToList();
	}
}
