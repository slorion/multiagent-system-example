using System;

namespace DLC.Scientific.Core.Geocoding.Gps.Ncom.Channels
{
	internal sealed class Channel8
		: Channel
	{
		public override int ChannelNumber { get { return 8; } }

		/// <summary>
		/// Gets or sets the age ofgyro scale factor.
		/// byte 6
		/// </summary>
		/// <value>
		/// The age ofgyro scale factor.
		/// </value>
		public int AgeOfgyroScaleFactor { get; set; }

		/// <summary>
		/// Gets or sets the gyro scale factor X.
		/// bytes 0-1
		/// </summary>
		/// <value>
		/// The gyro scale factor X.
		/// </value>
		public double GyroScaleFactorX { get; set; }

		/// <summary>
		/// Gets or sets the gyro scale factor Y.
		/// bytes 2-3
		/// </summary>
		/// <value>
		/// The gyro scale factor Y.
		/// </value>
		public double GyroScaleFactorY { get; set; }

		/// <summary>
		/// Gets or sets the gyro scale factor Z.
		/// bytes 4-5
		/// </summary>
		/// <value>
		/// The gyro scale factor Z.
		/// </value>
		public double GyroScaleFactorZ { get; set; }
	}
}