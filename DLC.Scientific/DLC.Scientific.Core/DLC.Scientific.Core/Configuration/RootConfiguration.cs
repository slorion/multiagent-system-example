using System;

namespace DLC.Scientific.Core.Configuration
{
	public class RootConfiguration<TAgent, TModule>
		: BaseConfiguration
		where TAgent : AgentConfiguration
		where TModule : ModuleConfiguration
	{
		public TAgent Agent { get; set; }
		public TModule Module { get; set; }

		public override void Validate()
		{
			base.Validate();

			if (this.Agent != null)
				this.Agent.Validate();

			if (this.Module != null)
				this.Module.Validate();
		}
	}
}