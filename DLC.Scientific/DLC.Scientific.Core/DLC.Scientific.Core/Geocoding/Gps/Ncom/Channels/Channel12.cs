using System;

namespace DLC.Scientific.Core.Geocoding.Gps.Ncom.Channels
{
	internal sealed class Channel12
		: Channel
	{
		public override int ChannelNumber { get { return 12; } }

		/// <summary>
		/// Gets or sets the age of distance to primary antenna.
		/// byte 6
		/// </summary>
		/// <value>
		/// The age of distance to primary antenna.
		/// </value>
		public int AgeOfDistanceToPrimaryAntenna { get; set; }

		/// <summary>
		/// Gets or sets the distance X to primary antenna.
		/// bytes 0-1
		/// </summary>
		/// <value>
		/// The distance X to primary antenna.
		/// </value>
		public double DistanceXToPrimaryAntenna { get; set; }

		/// <summary>
		/// Gets or sets the distance Y to primary antenna.
		/// bytes 2-3
		/// </summary>
		/// <value>
		/// The distance Y to primary antenna.
		/// </value>
		public double DistanceYToPrimaryAntenna { get; set; }

		/// <summary>
		/// Gets or sets the distance Z to primary antenna.
		/// bytes 4-5
		/// </summary>
		/// <value>
		/// The distance Z to primary antenna.
		/// </value>
		public double DistanceZToPrimaryAntenna { get; set; }
	}
}