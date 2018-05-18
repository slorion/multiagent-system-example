using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DLC.Multiagent
{
	[DataContract]
	[Serializable]
	public abstract class AgentInformation
	{
		internal AgentInformation(PeerNode peer, string agentId, IEnumerable<string> contracts, bool isInternal)
		{
			if (peer == null) throw new ArgumentNullException("peer");
			if (string.IsNullOrEmpty(agentId)) throw new ArgumentNullException("agentId");
			if (contracts == null) throw new ArgumentNullException("contracts");

			this.PeerNode = peer;
			this.AgentId = agentId;
			this.Contracts = contracts.Distinct().ToArray();
			this.IsInternal = isInternal;
		}

		[DataMember]
		public PeerNode PeerNode { get; private set; }

		[DataMember]
		public string AgentId { get; private set; }

		[DataMember]
		public IReadOnlyCollection<string> Contracts { get; private set; }

		[DataMember]
		public bool IsInternal { get; private set; }

		[DataMember]
		public bool IsRecycled { get; internal set; }

		public abstract bool IsLocal { get; }
		public abstract bool IsReachable { get; }
		public abstract AgentDisplayData DisplayData { get; }
		public abstract AgentState LastKnownState { get; }

		public override string ToString()
		{
			return string.Format("{0} -> {1}", this.AgentId, this.LastKnownState);
		}
	}
}