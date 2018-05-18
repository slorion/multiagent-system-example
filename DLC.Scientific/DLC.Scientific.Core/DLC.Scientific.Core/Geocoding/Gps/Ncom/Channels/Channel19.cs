using System;

namespace DLC.Scientific.Core.Geocoding.Gps.Ncom.Channels
{
	internal sealed class Channel19
		: Channel
	{
		public override int ChannelNumber { get { return 19; } }

		/// <summary>
		/// Gets or sets the software version dev id.
		/// </summary>
		/// <value>
		/// The software version dev id.
		/// </value>
		public string SoftwareVersionDevId { get; set; }
	}
}