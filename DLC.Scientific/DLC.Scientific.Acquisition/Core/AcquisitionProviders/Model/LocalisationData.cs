using DLC.Scientific.Core.Geocoding.Gps;
using System;
using System.Runtime.Serialization;

namespace DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model
{
	[DataContract]
	[Serializable]
	public class LocalisationData
		: ProviderData
	{
		private GeoData _corrected;

		[DataMember]
		public GeoData RawData { get; set; }

		[DataMember]
		public GeoData CorrectedData
		{
			get { return _corrected ?? this.RawData; }
			set { _corrected = value; }
		}

		[DataMember]
		public GpsStatus GpsStatus { get; set; }

		public override string ToString()
		{
			string latlon;
			if (this.RawData == null || this.RawData.PositionData == null)
				latlon = "null";
			else
				latlon = string.Format("({0},{1})", this.RawData.PositionData.Latitude, this.RawData.PositionData.Longitude);

			return string.Format("{0};{1}", base.ToString(), latlon);
		}
	}
}