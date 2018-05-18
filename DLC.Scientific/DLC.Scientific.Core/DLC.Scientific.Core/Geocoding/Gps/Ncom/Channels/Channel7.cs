using System;

namespace DLC.Scientific.Core.Geocoding.Gps.Ncom.Channels
{
	internal sealed class Channel7
		: Channel
	{
		public override int ChannelNumber { get { return 7; } }

		/// <summary>
		/// Gets or sets the acceleration bias X.
		/// bytes 0-1
		/// </summary>
		/// <value>
		/// The acceleration bias X.
		/// </value>
		public double AccelerationBiasX { get; set; }

		/// <summary>
		/// Gets or sets the acceleration bias Y.
		/// bytes 2-3
		/// </summary>
		/// <value>
		/// The acceleration bias Y.
		/// </value>
		public double AccelerationBiasY { get; set; }

		/// <summary>
		/// Gets or sets the acceleration bias Z.
		/// bytes 4-5
		/// </summary>
		/// <value>
		/// The acceleration bias Z.
		/// </value>
		public double AccelerationBiasZ { get; set; }

		/// <summary>
		/// Gets or sets the age ofacceleration bias.
		/// byte 6
		/// </summary>
		/// <value>
		/// The age ofacceleration bias.
		/// </value>
		public int AgeOfaccelerationBias { get; set; }
	}
}