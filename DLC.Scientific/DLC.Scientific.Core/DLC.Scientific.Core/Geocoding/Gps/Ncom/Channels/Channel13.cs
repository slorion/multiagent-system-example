using System;

namespace DLC.Scientific.Core.Geocoding.Gps.Ncom.Channels
{
	internal sealed class Channel13 
		: Channel
	{
		public override int ChannelNumber { get { return 13; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Channel13"/> class.
		/// </summary>
		public Channel13(NcomRawData data)
		{
			if (data == null) throw new ArgumentNullException("data");

			var bytes = data.Packet;

			// Heading orientation of the GPS antenna
			int heading = bytes[63] + (bytes[64] << 8);
			if (bytes[64] >= 128)
				heading = heading.TwosComplementFromInt16();
			double h = heading * Constants.Ang2Rad * Constants.Rad2Deg;
			if (h < 0)
				h = h + 360;

			// Pitch orientation of the GPS antenna
			int pitch = bytes[65] + (bytes[66] << 8);
			if (bytes[66] >= 128)
				pitch = pitch.TwosComplementFromInt16();
			double p = pitch * Constants.Ang2Rad * Constants.Rad2Deg;

			// Distance between the GPS antennas
			double distance = bytes[67] + (bytes[68] << 8);
			double d = distance;

			// Age
			int a = bytes[69];

			this.AgeOfOrientationOfGpsAntennas = a;
			this.DistanceBetweenGpsAntennas = d;
			this.PitchOrientationOfGpsAntennas = p;
			this.HeadingOrientationOfGpsAntennas = h;
		}

		/// <summary>
		/// Gets the age of orientation of GPS antennas.
		/// byte 6
		/// </summary>
		/// <value>
		/// The age of orientation of GPS antennas.
		/// </value>
		public int AgeOfOrientationOfGpsAntennas { get; set; }

		/// <summary>
		/// Gets the distance between GPS antennas.
		/// bytes 4-5
		/// </summary>
		/// <value>
		/// The distance between GPS antennas.
		/// </value>
		public double DistanceBetweenGpsAntennas { get; set; }

		/// <summary>
		/// Gets the heading orientation of GPS antennas.
		/// bytes 0-1
		/// </summary>
		/// <value>
		/// The heading orientation of GPS antennas.
		/// </value>
		public double HeadingOrientationOfGpsAntennas { get; set; }

		/// <summary>
		/// Gets the pitch orientation of GPS antennas.
		/// bytes 2-3
		/// </summary>
		/// <value>
		/// The pitch orientation of GPS antennas.
		/// </value>
		public double PitchOrientationOfGpsAntennas { get; set; }
	}
}