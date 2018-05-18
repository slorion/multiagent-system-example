using System;

namespace DLC.Scientific.Core.Geocoding.Gps.Ncom.Channels
{
	internal sealed class Channel16
		: Channel
	{
		public override int ChannelNumber { get { return 16; } }

		/// <summary>
		/// Gets or sets the vehicle angles time offset.
		/// </summary>
		/// <value>
		/// The vehicle angles time offset.
		/// </value>
		public int VehicleAnglesTimeOffset { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [vehicle angles valid].
		/// </summary>
		/// <value>
		///   <c>true</c> if [vehicle angles valid]; otherwise, <c>false</c>.
		/// </value>
		public bool VehicleAnglesValid { get; set; }

		/// <summary>
		/// Gets or sets the vehicle heading.
		/// </summary>
		/// <value>
		/// The vehicle heading.
		/// </value>
		public double VehicleHeading { get; set; }

		/// <summary>
		/// Gets or sets the vehicle pitch.
		/// </summary>
		/// <value>
		/// The vehicle pitch.
		/// </value>
		public double VehiclePitch { get; set; }

		/// <summary>
		/// Gets or sets the vehicle roll.
		/// </summary>
		/// <value>
		/// The vehicle roll.
		/// </value>
		public double VehicleRoll { get; set; }
	}
}