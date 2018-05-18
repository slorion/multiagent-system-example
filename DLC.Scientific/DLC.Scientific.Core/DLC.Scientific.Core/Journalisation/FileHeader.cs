namespace DLC.Scientific.Core.Journalisation
{
	public class FileHeader
		: JournalHeader
	{
		public string Root { get; set; }

		/// <summary>
		/// crs_id pointant vers le répertoire contenant les fichiers sources référencés par ce journal, s'il y a lieu.
		/// </summary>
		public int? RepertoireSourceId { get; set; }
		public bool ShouldSerializeRepertoireSourceId()
		{
			return RepertoireSourceId.HasValue;
		}
	}
}