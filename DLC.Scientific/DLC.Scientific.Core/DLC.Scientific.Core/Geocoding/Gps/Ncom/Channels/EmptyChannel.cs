using System;

namespace DLC.Scientific.Core.Geocoding.Gps.Ncom.Channels
{
	internal sealed class EmptyChannel
		: Channel
	{
		public override int ChannelNumber { get { return -1; } }
	}
}
