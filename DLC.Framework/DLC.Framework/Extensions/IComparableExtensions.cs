using System;

namespace DLC.Framework.Extensions
{
	public static class IComparableExtensions
	{
		public static bool Between<T>(this T source, T low, T high, bool inclusive = true)
			where T : IComparable
		{
			if (low.CompareTo(high) > 0)
			{
				T x = low;
				low = high;
				high = x;
			};

			return inclusive ?
				(source.CompareTo(low) >= 0 && source.CompareTo(high) <= 0)
				: (source.CompareTo(low) > 0 && source.CompareTo(high) < 0);
		}
	}
}