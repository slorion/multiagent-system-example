using System;

namespace DLC.Scientific.Core.Geocoding.Gps.Ncom.Channels
{
	internal sealed class Channel45
		: Channel
	{
		public override int ChannelNumber { get { return 45; } }

		/// <summary>
		/// Gets or sets the wheelspeed input count.
		/// bytes 0-1
		/// </summary>
		/// <value>
		/// The wheelspeed input count.
		/// </value>
		public ulong WheelspeedInputCount { get; set; }

		/// <summary>
		/// Gets or sets the wheelspeed timestamp.
		/// bytes 4-5
		/// </summary>
		/// <value>
		/// The wheelspeed timestamp.
		/// </value>
		public DateTime WheelspeedTimestamp { get; set; }

		/// <summary>
		/// Gets or sets the wheelspeed update.
		/// byte 6
		/// </summary>
		/// <value>
		/// The wheelspeed update.
		/// </value>
		public ushort WheelspeedUpdate { get; set; }
	}
}