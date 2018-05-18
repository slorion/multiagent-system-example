using System.Collections.Generic;

namespace DLC.Multiagent.Configuration
{
	public class AgentConfiguration
	{
		public AgentConfiguration()
		{
			this.DependencyFolders = new List<string>();
		}

		public bool Enabled { get; set; }
		public string Name { get; set; }
		public string ShortName { get; set; }
		public string Description { get; set; }
		public string ConfigurationFilePath { get; set; }
		public string TypeName { get; set; }
		public List<string> DependencyFolders { get; private set; }
	}
}