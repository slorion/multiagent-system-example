using DLC.Scientific.Core.Geocoding.Gps;
using GeoCode;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DLC.Scientific.Core.Geocoding.Bgr.Services
{
	public static class GeoCodeExtensions
	{
		public static IEnumerable<Tuple<double, Rtssc>> InverseGeocodeMultipleWithDistance(this PostGIS client, GeoCoordinate coordinates, int radius, DateTime date)
		{
			if (client == null) throw new ArgumentNullException("client");
			if (coordinates == null) throw new ArgumentNullException("coordinates");

			LambertCoordinate lambert = GpsHelper.ConvertNAD83ToLambertMtq(coordinates.Latitude, coordinates.Longitude);

			return client.ObtenirListeRTSSC(new Point { X = lambert.X, Y = lambert.Y }, radius, string.Empty, date.ToString())
				.Select(rtssc => Tuple.Create(
					rtssc.Distance,
					new Rtssc {
						Ide = client.ObtenirIDE(rtssc, date.ToShortDateString()).IdentifiantIDE,
						Route = rtssc.Route,
						Troncon = rtssc.Troncon,
						Section = rtssc.Section,
						SousRoute = rtssc.CodeSousRoute,
						Chainage = Math.Round(rtssc.Chainage, 0),
						Longueur = rtssc.LongueurSousRoute
					}));
		}
	}
}