using System;

namespace DLC.Scientific.Core.Geocoding.Gps.Ncom.Channels
{
	internal sealed class Channel3
		: Channel
	{
		public override int ChannelNumber { get { return 3; } }

		/// <summary>
		/// Units of position accuracies are 1mm (channel 3)
		/// </summary>
		private const double Posacc = 0.001;

		/// <summary>
		/// Initializes a new instance of the <see cref="Channel3"/> class.
		/// </summary>
		public Channel3(NcomRawData data)
		{
			if (data == null) throw new ArgumentNullException("data");

			var bytes = data.Packet;

			this.NorthPositionAccuracy = (bytes[63] + (bytes[64] << 8)) * Posacc;
			this.EastPositionAccuracy = (bytes[65] + (bytes[66] << 8)) * Posacc;
			this.DownPositionAccurady = (bytes[67] + (bytes[68] << 8)) * Posacc;
			this.AgeOfPositionAccuracy = bytes[69];
		}

		/// <summary>
		/// Gets the age of position accuracy.
		/// byte 6
		/// </summary>
		/// <value>
		/// The age of position accuracy.
		/// </value>
		public int AgeOfPositionAccuracy { get; set; }

		/// <summary>
		/// Gets down position accurady.
		/// bytes 4-5
		/// </summary>
		/// <value>
		/// Down position accurady.
		/// </value>
		public double DownPositionAccurady { get; set; }

		/// <summary>
		/// Gets the east position accuracy.
		/// bytes 2-3
		/// </summary>
		/// <value>
		/// The east position accuracy.
		/// </value>
		public double EastPositionAccuracy { get; set; }

		/// <summary>
		/// Gets the north position accuracy.
		/// bytes 0-1
		/// </summary>
		/// <value>
		/// The north position accuracy.
		/// </value>
		public double NorthPositionAccuracy { get; set; }
	}
}