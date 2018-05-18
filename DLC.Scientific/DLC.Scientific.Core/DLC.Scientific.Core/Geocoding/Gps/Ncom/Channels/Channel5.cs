using System;

namespace DLC.Scientific.Core.Geocoding.Gps.Ncom.Channels
{
	internal sealed class Channel5
		: Channel
	{
		public override int ChannelNumber { get { return 5; } }

		private const double HprAccUnits = 0.00001;

		/// <summary>
		/// Initializes a new instance of the <see cref="Channel5"/> class.
		/// </summary>
		public Channel5(NcomRawData data)
		{
			if (data == null) throw new ArgumentNullException("data");

			var bytes = data.Packet;

			// Heading accuracy
			int headingAccuracy = bytes[63] + (bytes[64] << 8);
			if (bytes[64] >= 128)
				headingAccuracy = headingAccuracy.TwosComplementFromInt16();
			double h = headingAccuracy * HprAccUnits * Constants.Rad2Deg;
			if (h < 0)
				h = h + 360;

			// Pitch accuracy
			int pitchAccuracy = bytes[65] + (bytes[66] << 8);
			if (bytes[66] >= 128)
				pitchAccuracy = pitchAccuracy.TwosComplementFromInt16();
			double p = pitchAccuracy * HprAccUnits * Constants.Rad2Deg;

			// Roll accuracy
			int rollAccuracy = bytes[67] + (bytes[68] << 8);

			if (bytes[68] >= 128)
				rollAccuracy = rollAccuracy.TwosComplementFromInt16();
			double r = rollAccuracy * HprAccUnits * Constants.Rad2Deg;

			// Age
			int a = bytes[69];

			this.AgeOfHprAccuracy = a;
			this.HeadingAccuracy = h;
			this.PitchAccuracy = p;
			this.RollAccuracy = r;
		}

		/// <summary>
		/// Gets the age of HPR accuracy.
		/// byte 6
		/// </summary>
		/// <value>
		/// The age of HPR accuracy.
		/// </value>
		public int AgeOfHprAccuracy { get; set; }

		/// <summary>
		/// Gets the heading accuracy.
		/// bytes 0-1
		/// </summary>
		/// <value>
		/// The heading accuracy.
		/// </value>
		public double HeadingAccuracy { get; set; }

		/// <summary>
		/// Gets the pitch accuracy.
		/// bytes 2-3
		/// </summary>
		/// <value>
		/// The pitch accuracy.
		/// </value>
		public double PitchAccuracy { get; set; }

		/// <summary>
		/// Gets the roll accuracy.
		/// bytes 4-5
		/// </summary>
		/// <value>
		/// The roll accuracy.
		/// </value>
		public double RollAccuracy { get; set; }
	}
}