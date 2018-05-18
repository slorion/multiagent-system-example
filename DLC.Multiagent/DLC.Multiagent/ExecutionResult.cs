using System;
using System.Runtime.Serialization;

namespace DLC.Multiagent
{
	[DataContract]
	[KnownType(typeof(ExecutionResult<>))]
	public class ExecutionResult
	{
		[DataMember]
		public string AgentId { get; internal set; }

		[DataMember]
		public Exception Exception { get; internal set; }

		[DataMember]
		public bool IsCanceled { get; internal set; }

		[DataMember]
		public bool IsSuccessful { get { return this.Exception == null && !this.IsCanceled; } }

		public override string ToString()
		{
			if (this.IsSuccessful)
				return string.Format("Operation on agent '{0}' has been successful.", this.AgentId);
			else if (this.IsCanceled)
				return string.Format("Operation on agent '{0}' has been canceled.", this.AgentId);
			else
				return string.Format("Operation on agent '{0}' has failed: '{1}'.", this.AgentId, this.Exception.Message);
		}
	}

	[DataContract]
	public class ExecutionResult<T>
		: ExecutionResult
	{
		[DataMember]
		public T Result { get; internal set; }
	}
}