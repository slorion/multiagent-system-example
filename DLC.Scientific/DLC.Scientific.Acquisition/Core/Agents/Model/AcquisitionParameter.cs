using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace DLC.Scientific.Acquisition.Core.Agents.Model
{
	[DataContract]
	[Serializable]
	public abstract class AcquisitionParameter
	{
		[DataMember]
		public string SequenceId { get; set; }

		public abstract AcquisitionStep AcquisitionStep { [OperationContract] get; }
	}
}