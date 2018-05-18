using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using DLC.Scientific.Core.Geocoding;
using DLC.Scientific.Core.Geocoding.Bgr;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Modules.BgrModule
{
	// data can be downloaded from https://www.donneesquebec.ca/recherche/fr/dataset/systeme-de-reference-lineaire-transports-quebec
	public class PgsqlBgrProvider
		: BgrProvider
	{
		private const int SridBgr = 3798;
		private const int SridNorthAmerica = 4269;

		public string ConnectionString { get; set; }

		protected override void ValidateConfigurationCore()
		{
			base.ValidateConfigurationCore();

			if (string.IsNullOrEmpty(this.ConnectionString)) throw new InvalidOperationException("ConnectionString is mandatory.");
		}

		public override GeoCoordinate GeoCodage(IRtssc rtssc)
		{
			if (rtssc == null) throw new ArgumentNullException("rtssc");

			using (var cnn = new NpgsqlConnection(this.ConnectionString))
			using (var cmd = cnn.CreateCommand())
			{
				cmd.CommandText =
					@"SELECT
						st_X(the_geom) as x,
						st_Y(the_geom) as y
					FROM (
						SELECT
							st_SetSrid(st_MakePoint(:p_x, :p_y), :p_sridIn), :p_sridOut) AS the_geom
						FROM reseau_exe
						WHERE
							(:p_ideSousRoute IS NOT NULL AND ide_sous_r = :p_ideSousRoute)
							OR (:p_numRts IS NOT NULL AND num_rts = :p_numRts)
						LIMIT 1
					) t";

				cmd.Parameters.AddWithValue("p_sridIn", SridBgr);
				cmd.Parameters.AddWithValue("p_sridOut", SridNorthAmerica);
				cmd.Parameters.AddWithValue("p_ideSousRoute", rtssc.Ide);
				cmd.Parameters.AddWithValue("p_numRts", rtssc.NumeroRTSS);

				cnn.Open();
				using (var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SingleResult))
				{
					if (reader.Read())
						return new GeoCoordinate { Longitude = Convert.ToDouble(reader["x"]), Latitude = Convert.ToDouble(reader["y"]) };
					else
						return null;
				}
			}
		}

		public override double GetSectionLength(IRtssc rtssc)
		{
			if (rtssc == null) throw new ArgumentNullException("rtssc");

			using (var cnn = new NpgsqlConnection(this.ConnectionString))
			using (var cmd = cnn.CreateCommand())
			{
				cmd.CommandText =
					@"SELECT
						longueur
					FROM reseau_exe
					WHERE
						num_rts = :p_route || :p_troncon || :p_section || :p_sousRoute
					LIMIT 1";

				cmd.Parameters.AddWithValue(":p_route", rtssc.Route);
				cmd.Parameters.AddWithValue(":p_sousRoute", rtssc.SousRoute);
				cmd.Parameters.AddWithValue(":p_section", rtssc.Section);
				cmd.Parameters.AddWithValue(":p_troncon", rtssc.Troncon);

				cnn.Open();
				return Convert.ToDouble(cmd.ExecuteScalar());
			}
		}

		public override IEnumerable<IRtssc> GetRtssFromRoute(string route)
		{
			if (string.IsNullOrEmpty(route)) throw new ArgumentNullException("route");

			using (var cnn = new NpgsqlConnection(this.ConnectionString))
			using (var cmd = cnn.CreateCommand())
			{
				cmd.CommandText =
					@"SELECT
						num_rts,
						num_route,
						num_tronc,
						num_sectn,
						cod_srte || seq_srte || cod_code || code_chas AS sous_route
						--,longueur
					FROM reseau_exe
					WHERE
						num_route = :p_route
					ORDER BY num_sectn";

				cmd.Parameters.AddWithValue(":p_route", route);

				var results = new List<Rtssc>();

				cnn.Open();
				using (var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
				{
					while (reader.Read())
						results.Add(
							new Rtssc {
								Route = Convert.ToString(reader["num_route"]),
								Troncon = Convert.ToString(reader["num_tronc"]),
								Section = Convert.ToString(reader["num_sectn"]),
								SousRoute = Convert.ToString(reader["sous_route"]),
							});
				}

				return ApplyActiveFilters(results);
			}
		}

		public override IEnumerable<string> SelectRoutes(GeoCoordinate coord, double searchRadiusInMeters, int? maxRouteNumber)
		{
			if (coord == null) throw new ArgumentNullException("coord");
			if (searchRadiusInMeters < 0) throw new ArgumentOutOfRangeException("searchRadiusInMeters", "searchRadiusInMeters must be greater than or equal to 0.");

			using (var cnn = new NpgsqlConnection(this.ConnectionString))
			using (var cmd = cnn.CreateCommand())
			{
				cmd.CommandText =
					@"SELECT DISTINCT
						num_route
					FROM reseau_exe
					WHERE
						(:p_maxRoute IS NULL OR CAST(num_route AS INTEGER) < :p_maxRoute)
						AND st_DWithin(the_geom, st_Transform(st_SetSrid(st_MakePoint(:p_x, :p_y), :p_sridIn), :p_sridOut), :p_radius)
					ORDER BY num_route";

				cmd.Parameters.AddWithValue("p_sridIn", SridNorthAmerica);
				cmd.Parameters.AddWithValue("p_sridOut", SridBgr);
				cmd.Parameters.AddWithValue("p_x", coord.Longitude);
				cmd.Parameters.AddWithValue("p_y", coord.Latitude);
				cmd.Parameters.AddWithValue("p_radius", searchRadiusInMeters);
				cmd.Parameters.AddWithValue("p_maxRoute", maxRouteNumber);

				var result = new List<string>();

				cnn.Open();
				using (var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
				{
					while (reader.Read())
						result.Add(Convert.ToString(reader[0]));
				}

				return result;
			}
		}

		protected override IRtssc ObtenirRtsscSuivant(GeoCoordinate coord, double searchRadiusInMeters, DirectionBgr direction)
		{
			if (coord == null) throw new ArgumentNullException("coord");
			if (searchRadiusInMeters < 0) throw new ArgumentOutOfRangeException("searchRadiusInMeters", "searchRadiusInMeters must be greater than or equal to 0.");

			IRtssc rtssc = GeoCodageInverse(coord);
			if (rtssc == null)
				return null;

			double distance;
			if (direction == DirectionBgr.ForwardChaining)
				distance = rtssc.Longueur - rtssc.Chainage.Value + 1;
			else
				distance = -1 * (rtssc.Chainage.Value + 5);

			long absoluteDistance = Convert.ToInt64(Math.Abs(distance));

			rtssc = SelectRtssc(coord, searchRadiusInMeters, DateTime.Now)
				.Where(r => r.Longueur <= absoluteDistance)
				.FirstOrDefault();

			if (rtssc != null)
				rtssc.Chainage = distance >= 0 ? distance : rtssc.Longueur + distance;

			return rtssc;
		}

		protected override IEnumerable<IRtssc> SelectRtssc(GeoCoordinate coord, double searchRadiusInMeters, DateTime? date)
		{
			if (coord == null) throw new ArgumentNullException("coord");
			if (searchRadiusInMeters < 0) throw new ArgumentOutOfRangeException("searchRadiusInMeters", "searchRadiusInMeters must be greater than or equal to 0.");

			using (var cnn = new NpgsqlConnection(this.ConnectionString))
			using (var cmd = cnn.CreateCommand())
			{
				string dateInBgrFormat = date?.ToString("yyyyMMdd000000");

				cmd.CommandText =
					@"SELECT
						ide_sous_r,
						num_route,
						num_tronc,
						num_sectn,
						cod_srte || seq_srte || cod_code || code_chas AS sous_route,
						val_longr_sous_route,
						st_Transform(st_SetSrid(st_MakePoint(:p_x, :p_y), :p_sridIn), :p_sridOut) AS center
					FROM reseau_exe
					WHERE
						(:p_date IS NULL OR :p_date >= dat_debut_)
						AND (:p_date IS NULL OR dat_fin_ IS NULL OR :p_date <= dat_fin_)
						AND st_DWithin(the_geom, center, :p_radius)
					ORDER BY st_Distance_Sphere(the_geom, center)";

				cmd.Parameters.AddWithValue("p_sridIn", SridNorthAmerica);
				cmd.Parameters.AddWithValue("p_sridOut", SridBgr);
				cmd.Parameters.AddWithValue("p_x", coord.Longitude);
				cmd.Parameters.AddWithValue("p_y", coord.Latitude);
				cmd.Parameters.AddWithValue("p_radius", searchRadiusInMeters);
				cmd.Parameters.AddWithValue("p_date", dateInBgrFormat);

				var results = new List<Rtssc>();

				cnn.Open();
				using (var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
				{
					while (reader.Read())
						results.Add(
							new Rtssc {
								Ide = Convert.ToInt32(reader["ide_sous_r"]),
								Route = Convert.ToString(reader["num_route"]),
								Troncon = Convert.ToString(reader["num_tronc"]),
								Section = Convert.ToString(reader["num_sectn"]),
								SousRoute = Convert.ToString(reader["sous_route"]),
								Longueur = Convert.ToInt32(reader["val_longr_sous_route"])
							});
				}

				return ApplyActiveFilters(results);
			}
		}
	}
}