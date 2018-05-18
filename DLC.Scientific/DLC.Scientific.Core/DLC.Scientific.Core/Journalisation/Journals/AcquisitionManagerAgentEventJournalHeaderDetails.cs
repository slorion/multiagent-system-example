using System;

namespace DLC.Scientific.Core.Journalisation.Journals
{
	[Serializable]
	public class AcquisitionManagerAgentEventJournalHeaderDetails
	{
		/// <summary>
		/// Nom du côté utilisé pour les mesures
		/// </summary>
		public string CoteMesure { get; set; }

		/// <summary>
		/// Couleur du marquage
		/// </summary>
		public string CouleurMarquage { get; set; }

		/// <summary>
		/// Nom du marquage
		/// </summary>
		public string NomMarquage { get; set; }
	}
}
