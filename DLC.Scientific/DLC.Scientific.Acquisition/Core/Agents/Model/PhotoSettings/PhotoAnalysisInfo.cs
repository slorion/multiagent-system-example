using System;
using System.Runtime.Serialization;

namespace DLC.Scientific.Acquisition.Core.Agents.Model.PhotoSettings
{
	[DataContract]
	[Serializable]
	public class PhotoAnalysisInfo
	{

		[DataMember]
		public int FrameId { get; set; }

		[DataMember]
		public long ExposureAutoTarget { get; set; }

		[DataMember]
		public string ExposureMode { get; set; }

		[DataMember]
		public double ExposureTime { get; set; }

		[DataMember]
		public long IrisAutoTarget { get; set; }

		[DataMember]
		public string IrisMode { get; set; }

		[DataMember]
		public long IrisVideoLevel { get; set; }

		[DataMember]
		public float PixelAverageValue { get; set; }

		[DataMember]
		public double SaturationScore { get; set; }

		[DataMember]
		public string TypeCasLimite { get; set; }
	}
}