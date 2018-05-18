using System;

namespace DLC.Scientific.Core.Journalisation.Journals
{
	public class FileProcessingJournalHeader
		: FileHeader
	{
		/// <summary>
		/// suivi de traitement lié
		/// </summary>
		public string SuiviTraitementIdLie { get; set; }

		/// <summary>
		/// suivi de traitement 
		/// </summary>
		public string SuiviTraitementId { get; set; }
	}
}