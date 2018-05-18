using NLog;
using NLog.Config;
using NLog.LayoutRenderers;
using System.IO;
using System.Text;

namespace DLC.Multiagent.Logging
{
	[LayoutRenderer(MultiagentLayoutRenderer.Name)]
	internal class MultiagentLayoutRenderer
		: LayoutRenderer
	{
		public const string Name = "dlc-multiagent";

		[RequiredParameter]
		[DefaultParameter]
		public string Variable { get; set; }

		protected override void Append(StringBuilder builder, LogEventInfo logEvent)
		{
			if (!string.IsNullOrEmpty(this.Variable))
			{
				switch (this.Variable.ToLowerInvariant())
				{
					case "sourcename":
						builder.Append(Path.GetFileNameWithoutExtension(AgentBroker.Instance.ConfigurationFilePath));
						break;
					case "host":
						builder.Append(AgentBroker.Instance.LocalPeerNode.Host);
						break;
					case "port":
						builder.Append(AgentBroker.Instance.LocalPeerNode.Port);
						break;
				}
			}
		}
	}
}