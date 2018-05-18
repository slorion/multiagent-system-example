using System;

namespace DLC.Scientific.Core.Geocoding.Gps.Ncom.Channels
{
	internal sealed class Channel44
		: Channel
	{
		public override int ChannelNumber { get { return 44; } }

		/// <summary>
		/// Gets or sets the wheelspeed scaling.
		/// bytes 0-1
		/// </summary>
		/// <value>
		/// The wheelspeed scaling.
		/// </value>
		public double WheelspeedScaling { get; set; }

		/// <summary>
		/// Gets or sets the wheelspeed scaling accuracy.
		/// bytes 2-3
		/// </summary>
		/// <value>
		/// The wheelspeed scaling accuracy.
		/// </value>
		public double WheelspeedScalingAccuracy { get; set; }
	}
}