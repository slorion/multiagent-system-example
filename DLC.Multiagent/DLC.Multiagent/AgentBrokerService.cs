using System;
using System.ServiceProcess;

namespace DLC.Multiagent
{
	public class AgentBrokerService
		: ServiceBase
	{
		private static readonly TimeSpan ServiceTimeout = TimeSpan.FromSeconds(30);

		public AgentBrokerService(string serviceName, string configurationFilePath)
			: base()
		{
			if (string.IsNullOrEmpty(serviceName)) throw new ArgumentNullException("serviceName");

			this.ServiceName = serviceName;
			this.ConfigurationFilePath = configurationFilePath;
		}

		public string ConfigurationFilePath { get; private set; }

		protected override void OnStart(string[] args)
		{
			// priority is given to service start arguments
			string configurationFilePath;
			if (args.Length == 0)
				configurationFilePath = this.ConfigurationFilePath;
			else
				configurationFilePath = args[0];

			AgentBroker.Instance.LoadConfiguration(configurationFilePath);
			AgentBroker.Instance.Start().Wait(ServiceTimeout);
		}

		protected override void OnStop()
		{
			AgentBroker.Instance.Stop().Wait(ServiceTimeout);
		}

		protected override void Dispose(bool disposing)
		{
			AgentBroker.Instance.Dispose();
			base.Dispose(disposing);
		}
	}
}