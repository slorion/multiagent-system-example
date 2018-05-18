using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DLC.Multiagent.Configuration
{
	public sealed class AgentBrokerConfiguration
	{
		public AgentBrokerConfiguration()
		{
			this.Agents = new List<AgentConfiguration>();
			this.Peers = new List<PeerNodeConfiguration>();
			this.DependencyFolders = new List<string>();
		}

		public static string DefaultConfigurationFilePath { get { return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "DLC.Multiagent.Services.conf"); } }

		public bool AutoStartBroker { get; set; }
		public bool AutoActivateAgents { get; set; }
		public bool AutoShowLogWindow { get; set; }
		public string LogConfigurationFilePath { get; set; }

		public string Host { get; set; }
		public int Port { get; set; }
		public int RxPort { get; set; }
		public string Description { get; set; }

		public int HeartbeatFrequencyInMs { get; set; }
		public int MinOperationRetryDelayInMs { get; set; }
		public int MaxOperationRetryDelayInMs { get; set; }

		public IList<AgentConfiguration> Agents { get; private set; }
		public IList<PeerNodeConfiguration> Peers { get; private set; }
		public IList<string> DependencyFolders { get; private set; }

		public int? WcfBindingListenBacklog { get; set; }
		public int? WcfBindingMaxConnections { get; set; }
		public int? WcfBehaviorMaxConcurrentCalls { get; set; }
		public int? WcfBehaviorMaxConcurrentSessions { get; set; }
		public int? WcfBehaviorMaxConcurrentInstances { get; set; }

		public static AgentBrokerConfiguration Load(string configurationFilePath)
		{
			if (string.IsNullOrEmpty(configurationFilePath)) throw new ArgumentNullException("configurationFilePath");

			var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

			if (!Path.IsPathRooted(configurationFilePath))
				configurationFilePath = Path.Combine(root, configurationFilePath);

			var configFile = File.ReadAllText(configurationFilePath);
			var config = JsonConvert.DeserializeObject<AgentBrokerConfiguration>(configFile);

			config.DependencyFolders = new List<string>(config.DependencyFolders.Select(f => IsPathRootedOrEmpty(f) ? f : Path.Combine(root, f)));

			foreach (var agent in config.Agents)
			{
				agent.ConfigurationFilePath = IsPathRootedOrEmpty(agent.ConfigurationFilePath) ? agent.ConfigurationFilePath : Path.Combine(root, agent.ConfigurationFilePath);

				var folders = new List<string>(config.DependencyFolders.Select(f => IsPathRootedOrEmpty(f) ? f : Path.Combine(root, f)));
				agent.DependencyFolders.Clear();
				agent.DependencyFolders.AddRange(folders);
			}

			return config;
		}

		public void Validate()
		{
			var errors = new List<string>();

			if (string.IsNullOrEmpty(this.Host)) errors.Add("Host is mandatory.");
			if (this.Port < 1 || this.Port > 65535) errors.Add("Port must be between 1 and 65535 inclusively.");
			if (this.RxPort < 1 || this.RxPort > 65535) errors.Add("RxPort must be between 1 and 65535 inclusively.");
			if (this.Port == this.RxPort) errors.Add("Port and RxPort must be different.");

			if (this.HeartbeatFrequencyInMs <= 0) errors.Add("HeartBeatFrequencyInMs must be > 0.");
			if (this.MinOperationRetryDelayInMs <= 0) errors.Add("MinOperationRetryDelayInMs must be > 0.");
			if (this.MaxOperationRetryDelayInMs < this.MinOperationRetryDelayInMs) errors.Add("MaxOperationRetryDelayInMs must be >= MinOperationRetryDelayInMs.");

			if (this.WcfBindingListenBacklog <= 0) errors.Add("WcfBindingListenBacklog must be > 0. To use the default WCF value, do not provide a value.");
			if (this.WcfBindingMaxConnections <= 0) errors.Add("WcfBindingMaxConnections must be > 0. To use the default WCF value, do not provide a value.");
			if (this.WcfBehaviorMaxConcurrentCalls <= 0) errors.Add("WcfBehaviorMaxConcurrentCalls must be > 0. To use the default WCF value, do not provide a value.");
			if (this.WcfBehaviorMaxConcurrentSessions <= 0) errors.Add("WcfBehaviorMaxConcurrentSessions must be > 0. To use the default WCF value, do not provide a value.");
			if (this.WcfBehaviorMaxConcurrentInstances <= 0) errors.Add("WcfBehaviorMaxConcurrentInstances must be > 0. To use the default WCF value, do not provide a value.");

			foreach (var agent in this.Agents)
			{
				if (string.IsNullOrEmpty(agent.Name)) errors.Add(string.Format("Agent of type '{0}': Name is mandatory.", agent.TypeName));
				if (string.IsNullOrEmpty(agent.ShortName)) errors.Add(string.Format("Agent '{0}': ShortName is mandatory.", agent.Name));
				if (string.IsNullOrEmpty(agent.TypeName)) errors.Add(string.Format("Agent '{0}': TypeName is mandatory.", agent.Name));
			}

			foreach (var peer in this.Peers)
			{
				if (string.IsNullOrEmpty(peer.Host)) errors.Add(string.Format("Peer '{0}:{1}': Host is mandatory.", peer.Host, peer.Port));
				if (peer.Port < 1 || peer.Port > 65535) errors.Add(string.Format("Peer '{0}:{1}': Port must be between 1 and 65535 inclusively.", peer.Host, peer.Port));
				if (peer.RxPort < 1 || peer.RxPort > 65535) errors.Add(string.Format("Peer '{0}:{1}': RxPort must be between 1 and 65535 inclusively.", peer.Host, peer.Port));
				if (peer.Port == peer.RxPort) errors.Add(string.Format("Peer '{0}:{1}': Port and RxPort must be different.", peer.Host, peer.Port));
			}

			if (errors.Count > 0)
				throw new ConfigurationErrorsException(string.Join(Environment.NewLine, errors));
		}

		private static bool IsPathRootedOrEmpty(string path)
		{
			return string.IsNullOrEmpty(path) || Path.IsPathRooted(path);
		}
	}
}