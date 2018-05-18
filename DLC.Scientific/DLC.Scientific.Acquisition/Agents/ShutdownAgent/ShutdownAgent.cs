using DLC.Multiagent;
using DLC.Scientific.Acquisition.Agents.ShutdownAgent.UI;
using DLC.Scientific.Acquisition.Core.Agents;
using DLC.Scientific.Acquisition.Core.Configuration;
using DLC.Scientific.Core.Agents;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Agents.ShutdownAgent
{
	public class ShutdownAgent
		: OperationalAgent<AcquisitionAgentConfiguration, AcquisitionModuleConfiguration>, IShutdownAgent, IVisibleAgent
	{
		protected override void ConfigureAgent()
		{
			base.ConfigureAgent();

			this.AutoShowUI = this.Configuration.Agent.AutoShowUI;
			this.MainUITypeName = typeof(ShutdownUI).AssemblyQualifiedName;
			this.MainUIAgentTypeName = typeof(IShutdownAgent).AssemblyQualifiedName;
		}

		public Task ShutdownMachine()
		{
			return Task.Run(() => ShutdownHelper.SystemShutdown());
		}

		public Task RebootMachine()
		{
			return Task.Run(() => ShutdownHelper.SystemReboot());
		}
	}
}