using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace DLC.Scientific.Acquisition.Core.Agents.Model.PhotoSettings
{
	[DataContract]
	[Serializable]
	public class NavigationROI
	{
		[DataMember]
		public int NavigationROITop { get; set; }

		[DataMember]
		public int NavigationROIBottom { get; set; }

		[DataMember]
		public int NavigationROILeft { get; set; }

		[DataMember]
		public int NavigationROIRight { get; set; }
	}
}