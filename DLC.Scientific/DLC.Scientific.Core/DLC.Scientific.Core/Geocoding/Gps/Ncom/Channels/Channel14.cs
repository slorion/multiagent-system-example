using System;

namespace DLC.Scientific.Core.Geocoding.Gps.Ncom.Channels
{
	internal sealed class Channel14
		: Channel
	{
		public override int ChannelNumber { get { return 14; } }

		private const double Distacc = 0.001;

		/// <summary>
		/// Initializes a new instance of the <see cref="Channel14"/> class.
		/// </summary>
		public Channel14(NcomRawData data)
		{
			if (data == null) throw new ArgumentNullException("data");

			var bytes = data.Packet;

			this.XDistanceAccuracy = (bytes[63] + (bytes[64] << 8)) * Distacc;
			this.YDistanceAccuracy = (bytes[65] + (bytes[66] << 8)) * Distacc;
			this.ZDistanceAccuracy = (bytes[67] + (bytes[68] << 8)) * Distacc;
			this.AgeOfDistanceAccuracy = bytes[69];
		}

		/// <summary>
		/// Gets the age of distance accuracy.
		/// byte 6
		/// </summary>
		/// <value>
		/// The age of distance accuracy.
		/// </value>
		public int AgeOfDistanceAccuracy { get; set; }

		/// <summary>
		/// Gets the X distance accuracy.
		/// Accuracy of distance to primary GPS antenna in x-direction
		/// bytes 0-1
		/// </summary>
		/// <value>
		/// The X distance accuracy.
		/// </value>
		public double XDistanceAccuracy { get; set; }

		/// <summary>
		/// Gets the Y distance accuracy.
		/// Accuracy of distance to primary GPS antenna in y-direction
		/// bytes 2-3
		/// </summary>
		/// <value>
		/// The Y distance accuracy.
		/// </value>
		public double YDistanceAccuracy { get; set; }

		/// <summary>
		/// Gets the Z distance accuracy.
		/// Accuracy of distance to primary GPS antenna in z-direction
		/// bytes 4-5
		/// </summary>
		/// <value>
		/// The Z distance accuracy.
		/// </value>
		public double ZDistanceAccuracy { get; set; }
	}
}