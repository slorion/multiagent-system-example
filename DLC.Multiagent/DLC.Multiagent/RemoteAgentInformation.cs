using DLC.Multiagent.Wcf;
using System;
using System.Collections.Generic;

namespace DLC.Multiagent
{
	internal sealed class RemoteAgentInformation
		: AgentInformation
	{
		private readonly AgentDisplayData _displayData;

		private bool _isReachable;
		private AgentState _lastKnownState;

		internal RemoteAgentInformation(PeerNode peerNode, string agentId, AgentDisplayData displayData, IEnumerable<string> contracts, bool isInternal, ServiceClientFactory serviceClientFactory)
			: base(peerNode, agentId, contracts, isInternal)
		{
			if (displayData == null) throw new ArgumentNullException("displayData");
			if (serviceClientFactory == null) throw new ArgumentNullException("serviceClientFactory");

			_displayData = displayData;
			this.ServiceClientFactory = serviceClientFactory;
		}

		public override bool IsLocal { get { return false; } }
		public override AgentDisplayData DisplayData { get { return _displayData; } }

		public override bool IsReachable { get { return _isReachable; } }
		public override AgentState LastKnownState { get { return _lastKnownState; } }

		public ServiceClientFactory ServiceClientFactory { get; private set; }

		// refreshed at each heartbeat (see RegisterPeer)
		internal void SetIsReachable(bool value) { _isReachable = value; }
		internal void SetLastKnownState(AgentState value) { _lastKnownState = value; }
	}
}