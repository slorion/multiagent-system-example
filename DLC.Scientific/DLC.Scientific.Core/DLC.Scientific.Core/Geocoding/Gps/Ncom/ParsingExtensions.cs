using System;

namespace DLC.Scientific.Core.Geocoding.Gps.Ncom
{
	internal static class ParsingExtensions
	{
		/// <summary>
		/// Twoses the complement from int16.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The int16</returns>
		public static int TwosComplementFromInt16(this int value)
		{
			int result = value + (255 << 16);
			return -(~result + 1);
		}

		/// <summary>
		/// Twoses the complement from int24.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The int24</returns>
		public static int TwosComplementFromInt24(this int value)
		{
			int result = value + (255 << 24);
			return -(~result + 1);
		}

		/// <summary>
		/// Converts a 4-bytes array into a single-precision floating point number
		/// </summary>
		/// <param name="byteArray4">The byte array4.</param>
		/// <returns>The float</returns>
		public static float ToSingle(this byte[] byteArray4)
		{
			if (byteArray4 == null) throw new ArgumentNullException("byteArray4");

			if (BitConverter.IsLittleEndian)
				Array.Reverse(byteArray4);

			return BitConverter.ToSingle(byteArray4, 0);
		}

		/// <summary>
		/// Converts a 8-bytes array into a double-precision floating point number
		/// </summary>
		/// <param name="byteArray8">The byte array8.</param>
		/// <returns>The double</returns>
		public static double ToDouble(this byte[] byteArray8)
		{
			if (byteArray8 == null) throw new ArgumentNullException("byteArray8");

			if (BitConverter.IsLittleEndian)
				Array.Reverse(byteArray8);

			return BitConverter.ToDouble(byteArray8, 0);
		}
	}
}