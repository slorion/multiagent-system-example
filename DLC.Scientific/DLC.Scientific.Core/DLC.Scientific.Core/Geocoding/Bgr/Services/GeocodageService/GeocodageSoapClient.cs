using DLC.Scientific.Core.Geocoding.Gps;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DLC.Scientific.Core.Geocoding.Bgr.Services.GeocodageService
{
	partial class GeocodageSoapClient
	{
		public IList<T> InverseGeocodeMultipleRoute<T>(GeoCoordinate coordonnees, int rayonRecherche, IEnumerable<int> ideRTSs)
			where T : IRtssc, new()
		{
			// conversion coordonnées gps vers lambert
			LambertCoordinate lambert = GpsHelper.ConvertNAD83ToLambertMtq(coordonnees.Latitude, coordonnees.Longitude);

			var request = new GeocoderPointsInverseRequete();
			request.ListeInformation = ideRTSs.Select(
				ideRTS => new InfoGeocoderPointInverse {
					CoordonneeX = lambert.X,
					CoordonneeY = lambert.Y,
					RayonRecherche = rayonRecherche,
					IdentifiantTheme = 207,
					IdentifiantSousRoute = Convert.ToInt32(ideRTS),
					DateActivite = DateTime.Today,
					CodeEmplacement = ""
				}).ToArray();

			GeocoderPointsInverseReponse response = this.GeocoderPointsInverse(request);

			return response.ListeInformation
				.OrderBy(item => item.DistanceTrace)
				.Select(
					item => new T {
						Chainage = item.Chainage,
						Route = item.NumeroRoute,
						Section = item.NumeroSection,
						SousRoute = item.NumeroSousRoute,
						Troncon = item.NumeroTroncon,
						Ide = Convert.ToInt32(item.IdentifiantSousRoute)
					})
				.ToList();
		}

		private IList<Tuple<double, Rtssc>> InverseGeocodeMultipleRouteWithDistance(LambertCoordinate lambert, int rayonRecherche, IEnumerable<int> ideRTSs)
		{
			var request = new GeocoderPointsInverseRequete();
			request.ListeInformation = ideRTSs.Select(
				ideRTS => new InfoGeocoderPointInverse {
					CoordonneeX = lambert.X,
					CoordonneeY = lambert.Y,
					RayonRecherche = rayonRecherche,
					IdentifiantTheme = 207,
					IdentifiantSousRoute = ideRTS,
					DateActivite = DateTime.Today,
					CodeEmplacement = ""
				}).ToArray();

			GeocoderPointsInverseReponse response = this.GeocoderPointsInverse(request);

			return response.ListeInformation
				.OrderBy(item => item.DistanceTrace)
				.Select(item => Tuple.Create(
					item.DistanceTrace,
					RtsscHelper.GetRtsscFromIde(Convert.ToInt32(item.IdentifiantSousRoute), item.Chainage)))
				.ToList();
		}

		/// <summary>
		/// obtient une liste RTSSC  qui se trouvent à la coordonnée GPS demandée
		/// </summary>
		public IList<Rtssc> InverseGeocodeMultiple(GeoCoordinate coordonnees, int rayonRecherche, int nombreMaximumRtssRechercher)
		{
			// conversion coordonnées gps vers lambert
			LambertCoordinate lambert = GpsHelper.ConvertNAD83ToLambertMtq(coordonnees.Latitude, coordonnees.Longitude);

			var request = new ObtenirSousRouteCoordonneesRequete();
			request.CoordonneeX = lambert.X;
			request.CoordonneeY = lambert.Y;
			request.RayonRecherche = rayonRecherche;
			request.NombreMaximumRtssRechercher = nombreMaximumRtssRechercher;

			ObtenirSousRouteCoordonneesReponse response = this.ObtenirSousRouteCoordonnees(request);

			return InverseGeocodeMultipleRoute<Rtssc>(coordonnees, rayonRecherche, response.ListeSousRoute.Select(coord => coord.IdentifiantSousRoute));
		}

		/// <summary>
		/// obtient une liste de RTSSC ainsi que sa distance par rapport à la BGR
		/// </summary>
		public IList<Tuple<double, Rtssc>> InverseGeocodeMultipleWithDistance(GeoCoordinate coordonnees, int rayonRecherche, int nombreMaximumRtssRechercher, IEnumerable<int> ideRtss)
		{
			// conversion coordonnées gps vers lambert
			LambertCoordinate lambert = GpsHelper.ConvertNAD83ToLambertMtq(coordonnees.Latitude, coordonnees.Longitude);

			var request = new ObtenirSousRouteCoordonneesRequete();
			request.CoordonneeX = lambert.X;
			request.CoordonneeY = lambert.Y;
			request.RayonRecherche = rayonRecherche;
			request.NombreMaximumRtssRechercher = nombreMaximumRtssRechercher;

			ObtenirSousRouteCoordonneesReponse response = this.ObtenirSousRouteCoordonnees(request);

			var listeResultat = response.ListeSousRoute.Select(coord => coord.IdentifiantSousRoute).OrderBy(_ => _).ToArray();

			if (ideRtss == null || !listeResultat.Intersect(ideRtss).Any())
				return InverseGeocodeMultipleRouteWithDistance(lambert, rayonRecherche, listeResultat);
			else
				return InverseGeocodeMultipleRouteWithDistance(lambert, rayonRecherche, listeResultat.Intersect(ideRtss));
		}

		public List<Rtssc> InverseGeocodeMultiple(GeoCoordinate coordonnees)
		{
			return this.InverseGeocodeMultiple(coordonnees, 20, 10)
				.Where(x => !string.IsNullOrEmpty(x.Statut) && x.Statut != "I" && x.Statut != "P")
				.ToList();
		}

		public List<Tuple<double, Rtssc>> InverseGeocodeMultipleWithDistance(GeoCoordinate coordonnees, int rayon)
		{
			return this.InverseGeocodeMultipleWithDistance(coordonnees, rayon, 10, null)
				.Where(x => !string.IsNullOrEmpty(x.Item2.Statut) && x.Item2.Statut != "I" && x.Item2.Statut != "P")
				.ToList();
		}

		public List<Tuple<double, Rtssc>> InverseGeocodeMultipleWithDistance(GeoCoordinate coordonnees, List<int> ideRtss, int rayon)
		{
			return this.InverseGeocodeMultipleWithDistance(coordonnees, rayon, 10, ideRtss)
				.Where(x => !string.IsNullOrEmpty(x.Item2.Statut) && x.Item2.Statut != "I" && x.Item2.Statut != "P")
				.ToList();
		}

		public IRtssc FromGeocoordinate(GeoCoordinate coordinates, int ideRtss, DateTime dateRef)
		{
			var completeResult = this.InverseGeocodeMultiple(coordinates, 20, 10)
				.Where(x => !string.IsNullOrEmpty(x.Statut) && x.Statut != "I" && x.Statut != "P");

			var result = completeResult
				.Where(item => item.Ide == ideRtss);

			if (!result.Any())
			{
				if (completeResult != null && completeResult.Any())
					return completeResult.First();
				else
					return new Rtssc();
			}
			else
			{
				return result.First();
			}
		}
	}
}