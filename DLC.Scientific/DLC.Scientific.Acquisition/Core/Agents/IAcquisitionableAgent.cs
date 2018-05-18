using DLC.Multiagent;
using DLC.Scientific.Acquisition.Core.Agents.Model;
using DLC.Scientific.Core.Geocoding;
using DLC.Scientific.Core.Geocoding.Bgr;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Core.Agents
{
	[ServiceContract]
	[ServiceKnownType(typeof(Rtssc))]
	public interface IAcquisitionableAgent
		: IAcquisitionAgent
	{
		/// <summary>
		/// Acquisition agent priority. Lowest priority = 1, highest priority = 100.
		/// </summary>
		int Priority { [OperationContract] get; }

		string AgentFolderPath { [OperationContract] get; }
		string AgentUniversalName { [OperationContract] get; }

		string SequenceId { [OperationContract] get; }
		string JournalRootPath { [OperationContract] get; }
		string JournalAbsoluteSavePath { [OperationContract] get; }
		string JournalRelativeSavePath { [OperationContract] get; }

		string ConfigurationJournalRelativePath { [OperationContract] get; }
		string EventJournalRelativePath { [OperationContract] get; }
		string FileJournalRelativePath { [OperationContract] get; }

		string ConfigurationJournalFileExtension { [OperationContract] get; }
		string EventJournalFileExtension { [OperationContract] get; }
		string FileJournalFileExtension { [OperationContract] get; }

		[OperationContract]
		Task<AcquisitionActionResult> InitializeRecord(InitializeRecordParameter parameters);

		[OperationContract]
		Task<AcquisitionActionResult> StartRecord(StartRecordParameter parameters);

		[OperationContract]
		Task<AcquisitionActionResult> StopRecord(StopRecordParameter parameters);

		[OperationContract]
		Task<AcquisitionActionResult> UninitializeRecord(UninitializeRecordParameter parameters);
	}
}