using System;

namespace DLC.Scientific.Core.Geocoding.Gps.Ncom.Channels
{
	internal sealed class Channel6
		: Channel
	{
		public override int ChannelNumber { get { return 6; } }

		/// <summary>
		/// Gets or sets the age of gyro bias.
		/// byte 6
		/// </summary>
		/// <value>
		/// The age of gyro bias.
		/// </value>
		public int AgeOfGyroBias { get; set; }

		/// <summary>
		/// Gets or sets the gyro bias X.
		/// bytes 0-1
		/// </summary>
		/// <value>
		/// The gyro bias X.
		/// </value>
		public double GyroBiasX { get; set; }

		/// <summary>
		/// Gets or sets the gyro bias Y.
		/// bytes 2-3
		/// </summary>
		/// <value>
		/// The gyro bias Y.
		/// </value>
		public double GyroBiasY { get; set; }

		/// <summary>
		/// Gets or sets the gyro bias Z.
		/// bytes 4-5
		/// </summary>
		/// <value>
		/// The gyro bias Z.
		/// </value>
		public double GyroBiasZ { get; set; }
	}
}