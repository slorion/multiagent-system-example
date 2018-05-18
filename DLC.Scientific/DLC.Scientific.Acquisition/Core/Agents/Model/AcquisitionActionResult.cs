using DLC.Framework;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using System;
using System.Runtime.Serialization;

namespace DLC.Scientific.Acquisition.Core.Agents.Model
{
	[DataContract]
	[Serializable]
	public class AcquisitionActionResult
	{
		public AcquisitionActionResult(string sequenceId, AcquisitionStep acquisitionStep, string agentId, string agentName)
		{
			if (string.IsNullOrEmpty(agentId)) throw new ArgumentNullException("agentId");
			if (string.IsNullOrEmpty(agentName)) throw new ArgumentNullException("agentName");

			this.SequenceId = sequenceId;
			this.AcquisitionStep = acquisitionStep;
			this.AgentId = agentId;
			this.AgentName = agentName;
			this.IsSuccessful = true;
			this.Timestamp = DateTimePrecise.Now;
		}

		[DataMember]
		public bool IsSuccessful { get; set; }

		[DataMember]
		public DateTime Timestamp { get; private set; }

		[DataMember]
		public string SequenceId { get; set; }

		[DataMember]
		public AcquisitionStep AcquisitionStep { get; set; }

		[DataMember]
		public string AgentId { get; set; }

		[DataMember]
		public string AgentName { get; set; }

		[DataMember]
		public Exception Exception { get; set; }

		[DataMember]
		public ProviderState ProviderState { get; set; }

		[DataMember]
		public string MachineName { get; set; }

		[DataMember]
		public string ConfigurationJournalRelativePath { get; set; }

		[DataMember]
		public string EventJournalRelativePath { get; set; }

		[DataMember]
		public string FileJournalRelativePath { get; set; }
	}
}