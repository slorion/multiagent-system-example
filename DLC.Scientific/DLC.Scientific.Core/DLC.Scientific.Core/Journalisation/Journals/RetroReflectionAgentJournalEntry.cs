using System;

namespace DLC.Scientific.Core.Journalisation.Journals
{
	[Serializable]
	public class RetroReflectionAgentJournalEntry : JournalEntry
	{
		public int ProgressionDebut { get; set; }
		public int ProgressionFin { get; set; }
		public double MoyenneRl { get; set; }
		public double MoyenneRlMax { get; set; }
		public double ContrasteJour { get; set; }
		public double PourcentageMesure { get; set; }
		public double MoyenneFluxExt { get; set; }
	}
}
