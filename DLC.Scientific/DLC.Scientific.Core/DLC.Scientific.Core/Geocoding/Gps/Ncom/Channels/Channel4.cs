using System;

namespace DLC.Scientific.Core.Geocoding.Gps.Ncom.Channels
{
	internal sealed class Channel4
		: Channel
	{
		public override int ChannelNumber { get { return 4; } }

		/// <summary>
		/// Units of velocity accuracies are 1mm/s (channel 4)
		/// </summary>
		private const double Velacc = 0.001;

		/// <summary>
		/// Initializes a new instance of the <see cref="Channel4"/> class.
		/// </summary>
		public Channel4(NcomRawData data)
		{
			if (data == null) throw new ArgumentNullException("data");

			var bytes = data.Packet;

			this.NorthVelocityAccuracy = (bytes[63] + (bytes[64] << 8)) * Velacc;
			this.EastVelocityAccuracy = (bytes[65] + (bytes[66] << 8)) * Velacc;
			this.DownVelocityAccuracy = (bytes[67] + (bytes[68] << 8)) * Velacc;
			this.AgeOfVelocityAccuracy = bytes[69];
		}

		/// <summary>
		/// Gets the age of velocity accuracy.
		/// byte 6
		/// </summary>
		/// <value>
		/// The age of velocity accuracy.
		/// </value>
		public int AgeOfVelocityAccuracy { get; set; }

		/// <summary>
		/// Gets down velocity accuracy.
		/// bytes 4-5
		/// </summary>
		/// <value>
		/// Down velocity accuracy.
		/// </value>
		public double DownVelocityAccuracy { get; set; }

		/// <summary>
		/// Gets the east velocity accuracy.
		/// bytes 2-3
		/// </summary>
		/// <value>
		/// The east velocity accuracy.
		/// </value>
		public double EastVelocityAccuracy { get; set; }

		/// <summary>
		/// Gets the north velocity accuracy.
		/// bytes 0-1
		/// </summary>
		/// <value>
		/// The north velocity accuracy.
		/// </value>
		public double NorthVelocityAccuracy { get; set; }
	}
}