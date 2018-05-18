using DLC.Scientific.Core.Geocoding;
using DLC.Scientific.Core.Geocoding.Bgr;
using System;
using System.Runtime.Serialization;

namespace DLC.Scientific.Acquisition.Core.Agents.Model
{
	[DataContract]
	[Serializable]
	public abstract class TriggeredAcquisitionParameter
		: AcquisitionParameter
	{
		[DataMember]
		public AcquisitionTriggerMode TriggerMode { get; set; }

		[DataMember]
		public DirectionBgr DirectionBgr { get; set; }

		[DataMember]
		public IRtssc Rtssc { get; set; }

		[DataMember]
		public int? ProximityRange { get; set; }

		[DataMember]
		public double? Distance { get; set; }

		[DataMember]
		public GeoCoordinate GeoCoordinate { get; set; }
	}
}