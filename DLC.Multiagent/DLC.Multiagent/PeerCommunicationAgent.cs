using DLC.Multiagent.Configuration;
using DLC.Multiagent.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DLC.Multiagent
{
	internal class PeerCommunicationAgent
		: Agent, IPeerCommunicationAgent
	{
		internal static readonly AgentConfiguration Configuration = new AgentConfiguration { Name = "[Internal Communication Agent]", ShortName = "[Communication Agent]", Description = "Allow communication between multiple instances of the Multiagent service", TypeName = typeof(PeerCommunicationAgent).FullName, Enabled = true };

		public IEnumerable<Tuple<string, AgentDisplayData, IEnumerable<string>>> GetAgentList()
		{
			return AgentBroker.Instance.GetLocalAgentInfos<IAgent>()
				.Select(info => Tuple.Create(info.AgentId, info.DisplayData, (IEnumerable<string>) info.Contracts));
		}

		public async Task<bool> RecycleAgent(string agentId)
		{
			return (await AgentBroker.Instance.RecycleAgent(agentId).ConfigureAwait(false)).IsSuccessful;
		}

		public int EnsureObservableListening(string agentId, string propertyName)
		{
			return AgentBroker.Instance.EnsureObservableListening(agentId, propertyName).ListenPort;
		}

		public string GetBrokerLog(bool archive)
		{
			return LogManagerHelper.GetBrokerLog(archive);
		}
	}
}