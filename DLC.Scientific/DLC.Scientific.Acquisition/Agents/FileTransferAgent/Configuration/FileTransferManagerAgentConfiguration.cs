using DLC.Scientific.Acquisition.Core.Configuration;
using System;

namespace DLC.Scientific.Acquisition.Agents.FileTransferAgent.Configuration
{
	public class FileTransferManagerAgentConfiguration
		: AcquisitionAgentConfiguration
	{
		public bool AutoCollapseGrid { get; set; }
	}
}