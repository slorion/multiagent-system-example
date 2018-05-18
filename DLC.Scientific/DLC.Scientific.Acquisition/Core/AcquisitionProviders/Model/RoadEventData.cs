using DLC.Scientific.Core.Geocoding.Gps;
using System;
using System.Runtime.Serialization;

namespace DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model
{
	[DataContract]
	[Serializable]
	public class RoadEventData
		: ProviderData
	{
		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string EventType { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int Severity { get; set; }

		[DataMember]
		public bool IsSnapshot { get; set; }

		[DataMember]
		public bool IsJournalised { get; set; }

		[DataMember]
		public string Comment { get; set; }

		[DataMember]
		public double Distance { get; set; }

		[DataMember]
		public GeoData Localisation { get; set; }

		[DataMember]
		public double DistanceManualCorrection { get; set; }

		public override string ToString()
		{
			return string.Format("{0};{1}", base.ToString(), this.Id);
		}
	}
}