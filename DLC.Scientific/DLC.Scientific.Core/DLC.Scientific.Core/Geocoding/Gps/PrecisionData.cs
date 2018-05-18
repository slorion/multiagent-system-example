using System;
using System.Runtime.Serialization;

namespace DLC.Scientific.Core.Geocoding.Gps
{
	[DataContract]
	[Serializable]
	public class PrecisionData
	{
		[DataMember]
		public double? Pdop { get; set; }
		[DataMember]
		public double? Hdop { get; set; }
		[DataMember]
		public double? Vdop { get; set; }

		[DataMember]
		public double? DownPositionAccuracy { get; set; }
		[DataMember]
		public double? EastPositionAccuracy { get; set; }
		[DataMember]
		public double? NorthPositionAccuracy { get; set; }

		[DataMember]
		public double? DownVelocityAccuracy { get; set; }
		[DataMember]
		public double? EastVelocityAccuracy { get; set; }
		[DataMember]
		public double? NorthVelocityAccuracy { get; set; }

		[DataMember]
		public double? HeadingAccuracy { get; set; }
		[DataMember]
		public double? PitchAccuracy { get; set; }
		[DataMember]
		public double? RollAccuracy { get; set; }

		public PrecisionData()
		{
			Pdop = null;
			Hdop = null;
			Vdop = null;

			DownPositionAccuracy = null;
			EastPositionAccuracy = null;
			NorthPositionAccuracy = null;

			DownVelocityAccuracy = null;
			EastVelocityAccuracy = null;
			NorthVelocityAccuracy = null;

			HeadingAccuracy = null;
			PitchAccuracy = null;
			RollAccuracy = null;
		}
	}
}