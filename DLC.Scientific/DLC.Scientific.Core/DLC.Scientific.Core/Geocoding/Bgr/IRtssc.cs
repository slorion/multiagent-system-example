using System;

namespace DLC.Scientific.Core.Geocoding.Bgr
{
	public interface IRtssc
		: IEquatable<IRtssc>, IComparable<IRtssc>
	{
		string Route { get; set; }
		string Troncon { get; set; }
		string Section { get; set; }

		string SousRoute { get; set; }
		string CodeSousRoute { get; }
		string SequenceSousRoute { get; }
		string CodeSousCode { get; }
		string CodeCoteChaussee { get; }

		int Voie { get; set; }

		DirectionBgr Direction { get; set; }

		long Longueur { get; set; }

		/// <summary>
		/// Concatenation of Route + Troncon + Section + SousRoute
		/// </summary>
		string NumeroRTSS { get; set; }
		string NumeroRTSSFormate { get; set; }

		string CentreDeServiceName { get; set; }
		string DirectionGeneraleName { get; set; }
		string DirectionTerritorialeName { get; set; }
		string CentreDeServiceID { get; set; }
		string DirectionGeneraleID { get; set; }
		string DirectionTerritorialeID { get; set; }

		double? Chainage { get; set; }

		int Ide { get; set; }
		string Statut { get; set; }

		bool Equals(IRtssc other, bool ignoreChainage);
	}
}