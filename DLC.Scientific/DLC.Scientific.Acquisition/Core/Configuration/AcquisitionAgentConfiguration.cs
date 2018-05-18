using DLC.Scientific.Core.Configuration;

namespace DLC.Scientific.Acquisition.Core.Configuration
{
	public class AcquisitionAgentConfiguration
		: AgentConfiguration
	{
		public bool AutoShowUI { get; set; }
		public bool ShowErrorListOnLoad { get; set; }
		public JournalisationConfiguration Journalisation { get; set; }

		public override void Validate()
		{
			base.Validate();

			if (this.Journalisation != null)
				this.Journalisation.Validate();
		}
	}
}