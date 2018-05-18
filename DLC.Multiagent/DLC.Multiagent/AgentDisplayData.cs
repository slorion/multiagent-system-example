using System;
using System.Runtime.Serialization;

namespace DLC.Multiagent
{
	[DataContract]
	[Serializable]
	public class AgentDisplayData
	{
		public AgentDisplayData(string name, string shortName, string description)
		{
			this.Name = name;
			this.ShortName = shortName;
			this.Description = description;
		}

		[DataMember(EmitDefaultValue = false)]
		public string Name { get; private set; }

		[DataMember(EmitDefaultValue = false)]
		public string ShortName { get; private set; }

		[DataMember(EmitDefaultValue = false)]
		public string Description { get; private set; }
	}
}