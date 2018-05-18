namespace DLC.Scientific.Core.Geocoding.Gps
{
	/// <summary>
	/// Helps convert Framework object to compatible GPX format values
	/// </summary>
	public static class GpxConvert
	{
		/// <summary>
		/// Froms the type of the fix.
		/// </summary>
		/// <param name="fix">The fix.</param>
		/// <returns>A string representation of a Fix Type</returns>
		public static string FromFixType(FixType fix)
		{
			switch (fix)
			{
				case FixType.Fix:
					return "3d";
				case FixType.Diff:
				case FixType.RTKfixed:
				case FixType.RTKfloating:
				case FixType.WAAS:
				case FixType.PostProcess:
					return "dgps";
				default:
					return "none";
			}
		}

		/// <summary>
		/// Toes the type of the fix.
		/// </summary>
		/// <param name="fix">The fix.</param>
		/// <returns>A Fix Type</returns>
		public static FixType ToFixType(string fix)
		{
			switch (fix)
			{
				case "3d":
					return FixType.Fix;
				case "dgps":
					return FixType.Diff;
				default:
					return FixType.None;
			}
		}
	}
}