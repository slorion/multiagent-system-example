using System;

namespace DLC.Scientific.Core.Geocoding.Gps.Ncom.Channels
{
	internal sealed class Channel20
		: Channel
	{
		public override int ChannelNumber { get { return 20; } }

		/// <summary>
		/// Units of differential corrections (channel 20)
		/// </summary>
		private const double DiffUnits = 0.01;

		/// <summary>
		/// Initializes a new instance of the <see cref="Channel20"/> class.
		/// </summary>
		public Channel20(NcomRawData data)
		{
			if (data == null) throw new ArgumentNullException("data");

			var bytes = data.Packet;

			this.AgeDiffRefStationUpdate = (bytes[63] + (bytes[64] << 8)) * DiffUnits;
			this.DifferentialStationId = bytes[65] + (bytes[66] << 8) + (bytes[67] << 16) + (bytes[68] << 24);
		}

		/// <summary>
		/// Gets the age diff ref station update.
		/// bytes 0-1
		/// </summary>
		/// <value>
		/// The age diff ref station update.
		/// </value>
		public double AgeDiffRefStationUpdate { get; set; }

		/// <summary>
		/// Gets the differential station id.
		/// bytes 2-5
		/// </summary>
		/// <value>
		/// The differential station id.
		/// </value>
		public long DifferentialStationId { get; set; }
	}
}