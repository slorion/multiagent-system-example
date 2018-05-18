using DLC.Multiagent;
using DLC.Scientific.Acquisition.Agents.DashboardAgent.UI;
using DLC.Scientific.Acquisition.Core.Agents;
using DLC.Scientific.Acquisition.Core.Configuration;
using DLC.Scientific.Core.Agents;

namespace DLC.Scientific.Acquisition.Agents.DashboardAgent
{
	public class DashboardAgent
		: OperationalAgent<AcquisitionAgentConfiguration, AcquisitionModuleConfiguration>, IDashboardAgent, IVisibleAgent
	{
		protected override void SetupAgentOperationalCommunicationsCore()
		{
			base.SetupAgentOperationalCommunicationsCore();

			TrackDependencyOperationalState<ILocalisationAgent>(false);
			TrackDependencyOperationalState<IDistanceAgent>(false);
			TrackDependencyOperationalState<IBgrDirectionalAgent>(false);
			TrackDependencyOperationalState<ISpeedAgent>(false);
			TrackDependencyOperationalState<IAcquisitionManagerAgent>(false);
			TrackDependencyOperationalState<IEventPanelAgent>(false);
		}

		protected override void ConfigureAgent()
		{
			base.ConfigureAgent();

			this.AutoShowUI = this.Configuration.Agent.AutoShowUI;
			this.MainUITypeName = typeof(DashboardUI).AssemblyQualifiedName;
			this.MainUIAgentTypeName = typeof(IDashboardAgent).AssemblyQualifiedName;
		}
	}
}