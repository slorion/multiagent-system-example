using DLC.Scientific.Core.Geocoding.Gps;
using System;
using System.Runtime.Serialization;

namespace DLC.Scientific.Core.Geocoding.Gps
{
	[DataContract]
	[Serializable]
	public class GeoData
	{
		public GeoData()
		{
			this.DeviceType = GpsDeviceType.Unknown;
			this.PositionData = new PositionData();
			this.PrecisionData = new PrecisionData();
			this.VelocityData = new VelocityData();
		}

		[DataMember]
		public GpsDeviceType DeviceType { get; set; }

		[DataMember]
		public PositionData PositionData { get; set; }

		[DataMember]
		public PrecisionData PrecisionData { get; set; }

		[DataMember]
		public VelocityData VelocityData { get; set; }
	}
}