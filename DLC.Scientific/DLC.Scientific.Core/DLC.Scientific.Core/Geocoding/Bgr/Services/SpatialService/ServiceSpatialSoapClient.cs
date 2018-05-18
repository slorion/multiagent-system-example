using System;

namespace DLC.Scientific.Core.Geocoding.Bgr.Services.SpatialService
{
	partial class ServiceSpatialSoapClient
	{
		private const int IdentDictnTheme = 207;
		private const int SystemCoordGps = 8307;
		private const int SystemCoordLambert = 1557057;

		/// <summary>
		/// Obtenir le RTSSC qui se trouve à la coordonnée GPS demandée
		/// à la date de référence choisie
		/// </summary>
		/// <param name="coordonnees"></param>
		/// <param name="rayonRecherche">Tolérance de distance entre le point GPS 
		/// et le RTSSC le plus proche, en mètre</param>
		/// <param name="dateRef"></param>
		public T InverseGeocode<T>(GeoCoordinate coordonnees, double rayonRecherche, DateTime dateRef)
			where T : IRtssc, new()
		{
			double trace = 0;
			string emplacement = "";
			string route = "";
			string troncon = "";
			string section = "";
			string sousRoute = "";
			int chainage = 0;

			// conversion coordonnées gps vers lambert
			LambertCoordinate lambert = DLC.Scientific.Core.Geocoding.Gps.GpsHelper.ConvertNAD83ToLambertMtq(coordonnees.Latitude, coordonnees.Longitude);

			// géocodage inverse
			this.GeocoderPointInverse(
				lambert.X, lambert.Y, rayonRecherche, dateRef, IdentDictnTheme, null, false,
				ref trace, ref emplacement, ref route, ref troncon, ref section, ref sousRoute, ref chainage);

			return new T {
				Route = route,
				Troncon = troncon,
				Section = section,
				SousRoute = sousRoute,
				Direction = 0,
				Chainage = chainage
			};
		}

		/// <summary>
		/// Obtenir la coordonnées Gps d'un RTSSC
		/// à la date de référence choisie
		/// </summary>
		public T Geocode<T>(IRtssc rtssc, DateTime dateRef)
			where T : GeoCoordinate, new()
		{
			double x = 0, y = 0, m = 0;
			double lat = 0, lon = 0;
			int identSRoute = 0;

			// géocodage du rtssc
			this.GeocodePoint(rtssc.Route, rtssc.Troncon, rtssc.Section, rtssc.SousRoute, (int) rtssc.Chainage.Value,
				null, "", dateRef, ref x, ref y, ref m, ref identSRoute);

			// convertion des coordonnées lambert vers GPS
			this.TransformerCoordonees(x, y, SystemCoordLambert, SystemCoordGps, ref lon, ref lat);

			return new T {
				Longitude = lon,
				Latitude = lat,
				Altitude = 0
			};
		}

		public T Geocode<T>(IRtssc rtssc) // dateRef non utilisé
			where T : GeoCoordinate, new()
		{
			double x = 0, y = 0, m = 0;
			double lat = 0, lon = 0;
			int identSRoute = 0;

			// géocodage du rtssc
			this.GeocodePoint(rtssc.Route, rtssc.Troncon, rtssc.Section, rtssc.SousRoute, (int) rtssc.Chainage.Value,
				null, "", DateTime.Now, ref x, ref y, ref m, ref identSRoute);

			// convertion des coordonnées lambert vers GPS
			this.TransformerCoordonees(x, y, SystemCoordLambert, SystemCoordGps, ref lon, ref lat);

			return new T {
				Longitude = lon,
				Latitude = lat,
				Altitude = 0
			};
		}

		public IRtssc InverseGeocode(GeoCoordinate coordonnees, double rayonRecherche, DateTime dateRef)
		{
			return this.InverseGeocode<Rtssc>(coordonnees, rayonRecherche, dateRef);
		}
	}
}