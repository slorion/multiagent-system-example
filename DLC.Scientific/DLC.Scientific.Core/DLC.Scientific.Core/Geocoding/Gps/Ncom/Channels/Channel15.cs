using System;

namespace DLC.Scientific.Core.Geocoding.Gps.Ncom.Channels
{
	internal sealed class Channel15
		: Channel
	{
		public override int ChannelNumber { get { return 15; } }

		/// <summary>
		/// Gets or sets the accuracy of distance between GPS antennas.
		/// bytes 4-5
		/// </summary>
		/// <value>
		/// The accuracy of distance between GPS antennas.
		/// </value>
		public double AccuracyOfDistanceBetweenGpsAntennas { get; set; }

		/// <summary>
		/// Gets or sets the accuracy of heading orientation of GPS antennas.
		/// bytes 0-1
		/// </summary>
		/// <value>
		/// The accuracy of heading orientation of GPS antennas.
		/// </value>
		public double AccuracyOfHeadingOrientationOfGpsAntennas { get; set; }

		/// <summary>
		/// Gets or sets the accuracy of pitch orientation of GPS antennas.
		/// bytes 2-3
		/// </summary>
		/// <value>
		/// The accuracy of pitch orientation of GPS antennas.
		/// </value>
		public double AccuracyOfPitchOrientationOfGpsAntennas { get; set; }

		/// <summary>
		/// Gets or sets the age of accuracy of orientation of GPS antennas.
		/// byte 6
		/// </summary>
		/// <value>
		/// The age of accuracy of orientation of GPS antennas.
		/// </value>
		public int AgeOfAccuracyOfOrientationOfGpsAntennas { get; set; }
	}
}