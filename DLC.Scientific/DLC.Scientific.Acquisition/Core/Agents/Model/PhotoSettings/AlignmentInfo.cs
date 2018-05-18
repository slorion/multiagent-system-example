using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace DLC.Scientific.Acquisition.Core.Agents.Model.PhotoSettings
{
	[DataContract]
	[Serializable]
	public class AlignmentInfo
	{
		[DataMember]
		public NavigationROI NavigationROI { get; set; }

		[DataMember]
		public Initialization Initialization { get; set; }
	}
}