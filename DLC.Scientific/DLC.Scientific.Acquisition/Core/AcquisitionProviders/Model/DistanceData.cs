using System;
using System.Runtime.Serialization;

namespace DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model
{
	[DataContract]
	[Serializable]
	public class DistanceData
		: ProviderData
	{
		[DataMember]
		public double AbsoluteLeftPulseCount { get; set; }

		[DataMember]
		public double AbsoluteRightPulseCount { get; set; }

		[DataMember]
		public double AbsoluteDistance { get; set; }

		[DataMember]
		public int ReferenceEncoderNumber { get; set; }

		public override string ToString()
		{
			return string.Format("{0};{1}", base.ToString(), this.AbsoluteDistance);
		}
	}
}