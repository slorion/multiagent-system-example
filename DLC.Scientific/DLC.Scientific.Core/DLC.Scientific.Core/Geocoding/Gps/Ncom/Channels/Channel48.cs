using System;

namespace DLC.Scientific.Core.Geocoding.Gps.Ncom.Channels
{
	internal sealed class Channel48
		: Channel
	{
		public override int ChannelNumber { get { return 48; } }

		/// <summary>
		/// Units of HDOP and PDOP (channel 48)
		/// </summary>
		private const double DopUnits = 0.1;

		private const double UndulationUnits = 0.005;

		/// <summary>
		/// Initializes a new instance of the <see cref="Channel48"/> class.
		/// </summary>
		public Channel48(NcomRawData data)
		{
			if (data == null) throw new ArgumentNullException("data");

			var bytes = data.Packet;

			int undulation = bytes[63] + (bytes[64] << 8);

			if (bytes[64] >= 128)
				undulation = undulation.TwosComplementFromInt16();

			this.Undulation = undulation * UndulationUnits;
			this.Hdop = bytes[65] * DopUnits;
			this.Pdop = bytes[66] * DopUnits;
		}

		/// <summary>
		/// Gets the hdop.
		/// </summary>
		/// <value>
		/// The hdop.
		/// </value>
		public double Hdop { get; set; }

		/// <summary>
		/// Gets the pdop.
		/// </summary>
		/// <value>
		/// The pdop.
		/// </value>
		public double Pdop { get; set; }

		/// <summary>
		/// Gets the vdop.
		/// </summary>
		/// <value>
		/// The vdop.
		/// </value>
		public double Vdop
		{
			get { return Math.Sqrt(Math.Pow(Pdop, 2) - Math.Pow(Hdop, 2)); }
		}


		/// <summary>
		/// Gets the undulation.
		/// </summary>
		/// <value>
		/// The undulation.
		/// </value>
		public double Undulation { get; set; }
	}
}