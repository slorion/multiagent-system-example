using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using System;
using System.Runtime.Serialization;

namespace DLC.Scientific.Acquisition.Core.Agents.Model
{
	[DataContract]
	[Serializable]
	public class AcquisitionManagerStateChangedResult
	{
		[DataMember]
		public ProviderState ProviderState { get; set; }

		[DataMember]
		public AcquisitionParameter Parameters { get; set; }

		[DataMember]
		public AcquisitionActionResult Result { get; set; }
	}
}