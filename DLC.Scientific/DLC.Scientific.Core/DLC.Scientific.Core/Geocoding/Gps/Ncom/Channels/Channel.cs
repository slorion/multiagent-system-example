using System;

namespace DLC.Scientific.Core.Geocoding.Gps.Ncom.Channels
{
	public abstract class Channel
	{
		public static readonly Channel Empty = new EmptyChannel();

		public abstract int ChannelNumber { get; }
	}
}