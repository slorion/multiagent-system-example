using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using DLC.Scientific.Core.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace DLC.Scientific.Acquisition.Core.Configuration
{
	public sealed class AcquisitionConfigurationFactory
		: ConfigurationFactory
	{
		private static readonly Lazy<AcquisitionConfigurationFactory> _instance = new Lazy<AcquisitionConfigurationFactory>(() => new AcquisitionConfigurationFactory(), LazyThreadSafetyMode.PublicationOnly);
		public static AcquisitionConfigurationFactory Instance { get { return _instance.Value; } }

		private AcquisitionConfigurationFactory() { }

		protected override void LoadCore<TAgent, TModule>(RootConfiguration<TAgent, TModule> root, JObject jRoot, JsonSerializer serializer)
		{
			base.LoadCore<TAgent, TModule>(root, jRoot, serializer);

			//TODO: why a simple cast will not compile ?
			var module = root.Module as AcquisitionModuleConfiguration;

			if (module != null && !string.IsNullOrEmpty(module.ActiveProviderName))
			{
				// create an instance of the specific provider

				var providerTypeName = module.Providers[module.ActiveProviderName].Type;
				var providerType = Type.GetType(providerTypeName);

				if (providerType == null)
					throw new InvalidOperationException(string.Format("Cannot load the type '{0}' for the provider '{1}'.", providerTypeName, module.ActiveProviderName));

				JToken jProvider = jRoot["Module"]["Providers"][module.ActiveProviderName];
				module.Provider = (IAcquisitionProvider) jProvider.ToObject(providerType, serializer);

				// get the provider properties that can be set via configuration
				List<PropertyInfo> providerProperties = module.Provider.GetType().BaseType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
					.Aggregate(
							module.Provider.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).ToList(),
							(list, prop) =>
							{
								if (!list.Exists(p => string.Equals(p.Name, prop.Name, StringComparison.OrdinalIgnoreCase)))
								{
									list.Add(prop);
								}

								return list;
							}
					);

				foreach (var property in jRoot["Module"].OfType<JProperty>())
				{
					var ppi = providerProperties.SingleOrDefault(p => string.Equals(p.Name, property.Name, StringComparison.Ordinal));
					if (ppi != null)
						ppi.SetValue(module.Provider, property.Value.ToObject(ppi.PropertyType));
				}
			}
		}
	}
}