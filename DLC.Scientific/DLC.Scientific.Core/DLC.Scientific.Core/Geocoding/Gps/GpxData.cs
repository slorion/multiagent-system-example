namespace DLC.Scientific.Core.Geocoding.Gps
{
	public class GpxData
		: GeoData
	{
		public GpxData()
			: base()
		{
		}

		public GpxData(PositionData positionData, VelocityData velocityData, PrecisionData precisionData)
		{
			this.PositionData = positionData;
			this.PrecisionData = precisionData;
			this.VelocityData = velocityData;
		}

		public ExtensionData ExtensionData { get; set; }
	}
}
