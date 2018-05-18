using System;

namespace DLC.Scientific.Core.Geocoding.Gps.Ncom.Channels
{
	internal sealed class Channel1 
		: Channel
	{
		public override int ChannelNumber { get { return 1; } }

		/// <summary>
		/// Gets or sets the orientation heading innovation.
		/// byte 7
		/// </summary>
		/// <value>
		/// The orientation heading innovation.
		/// </value>
		public int OrientationHeadingInnovation { get; set; }

		/// <summary>
		/// Gets or sets the orientation pitch innovation.
		/// byte 6
		/// </summary>
		/// <value>
		/// The orientation pitch innovation.
		/// </value>
		public int OrientationPitchInnovation { get; set; }

		/// <summary>
		/// Gets or sets the position X innovation.
		/// byte 0
		/// </summary>
		/// <value>
		/// The position X innovation.
		/// </value>
		public int PositionXInnovation { get; set; }

		/// <summary>
		/// Gets or sets the position Y innovation.
		/// byte 1
		/// </summary>
		/// <value>
		/// The position Y innovation.
		/// </value>
		public int PositionYInnovation { get; set; }

		/// <summary>
		/// Gets or sets the position Z innovation.
		/// byte 2
		/// </summary>
		/// <value>
		/// The position Z innovation.
		/// </value>
		public int PositionZInnovation { get; set; }

		/// <summary>
		/// Gets or sets the velocity X innovation.
		/// byte 3
		/// </summary>
		/// <value>
		/// The velocity X innovation.
		/// </value>
		public int VelocityXInnovation { get; set; }

		/// <summary>
		/// Gets or sets the velocity Y innovation.
		/// byte 4
		/// </summary>
		/// <value>
		/// The velocity Y innovation.
		/// </value>
		public int VelocityYInnovation { get; set; }

		/// <summary>
		/// Gets or sets the velocity Z innovation.
		/// byte 5
		/// </summary>
		/// <value>
		/// The velocity Z innovation.
		/// </value>
		public int VelocityZInnovation { get; set; }
	}
}