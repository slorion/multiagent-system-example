using System;

namespace DLC.Scientific.Core.Journalisation
{
	public class FileVersionJournalHeader
		: FileHeader
	{
		private Guid _fileVersionID;

		public Guid FileVersionID
		{
			get
			{
				if (_fileVersionID == null || _fileVersionID.CompareTo(Guid.Empty) == 0)
					_fileVersionID = Guid.NewGuid();

				return _fileVersionID;
			}
			set
			{
				_fileVersionID = value;
			}
		}

		public string SuiviTraitementId { get; set; }

		/// <summary>
		/// Priorité (0-100) de traitement du fichier
		/// </summary>
		public int Priorite { get; set; }

		/// <summary>
		/// Indique si la vérification d'intégrité des descendants doit être ignorée lors du classement.
		/// </summary>
		/// <remarks>
		/// Cette valeur devrait être activée uniquement lorsqu'une partie des journaux de l'arborescence a déjà été classée correctement.
		/// </remarks>
		public bool SkipTreeCheck { get; set; }
	}
}