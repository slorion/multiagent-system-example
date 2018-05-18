using DLC.Multiagent.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ServiceModel;

namespace DLC.Multiagent
{
	internal sealed class LocalAgentInformation
		: AgentInformation
	{
		private readonly ConcurrentDictionary<string, Lazy<RxService>> _rxServices = new ConcurrentDictionary<string, Lazy<RxService>>();

		internal LocalAgentInformation(PeerNode peerNode, string agentId, AgentConfiguration configuration, IEnumerable<string> contracts, bool isInternal, IAgent agent, ServiceHost serviceHost)
			: base(peerNode, agentId, contracts, isInternal)
		{
			if (agent == null) throw new ArgumentNullException("agent");
			if (configuration == null) throw new ArgumentNullException("configuration");
			if (serviceHost == null) throw new ArgumentNullException("serviceHost");

			this.Configuration = configuration;
			this.Agent = agent;
			this.ServiceHost = serviceHost;
		}

		public override bool IsLocal { get { return true; } }
		public override bool IsReachable { get { return true; } }
		public override AgentDisplayData DisplayData { get { return this.Agent.DisplayData; } }
		public override AgentState LastKnownState { get { return this.Agent.State; } }

		public IAgent Agent { get; private set; }
		public ServiceHost ServiceHost { get; private set; }
		internal AgentConfiguration Configuration { get; private set; }

		internal ConcurrentDictionary<string, Lazy<RxService>> RxServices { get { return _rxServices; } }
	}
}