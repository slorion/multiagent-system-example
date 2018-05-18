using System;
using System.Runtime.Serialization;

namespace DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model
{
	[DataContract]
	[Serializable]
	public class DistanceCalibrationData
		: CalibrationData
	{
		public DistanceCalibrationData()
		{
			this.IntervalLength = 1;
		}

		[DataMember]
		public int ReferenceEncoderNumber { get; set; }

		[DataMember]
		public int PpkmLeft { get; set; }

		[DataMember]
		public int PpkmRight { get; set; }

		[DataMember]
		public int IntervalLength { get; set; }

		public override string ToString()
		{
			return string.Format("PPKM left = {0}; PPKM right = {1}; Interval length = {2}", this.PpkmLeft, this.PpkmRight, this.IntervalLength);
		}
	}
}