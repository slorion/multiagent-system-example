using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace DLC.Scientific.Acquisition.Core.Agents.Model.PhotoSettings
{
	[DataContract]
	[Serializable]
	public class Initialization
	{
		[DataMember]
		public double NavigationLeftSlope { get; set; }

		[DataMember]
		public double NavigationRightSlope { get; set; }

		[DataMember]
		public double NavigationLeftInnerRow { get; set; }

		[DataMember]
		public double NavigationLeftOuterRow { get; set; }

		[DataMember]
		public double NavigationRightInnerRow { get; set; }

		[DataMember]
		public double NavigationRightOuterRow { get; set; }

		[DataMember]
		public double NavigationLeftInnerColumn { get; set; }

		[DataMember]
		public double NavigationLeftOuterColumn { get; set; }

		[DataMember]
		public double NavigationRightInnerColumn { get; set; }

		[DataMember]
		public double NavigationRightOuterColumn { get; set; }

		[DataMember]
		public double ImageNavigationROIOffset { get; set; }
	}
}