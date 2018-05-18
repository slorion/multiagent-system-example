using System;

namespace DLC.Scientific.Core.Geocoding.Gps.Ncom.Channels
{
	/// <summary>
	/// Full time, Number of satellites, Position mode, Velocity mode, Dual antenna mode
	/// </summary>
	internal sealed class Channel0 
		: Channel
	{
		public override int ChannelNumber { get { return 0; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Channel0"/> class.
		/// </summary>
		public Channel0(NcomRawData data)
		{
			if (data == null) throw new ArgumentNullException("data");

			var bytes = data.Packet;

			this.Minutes = bytes[63] + (bytes[64] << 8) + (bytes[65] << 16) + (bytes[66] << 24);
			this.NumberOfSatellites = bytes[67];
			this.PositionMode = bytes[68];
			this.VelocityMode = bytes[69];
			this.OrientationMode = bytes[70];
		}

		/// <summary>
		/// Gets the time in minutes since GPS began (midnight 06/01/1980).
		/// bytes 0-3
		/// </summary>
		/// <value>
		/// The minutes.
		/// </value>
		public long Minutes { get; set; }

		/// <summary>
		/// Gets the number of satellites.
		/// byte 4
		/// </summary>
		/// <value>
		/// The number of satellites.
		/// </value>
		public int NumberOfSatellites { get; set; }

		/// <summary>
		/// Gets the orientation mode of dual antenna systems.
		/// byte 7
		/// </summary>
		/// <value>
		/// The orientation mode.
		/// </value>
		public int OrientationMode { get; set; }

		/// <summary>
		/// Gets the position mode of primary GPS.
		/// byte 5
		/// </summary>
		/// <value>
		/// The position mode.
		/// </value>
		public int PositionMode { get; private set; }

		/// <summary>
		/// Gets the velocity mode of primary GPS.
		/// byte 6
		/// </summary>
		/// <value>
		/// The velocity mode.
		/// </value>
		public int VelocityMode { get; set; }
	}
}