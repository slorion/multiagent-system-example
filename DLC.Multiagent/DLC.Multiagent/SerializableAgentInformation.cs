using System;
using System.Runtime.Serialization;

namespace DLC.Multiagent
{
	[DataContract]
	[Serializable]
	public sealed class SerializableAgentInformation
		: AgentInformation
	{
		[DataMember(Name = "IsLocal")]
		private readonly bool _isLocal;

		[DataMember(Name = "IsReachable")]
		private readonly bool _isReachable;

		[DataMember(Name = "DisplayData")]
		private readonly AgentDisplayData _displayData;

		[DataMember(Name = "LastKnownState")]
		private readonly AgentState _lastKnownState;

		public SerializableAgentInformation(AgentInformation agentInfo)
			: base(agentInfo.PeerNode, agentInfo.AgentId, agentInfo.Contracts, agentInfo.IsInternal)
		{
			_isLocal = agentInfo.IsLocal;
			_isReachable = agentInfo.IsReachable;
			_displayData = agentInfo.DisplayData;
			_lastKnownState = agentInfo.LastKnownState;
		}

		public override bool IsLocal { get { return _isLocal; } }
		public override bool IsReachable { get { return _isReachable; } }
		public override AgentDisplayData DisplayData { get { return _displayData; } }
		public override AgentState LastKnownState { get { return _lastKnownState; } }
	}
}