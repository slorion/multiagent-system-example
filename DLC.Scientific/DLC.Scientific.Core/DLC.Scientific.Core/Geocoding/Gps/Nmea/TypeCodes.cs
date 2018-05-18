namespace DLC.Scientific.Core.Geocoding.Gps.Nmea
{
	/// <summary>
	/// Internally supported NMEA type codes
	/// </summary>
	public static class TypeCodes
	{
		/// <summary>
		/// GPS Fix Data
		/// </summary>
		public const string GGA = "GGA";

		/// <summary>
		/// GPS DOP and active satellites
		/// </summary>
		public const string GSA = "GSA";

		/// <summary>
		/// GPS Pseudorange Noise Statistics
		/// </summary>
		public const string GST = "GST";

		/// <summary>
		/// Heading - True (obsolete as of 2009)
		/// </summary>
		public const string HDT = "HDT";

		/// <summary>
		/// Recommanded Minimum Navigation Information (C)
		/// </summary>
		public const string RMC = "RMC";

		/// <summary>
		/// Track made good and Ground speed
		/// </summary>
		public const string VTG = "VTG";

		/// <summary>
		/// Time & Date - UTC, day, month, year and local time zone
		/// </summary>
		public const string ZDA = "ZDA";

		/// <summary>
		/// Proprietary Heading, Pitch, Roll, Heave measurements (OxTS RT/Inertial+ family)
		/// </summary>
		public const string SHR = "SHR";
	}
}
