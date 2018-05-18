using MathNet.Numerics.LinearAlgebra;
using System;
using System.Reactive.Linq;
using System.Threading;

namespace DLC.Scientific.Core.Geocoding.Gps
{
	static partial class GpsHelper
	{
		private static IObservable<double> GetReliableRemainingDistanceSource(IObservable<Tuple<GeoData, GpsStatus>> gpsSource, IObservable<double> distanceSource, double minDistanceToSwitchToGps, Func<GeoData, double> calculateGpsRemainingDistance)
		{
			if (gpsSource == null) throw new ArgumentNullException("gpsSource");
			if (minDistanceToSwitchToGps < 0) throw new ArgumentOutOfRangeException("minDistanceToSwitchToGps", minDistanceToSwitchToGps, "Must be greater than or equal to 0.");
			if (calculateGpsRemainingDistance == null) throw new ArgumentNullException("calculateGpsRemainingDistance");

			double lastRemainingDistance = double.MinValue;
			bool useDistance = false;

			// when a GPS coordinate is received, select the most reliable data source:
			//		1 - select GPS if there is no odometer
			//		2 - select odomoter if GPS signal was lost and distance is close to triggering point (typically < 50m, minDistanceToSwitchToGps)
			//		3 - select either GPS or odometer according to GPS coordinate reliability
			//			a - if reliable, then calculate remaining distance using GPS
			//			b - otherwise, if at least one calculation based on the GPS was done, then odometer will be used to calculate remaining distance

			var gpsDataSource = gpsSource
				.Where(
					data =>
					{
						if (distanceSource == null) // Step 1
							return true;

						if (useDistance && lastRemainingDistance < minDistanceToSwitchToGps) // Step 2
							return false;

						useDistance = data.Item2 != GpsStatus.Reliable;
						return !useDistance;
					})
			   .Select(
				   geoCoord =>
				   {
					   lastRemainingDistance = calculateGpsRemainingDistance(geoCoord.Item1);
					   return lastRemainingDistance; // Step 3a
				   });

			var distanceDataSource = (distanceSource ?? Observable.Empty<double>())
			   .Where(data => useDistance && lastRemainingDistance != double.MinValue)
			   .Select(
					data =>
					{
						var comparandDistance = lastRemainingDistance;

						Interlocked.CompareExchange(ref lastRemainingDistance, comparandDistance - 1, comparandDistance);

						return lastRemainingDistance; // Step 3b
					});

			return distanceDataSource
			   .Merge(gpsDataSource);
		}

		public static IObservable<bool> DetectProximityToTarget(IObservable<Tuple<GeoData, GpsStatus>> gpsSource, IObservable<double> distanceSource, GeoCoordinate triggerCoordinate, double minDistanceToSwitchToGps, double offsetFromTriggerPoint, double radius)
		{
			if (gpsSource == null) throw new ArgumentNullException("gpsSource");
			if (triggerCoordinate == null) throw new ArgumentNullException("triggerCoordinate");
			if (minDistanceToSwitchToGps < 0) throw new ArgumentOutOfRangeException("minDistanceToSwitchToGps", "Must be greater than or equal to 0.");
			if (offsetFromTriggerPoint < 0) throw new ArgumentOutOfRangeException("offsetFromTriggerPoint", "Must be greater than or equal to 0.");
			if (radius < 0) throw new ArgumentOutOfRangeException("radius", "Must be greater than or equal to 0.");

			const int NB_POINT_TO_EVALUATE_PROXIMITY = 10;
			double lastRemainingDistance = double.MaxValue;
			int nbDistanceCroissant = 0;

			var reliableSource =
				GetReliableRemainingDistanceSource(
					gpsSource,
					distanceSource,
					minDistanceToSwitchToGps,
					(geoCoord) => OrthodromicDistance(geoCoord.PositionData.Latitude, geoCoord.PositionData.Longitude, triggerCoordinate.Latitude, triggerCoordinate.Longitude) - (radius + offsetFromTriggerPoint));

			return reliableSource
				   .Select(
						distance =>
						{
							if (distance <= 0)
								return true;

							if (lastRemainingDistance < distance)
								nbDistanceCroissant++;
							else
								nbDistanceCroissant = 0;

							lastRemainingDistance = distance;

							if (nbDistanceCroissant >= NB_POINT_TO_EVALUATE_PROXIMITY)
								throw new InvalidOperationException("Trigger radius has not been reached.");

							return false;
						});
		}

		public static IObservable<bool> DetectPositionExceededByAngleAndDistance(IObservable<Tuple<GeoData, GpsStatus>> gpsSource, IObservable<double> distanceSource, GeoCoordinate triggerCoordinate, double minDistanceToSwitchToGps, double offsetFromTriggerPoint, bool triggerOnlyAfterReached, double proximityRangeToStartCalculations, int frequency, Vector<double> directionalTriggerVector)
		{
			if (gpsSource == null) throw new ArgumentNullException("gpsSource");
			if (triggerCoordinate == null) throw new ArgumentNullException("triggerCoordinate");
			if (minDistanceToSwitchToGps < 0) throw new ArgumentOutOfRangeException("minDistanceToSwitchToGps", minDistanceToSwitchToGps, "Must be greater than or equal to 0.");
			if (offsetFromTriggerPoint < 0) throw new ArgumentOutOfRangeException("offsetFromTriggerPoint", offsetFromTriggerPoint, "Must be greater than or equal to 0.");
			if (proximityRangeToStartCalculations < 0) throw new ArgumentOutOfRangeException("proximityRangeToStartCalculations", proximityRangeToStartCalculations, "Must be greater than or equal to 0.");
			if (frequency < 0) throw new ArgumentOutOfRangeException("frequency", frequency, "Must be greater than or equal to 0.");
			if (directionalTriggerVector == null) throw new ArgumentNullException("directionalTriggerVector");

			var reliableSource = GetReliableRemainingDistanceSource(
				gpsSource
					// only do calculations if inside a proximity range
					.Where(gpsData => gpsData.Item1.VelocityData.SpeedKmh > 0 && OrthodromicDistance(gpsData.Item1.PositionData.Latitude, gpsData.Item1.PositionData.Longitude, triggerCoordinate.Latitude, triggerCoordinate.Longitude) <= proximityRangeToStartCalculations),
				distanceSource
					.Where(distance => distance <= proximityRangeToStartCalculations),
				minDistanceToSwitchToGps,
				(geoCoord) => DistanceBetweenVehicleAndBGrVirtualLine(triggerCoordinate, geoCoord, frequency, directionalTriggerVector) - offsetFromTriggerPoint);

			bool triggerNext = false;

			return reliableSource
				.Select(
					distance =>
					{
						bool isTriggerFired = false;

						try
						{
							if (!triggerOnlyAfterReached)
							{
								if (distance <= 0)
									isTriggerFired = true;
							}
							else
							{
								if (distance <= 0 && !triggerNext)
									triggerNext = true;
								else if (triggerNext)
									isTriggerFired = true;
							}
						}
						catch
						{
							return false;
						}

						return isTriggerFired;
					});
		}

		private static double CalculateAverageDistanceBySpeed(double speedKmh, int frequency)
		{
			// speed in meters/hour * frequency in hours
			return speedKmh * 1000 * Decimal.ToDouble(Decimal.Divide(frequency, 3600000));
		}

		private static double DistanceBetweenVehicleAndBGrVirtualLine(GeoCoordinate triggerCoordinate, GeoData currentCoord, int frequency, Vector<double> directionalTriggerVector)
		{
			Vector<double> bgrTriggerVector = ConvertGeoCoordinateToCartesian(triggerCoordinate.Latitude, triggerCoordinate.Longitude);
			Vector<double> inversedBGRDirectionalTriggerVector = directionalTriggerVector * -1;
			inversedBGRDirectionalTriggerVector = inversedBGRDirectionalTriggerVector.Normalize(2);

			Vector<double> currentVehicleVector = ConvertGeoCoordinateToCartesian(currentCoord.PositionData.Latitude, currentCoord.PositionData.Longitude);

			// calculate distance between the vehicle and a perpendicular line to the provided directional vector
			Vector<double> triggerPointToVehicleVector = currentVehicleVector.Subtract(bgrTriggerVector);
			triggerPointToVehicleVector = triggerPointToVehicleVector.Normalize(2);

			double radAngle =
				Math.Acos(
					inversedBGRDirectionalTriggerVector.DotProduct(triggerPointToVehicleVector)
					/ (
						Math.Sqrt(inversedBGRDirectionalTriggerVector.DotProduct(inversedBGRDirectionalTriggerVector))
						* Math.Sqrt(triggerPointToVehicleVector.DotProduct(triggerPointToVehicleVector))
					)
				);

			var orthodromicDistance = OrthodromicDistance(currentCoord.PositionData.Latitude, currentCoord.PositionData.Longitude, triggerCoordinate.Latitude, triggerCoordinate.Longitude);

			double distanceVehicleToVirtualTriggerLine = Math.Cos(radAngle) * orthodromicDistance;
			double averageDistanceBetweenGpsCoordinates = CalculateAverageDistanceBySpeed(currentCoord.VelocityData.SpeedKmh, frequency);

			return distanceVehicleToVirtualTriggerLine - averageDistanceBetweenGpsCoordinates;
		}
	}
}