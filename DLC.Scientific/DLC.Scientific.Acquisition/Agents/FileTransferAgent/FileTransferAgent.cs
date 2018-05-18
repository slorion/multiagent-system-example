using DLC.Multiagent;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using DLC.Scientific.Acquisition.Core.Agents;
using DLC.Scientific.Acquisition.Core.Agents.Model;
using DLC.Scientific.Acquisition.Core.Configuration;
using System;
using System.Threading.Tasks;

//TODO: tester arrêt/recyclage agent, drive qui disparaît, etc.

namespace DLC.Scientific.Acquisition.Agents.FileTransferAgent
{
	public class FileTransferAgent
		: ProviderAgent<FileTransferProvider, FileTransferData, AcquisitionAgentConfiguration, AcquisitionModuleConfiguration>, IFileTransferAgent
	{
		public IObservable<FileTransferData> FileTransferDataSource { get { return this.Provider.DataSource; } }

		public bool IsTransferring { get { return this.Provider.IsTransferring; } }
		public IObservable<bool> IsTransferringDataSource { get { return this.Provider.IsTransferringDataSource; } }

		public void StartTransferring()
		{
			this.Provider.StartTransferring();
		}

		public Task StopTransferring()
		{
			return this.Provider.StopTransferring();
		}
	}
}