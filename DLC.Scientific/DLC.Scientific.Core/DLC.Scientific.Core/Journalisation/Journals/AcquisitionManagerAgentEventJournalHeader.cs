using System;

namespace DLC.Scientific.Core.Journalisation.Journals
{
	[Serializable]
	public class AcquisitionManagerAgentEventJournalHeader
		: EventJournalHeader
	{
		/// <summary>
		/// Date de début de l'acquisition
		/// </summary>
		public DateTime AcquisitionDate { get; set; }

		/// <summary>
		/// Conductor last name
		/// </summary>
		public string DriverLastName { get; set; }

		/// <summary>
		/// Conductor first name
		/// </summary>
		public string DriverFirstName { get; set; }

		/// <summary>
		/// Operator last name
		/// </summary>
		public string OperatorLastName { get; set; }

		/// <summary>
		/// Operator first name
		/// </summary>
		public string OperatorFirstName { get; set; }

		/// <summary>
		/// VehicleId à l'acquisition devient VehicleNumber au processing.
		/// </summary>
		public string VehicleNumber { get; set; }

		/// <summary>
		/// Vehicle name
		/// </summary>
		public string VehicleName { get; set; }

		/// <summary>
		/// Vehicle type
		/// </summary>
		public string VehicleType { get; set; }

		/// <summary>
		/// Type d'essai
		/// </summary>
		public string TestType { get; set; }

		/// <summary>
		/// Détails de l'essai
		/// </summary>
		public AcquisitionManagerAgentEventJournalHeaderDetails Details { get; set; }
	}
}