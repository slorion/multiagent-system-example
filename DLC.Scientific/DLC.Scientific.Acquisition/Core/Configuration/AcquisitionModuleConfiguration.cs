using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using DLC.Scientific.Core.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DLC.Scientific.Acquisition.Core.Configuration
{
	public class AcquisitionModuleConfiguration
		: ModuleConfiguration
	{
		public string ActiveProviderName { get; set; }

		[JsonProperty]
		internal Dictionary<string, ProviderConfiguration> Providers { get; set; }

		public IAcquisitionProvider Provider { get; set; }

		public override void Validate()
		{
			base.Validate();

			if (string.IsNullOrEmpty(this.ActiveProviderName)) MissingProperty("ActiveProviderName");
			if (!this.Providers.ContainsKey(this.ActiveProviderName)) throw new ConfigurationException(string.Format("The provider '{0}' cannot be found in the configuration.", this.ActiveProviderName));
		}
	}
}