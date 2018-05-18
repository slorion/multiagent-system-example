using DLC.Scientific.Acquisition.Core.Configuration;
using System;
using System.Runtime.Serialization;

namespace DLC.Scientific.Acquisition.Core.Agents.Model.PhotoSettings
{
	[DataContract]
	[Serializable]
	public class CameraInfo
	{
		[DataMember]
		public AlignmentFile AlignmentFile { get; set; }

		[DataMember]
		public CalibrationFile CalibrationFile { get; set; }

		[DataMember]
		public RegionsOfInterestParameters RegionsOfInterestParameters { get; set; }

	}
}