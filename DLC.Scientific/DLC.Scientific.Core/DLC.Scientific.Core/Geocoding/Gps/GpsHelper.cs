using DotSpatial.Positioning;
using MathNet.Numerics.LinearAlgebra;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DLC.Scientific.Core.Geocoding.Gps
{
	public static partial class GpsHelper
	{
		private static int EARTH_RADIUS = 6371000;
		private static double DEGREE_TO_RAD = Math.PI / 180;
		private static double RAD_TO_DEGREE = 180 / Math.PI;

		#region Distance calculus Tools

		public static double OrthodromicDistance(GeoCoordinate coord1, GeoCoordinate coord2)
		{
			if (coord1 == null) throw new ArgumentNullException("coord1");
			if (coord2 == null) throw new ArgumentNullException("coord2");

			return OrthodromicDistance(coord1.Latitude, coord1.Longitude, coord2.Latitude, coord2.Longitude);
		}

		public static double OrthodromicDistance(double latitude1, double longitude1, double latitude2, double longitude2)
		{
			// See this article for an explanation of the algorithm:
			// http://www.movable-type.co.uk/scripts/latlong.html

			var dLat = DEGREE_TO_RAD * (latitude2 - latitude1);
			var dLon = DEGREE_TO_RAD * (longitude2 - longitude1);

			var lat1 = DEGREE_TO_RAD * latitude1;
			var lat2 = DEGREE_TO_RAD * latitude2;

			var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
					Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);

			var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

			return EARTH_RADIUS * c;
		}

		// this approximation has a precision of up to 7 decimals (micrometer) compared to orthodromic distance calculation
		public static double EuclidianDistance(Vector<double> coordinate1, Vector<double> coordinate2)
		{
			double distance = Math.Pow(coordinate1[0] - coordinate2[0], 2) + Math.Pow(coordinate1[1] - coordinate2[1], 2) + Math.Pow(coordinate1[2] - coordinate2[2], 2);
			return Math.Sqrt(distance);
		}

		public static double EuclidianDistance(double positionLat1, double positionLong1, double geoidHeight1, double positionLat2, double positionLong2, double geoidHeight2)
		{
			var position1 = ConvertGeoCoordinateToCartesian(positionLat1, positionLong1, geoidHeight1);
			var position2 = ConvertGeoCoordinateToCartesian(positionLat2, positionLong2, geoidHeight2);

			return EuclidianDistance(position1, position2);
		}

		public static double CalculateInitialBearing(double initialLat, double initialLong, double destinationLat, double destinationLong)
		{
			double bearing = CalculateBearing(initialLat, initialLong, destinationLat, destinationLong);

			return (bearing + 360) % 360;
		}

		public static double CalculateFinalBearing(double initialLat, double initialLong, double destinationLat, double destinationLong)
		{
			double bearing = CalculateBearing(destinationLat, destinationLong, initialLat, initialLong);

			return (bearing + 180) % 360;
		}

		private static double CalculateBearing(double initialLat, double initialLong, double destinationLat, double destinationLong)
		{
			// In general, your current heading will vary as you follow a great circle path (orthodrome);
			// the final heading will differ from the initial heading by varying degrees according to
			// distance and latitude (if you were to go from say 35°N,45°E (Baghdad) to 35°N,135°E (Osaka),
			// you would start on a heading of 60° and end up on a heading of 120°!).
			// This formula is for the initial bearing (sometimes referred to as forward azimuth) which if
			// followed in a straight line along a great-circle arc will take you from the start point to the
			// end point:1

			// Formula: θ = atan2( sin(Δλ).cos(φ2), cos(φ1).sin(φ2) − sin(φ1).cos(φ2).cos(Δλ) )

			// Since atan2 returns values in the range -π ... +π (that is, -180° ... +180°),
			// to normalise the result to a compass bearing (in the range 0° ... 360°,
			// with −ve values transformed into the range 180° ... 360°), convert to degrees and
			// then use (θ+360) % 360, where % is modulo.

			// For final bearing, simply take the initial bearing from the end point to the start point
			// and reverse it (using θ = (θ+180) % 360).

			double initialPhi = DEGREE_TO_RAD * initialLat;
			double initialLam = DEGREE_TO_RAD * initialLong;
			double destinationPhi = DEGREE_TO_RAD * destinationLat;
			double destinationLam = DEGREE_TO_RAD * destinationLong;

			double thetaRad = Math.Atan2(
				Math.Sin(destinationLam - initialLam) * Math.Cos(destinationPhi),
				Math.Cos(initialPhi) * Math.Sin(destinationPhi) - Math.Sin(initialPhi) * Math.Cos(destinationPhi) * Math.Cos(destinationLam - initialLam));

			return RAD_TO_DEGREE * thetaRad;
		}

		#endregion

		#region Projection Tools

		/// <summary>
		/// Make a projection of GPS coordinates to a cartesian system.
		/// Calculations are done as if Earth would be a perfect sphere.
		/// </summary>
		/// <param name="lat">Latitude</param>
		/// <param name="lon">Longitude</param>
		/// <returns>Cartesian coordinates.</returns>
		public static Vector<double> ConvertGeoCoordinateToCartesian(double lat, double lon)
		{
			double radLat = lat * Math.PI / 180;
			double radLon = lon * Math.PI / 180;

			double x = EARTH_RADIUS * Math.Cos(radLat) * Math.Cos(radLon);
			double y = EARTH_RADIUS * Math.Cos(radLat) * Math.Sin(radLon);
			double z = EARTH_RADIUS * Math.Sin(radLat);

			return Vector<double>.Build.Dense(new[] { x, y, z });
		}

		public static Vector<double> ConvertGeoCoordinateToCartesian(double lat, double lon, double heightAboveEllipsoid)
		{
			Position3D position3D = new Position3D(new Latitude(lat), new Longitude(lon), new Distance(heightAboveEllipsoid, DistanceUnit.Meters));
			CartesianPoint cartesianPoint = position3D.ToCartesianPoint(DotSpatial.Positioning.Ellipsoid.Default);

			return Vector<double>.Build.Dense(new[] { cartesianPoint.X.Value, cartesianPoint.Y.Value, cartesianPoint.Z.Value });
		}

		public static Vector<double> ConvertCartesianToGeoCoordinate(double x, double y, double z)
		{
			double r = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));

			return Vector<double>.Build.Dense(new[] {
				Math.Asin(z / r) * (180 / Math.PI),
				Math.Atan2(y, x) * (180 / Math.PI)
			});
		}

		public static Vector<double> ConvertCartesianToGeoCoordinate(Vector<double> point)
		{
			double r = Math.Sqrt(Math.Pow(point[0], 2) + Math.Pow(point[1], 2) + Math.Pow(point[2], 2));

			return Vector<double>.Build.Dense(new[] {
				Math.Asin(point[2] / r) * (180 / Math.PI),
				Math.Atan2(point[1], point[0]) * (180 / Math.PI)
			});
		}

		public static Position3D ConvertCartesianWGS84ToGeoCoordinate(Distance x, Distance y, Distance z)
		{
			return ConvertCartesianToGeoCoordinate(new CartesianPoint(x, y, z));
		}

		public static Position3D ConvertCartesianToGeoCoordinate(CartesianPoint point)
		{
			return point.ToPosition3D(DotSpatial.Positioning.Ellipsoid.Default);
		}

		public static Vector<double> ConvertCartesianToPolar(double x, double y)
		{
			return Vector<double>.Build.Dense(new[] {
				// theta
				Math.Atan2(y, x),
				// radius
				Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2))
			});
		}

		public static Vector<double> ConvertCartesianToPolar(double x, double y, double z)
		{
			return Vector<double>.Build.Dense(new[] {
				// theta
				Math.Atan2(y, x),
				// radius
				Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)),
				z
			});
		}

		public static Vector<double> ConvertPolarToCartesian(double radius, double theta)
		{
			return Vector<double>.Build.Dense(new[] {
				radius * Math.Cos(theta),
				radius * Math.Sin(theta)
			});
		}

		public static Vector<double> ConvertPolarToCartesian(double radius, double theta, double z)
		{
			return Vector<double>.Build.Dense(new[] {
				radius * Math.Cos(theta),
				radius * Math.Sin(theta),
				z
			});
		}

		public static LambertCoordinate ConvertNAD83ToLambertMtq(double lat, double lon)
		{
			IGeographicCoordinateSystem gcs;
			IProjectedCoordinateSystem coordsys;
			CoordinateSystemFactory cfac = CreateNad83ToLambertFactory(out gcs, out coordsys);

			// execute transformation
			ICoordinateTransformation trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems(gcs, coordsys);

			return new LambertCoordinate(trans.MathTransform.Transform(new[] { lon, lat }));
		}

		public static IEnumerable<LambertCoordinate> ConvertNAD83ToLambertMtq(List<GeoCoordinate> dataIn)
		{
			return dataIn.Select(c => ConvertNAD83ToLambertMtq(c.Latitude, c.Longitude));
		}

		private static CoordinateSystemFactory CreateNad83ToLambertFactory(out IGeographicCoordinateSystem gcs, out IProjectedCoordinateSystem coordsys)
		{
			CoordinateSystemFactory cfac = new CoordinateSystemFactory();

			// Define ellipsoid GRS 1980 used by NAD83
			IEllipsoid ellipsoid = cfac.CreateFlattenedSphere("GRS 1980", 6378137, 298.257222101, LinearUnit.Metre);

			// Define NAD83 system
			IHorizontalDatum datum = cfac.CreateHorizontalDatum("NAD83 (CSRS)", DatumType.HD_Other, ellipsoid, null);
			gcs = cfac.CreateGeographicCoordinateSystem("NAD83 (CSRS)", AngularUnit.Degrees, datum,
				PrimeMeridian.Greenwich, new AxisInfo("Lon", AxisOrientationEnum.East), new AxisInfo("Lat", AxisOrientationEnum.North));

			// Define Lambert parameters
			List<ProjectionParameter> parameters = new List<ProjectionParameter>(5);
			parameters.Add(new ProjectionParameter("latitude_of_origin", 44));
			parameters.Add(new ProjectionParameter("central_meridian", -70));
			parameters.Add(new ProjectionParameter("standard_parallel_1", 50));
			parameters.Add(new ProjectionParameter("standard_parallel_2", 46));
			parameters.Add(new ProjectionParameter("false_easting", 800000));
			parameters.Add(new ProjectionParameter("false_northing", 0));
			IProjection projection = cfac.CreateProjection("Lambert Conic Conformal (2SP)", "lambert_conformal_conic_2sp", parameters);

			coordsys = cfac.CreateProjectedCoordinateSystem("NAD83/Lambert", gcs, projection, LinearUnit.Metre, new AxisInfo("East", AxisOrientationEnum.East), new AxisInfo("North", AxisOrientationEnum.North));

			return cfac;
		}

		#endregion
	}
}