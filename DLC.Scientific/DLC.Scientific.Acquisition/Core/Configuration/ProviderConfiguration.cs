using DLC.Scientific.Core.Configuration;
using Newtonsoft.Json;

namespace DLC.Scientific.Acquisition.Core.Configuration
{
	public class ProviderConfiguration
		: BaseConfiguration
	{
		public string Name { get; set; }

		[JsonProperty]
		internal string Type { get; set; }

		public override void Validate()
		{
			base.Validate();

			if (string.IsNullOrEmpty(this.Name)) MissingProperty("Name");
			if (string.IsNullOrEmpty(this.Type)) MissingProperty("Type");
		}
	}
}