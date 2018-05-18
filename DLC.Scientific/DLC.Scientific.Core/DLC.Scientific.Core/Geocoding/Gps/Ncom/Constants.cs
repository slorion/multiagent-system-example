using System;

namespace DLC.Scientific.Core.Geocoding.Gps.Ncom
{
	internal static class Constants
	{
		public const double Ang2Rad = 0.000001;
		public const double Rad2Deg = 180 / Math.PI;

		public const double Acc2Mps2 = 0.0001;
		public const double Rate2Rps = 0.00001;
		public const double Vel2Mps = 0.0001;

		/// <summary>
		/// Units of wheelspeed scaling.
		/// </summary>
		public const double WheelSpeedScaling = 0.1;

		/// <summary>
		/// Units of wheelspeed scaling accuracy.
		/// </summary>
		public const double WheelSpeedScalingAccuracy = 0.002 / 100;
	}
}
