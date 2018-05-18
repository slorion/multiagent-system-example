using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using DLC.Scientific.Core.Analysis.LinearAlgebra;
using DLC.Scientific.Core.Configuration;
using DLC.Scientific.Core.Geocoding.Gps;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace DLC.Scientific.Acquisition.Core.AcquisitionProviders
{
	public abstract class LocalisationProvider
		: AcquisitionProvider<LocalisationData>
	{
		private class ProviderState
		{
			public int EstimateAccuracyCpt { get; set; }
			public Circle3DData CircleData { get; set; }
			public Line3DData LineData { get; set; }
			public DateTime LastRealPositionTime { get; set; }

			public int EstimateCpt { get; set; }
			public double EstimatedAverageGapAlt { get; set; }
			public double EstimatedAverageSpeed { get; set; }
			public int NbSuccessiveSpeedEstimation { get; set; }
		}

		private const double CircleFitting = 4;
		private const int NbPointToEvaluateAccuracy = 20;
		private const int DispersionRatio = 5;
		private const int MaxSuccessiveSpeedEstimation = 20;

		public int EstimationBufferSize { get; set; }
		public bool EstimationEnabled { get; set; }

		/// <summary>
		/// GPS data reception frequency.
		/// </summary>
		public int Frequency { get; set; }

		protected override void ValidateConfigurationCore()
		{
			base.ValidateConfigurationCore();

			if (this.EstimationEnabled && this.EstimationBufferSize <= 0) BaseConfiguration.OutOfRangeMin("PositionEstimationBufferSize", 1);
		}

		protected override IObservable<LocalisationData> CreateDataSource(IObservable<LocalisationData> rawDataSource)
		{
			if (rawDataSource == null) throw new ArgumentNullException("rawDataSource");

			var buffer = new Queue<LocalisationData>(this.EstimationBufferSize + 1);
			var state = new ProviderState();

			return base.CreateDataSource(rawDataSource)
				.Select(
					current =>
					{
						if (this.EstimationEnabled)
						{
							TryCorrectData(buffer.OrderBy(data => data.RawData.PositionData.Utc), current, state);

							buffer.Enqueue(current);
							if (buffer.Count >= this.EstimationBufferSize)
								buffer.Dequeue();
						}

						return current;
					});
		}

		private void TryCorrectData(IEnumerable<LocalisationData> buffer, LocalisationData current, ProviderState state)
		{
			if (buffer == null) throw new ArgumentNullException("buffer");
			if (current == null) throw new ArgumentNullException("current");
			if (state == null) throw new ArgumentNullException("state");

			// if at least 2 distinct positions (lat/lon)
			if (buffer.Select(x => x.RawData.PositionData).Distinct(PositionLatLongComparer.Instance).Skip(1).Any())
			{
				var previous = buffer.Select(b => b.CorrectedData);

				// if signal is lost, guess speed and position
				if (current.GpsStatus == GpsStatus.SignalLost)
				{
					if (buffer.Last().GpsStatus != GpsStatus.SignalLost)
						SetCurrentPrimitiveFittingModel(previous, current.RawData, state);

					EstimateSignalLostData(previous, current.RawData, state);
				}
				else
				{
					// do a multipath check only if moving
					if (current.RawData.VelocityData.SpeedKmh >= 1)
					{
						TryEstimateSpeed(previous, current.RawData, state);
						TryCorrectMultiPathData(previous, current, state);
					}
				}
			}
		}

		private void TryCorrectMultiPathData(IEnumerable<GeoData> previous, LocalisationData current, ProviderState state)
		{
			if (previous == null) throw new ArgumentNullException("previous");
			if (current == null) throw new ArgumentNullException("current");
			if (state == null) throw new ArgumentNullException("state");

			// If the GPS coordinate is outside the precision range, then estimate lastPosition,
			// but proceed like this only up to NbPointToEvaluateAccuracy times.
			// Once that count is reached, ignore two points and then start the verification again.
			// This ensures that we eventually go out of the evaluation loop.
			if (state.EstimateAccuracyCpt >= 0 && state.EstimateAccuracyCpt < NbPointToEvaluateAccuracy)
			{
				if (IsPositionReliable(previous, current.RawData, state))
				{
					state.EstimateAccuracyCpt = 0;
				}
				else // multipath case
				{
					if (state.EstimateAccuracyCpt == 0)
						SetCurrentPrimitiveFittingModel(previous, current.RawData, state);

					state.EstimateAccuracyCpt++;
					current.CorrectedData = GetCorrectedData(previous, current.RawData, state);
					current.GpsStatus = GpsStatus.MultiPathDetected;
				}
			}
			else if (state.EstimateAccuracyCpt < 0)
				state.EstimateAccuracyCpt++;
			else
				state.EstimateAccuracyCpt = -2; // skip 2 points
		}

		private void SetCurrentPrimitiveFittingModel(IEnumerable<GeoData> previous, GeoData current, ProviderState state)
		{
			if (previous == null) throw new ArgumentNullException("previous");
			if (current == null) throw new ArgumentNullException("current");
			if (state == null) throw new ArgumentNullException("state");

			state.LineData = new Line3DData(previous.Count());
			state.CircleData = new Circle3DData();

			Matrix<double> gpsCoordinates = ConvertPositionBufferToCartesianMatrix(previous, current, state);

			if (!PrimitivesFitting.ValidateDispersion(gpsCoordinates, DispersionRatio))
			{
				return;
			}

			if (state.LineData.ResidualsNorm > CircleFitting)
			{
				state.CircleData = PrimitivesFitting.Ls3DCircle(gpsCoordinates);
			}
			else
			{
				state.LineData = PrimitivesFitting.Ls3Dline(gpsCoordinates);
			}

			SetAltitudeEstimation(previous, current, state);

			state.LastRealPositionTime = previous.Last().PositionData.Utc;
			state.EstimateCpt = 0;
		}

		private static Matrix<double> ConvertPositionBufferToCartesianMatrix(IEnumerable<GeoData> previous, GeoData current, ProviderState state)
		{
			if (previous == null) throw new ArgumentNullException("previous");
			if (current == null) throw new ArgumentNullException("current");
			if (state == null) throw new ArgumentNullException("state");

			PositionData[] distinctCoordinates = previous.Select(x => x.PositionData).Distinct(PositionLatLongComparer.Instance).ToArray();

			var positions = new double[distinctCoordinates.Length][];
			for (int i = 0; i < distinctCoordinates.Length; i++)
			{
				var coor = GpsHelper.ConvertGeoCoordinateToCartesian(distinctCoordinates[i].Latitude, distinctCoordinates[i].Longitude);
				positions[i] = coor.ToArray();
			}

			return Matrix<double>.Build.DenseOfRowArrays(positions);
		}

		private static void SetAltitudeEstimation(IEnumerable<GeoData> previous, GeoData current, ProviderState state)
		{
			if (previous == null) throw new ArgumentNullException("previous");
			if (current == null) throw new ArgumentNullException("current");
			if (state == null) throw new ArgumentNullException("state");

			GeoData[] previousArray = previous.ToArray();
			var lstDiffAltitude = new double[previousArray.Length - 1];

			// build a list of the deltas between each consecutive buffer element
			for (int i = 1; i < previousArray.Length; i++)
				lstDiffAltitude[i - 1] = previousArray[i].PositionData.Altitude - previousArray[i - 1].PositionData.Altitude;

			Array.Sort(lstDiffAltitude);

			// get the average of the second tier of the buffer, we discard data from the first and last tier
			int firstThird = lstDiffAltitude.Length > 2 ? lstDiffAltitude.Length / 3 : 0;
			int secondThird = firstThird > 0 ? firstThird : lstDiffAltitude.Length;
			state.EstimatedAverageGapAlt = lstDiffAltitude.Skip(firstThird).Take(secondThird).Sum() / secondThird;
		}

		private void EstimateSignalLostData(IEnumerable<GeoData> previous, GeoData current, ProviderState state)
		{
			if (previous == null) throw new ArgumentNullException("previous");
			if (current == null) throw new ArgumentNullException("current");
			if (state == null) throw new ArgumentNullException("state");

			EstimateSpeed(previous, current, state);
			EstimatePositionFromCurrentPFModel(previous, current, state);
			state.EstimateAccuracyCpt = 0;
		}

		/// <summary>
		/// Try to estimate speed only if there was a 10 km/h speed jump compared to the last speed value
		/// and if the maximum successive estimation count has not been reached.
		/// </summary>
		private void TryEstimateSpeed(IEnumerable<GeoData> previous, GeoData current, ProviderState state)
		{
			if (previous == null) throw new ArgumentNullException("previous");
			if (current == null) throw new ArgumentNullException("current");
			if (state == null) throw new ArgumentNullException("state");

			if (Math.Abs(current.VelocityData.SpeedMs - previous.Last().VelocityData.SpeedMs) * 3.6 > 10
				&& state.NbSuccessiveSpeedEstimation < MaxSuccessiveSpeedEstimation)
			{
				EstimateSpeed(previous, current, state);
				state.NbSuccessiveSpeedEstimation++;
			}
			else
			{
				state.NbSuccessiveSpeedEstimation = 0;
			}
		}

		private static void EstimateSpeed(IEnumerable<GeoData> previous, GeoData current, ProviderState state)
		{
			if (previous == null) throw new ArgumentNullException("previous");
			if (current == null) throw new ArgumentNullException("current");
			if (state == null) throw new ArgumentNullException("state");

			IEnumerable<GeoData> lstArg = previous.Where(x => x.VelocityData.SpeedKmh > 0);

			if (lstArg.Any())
				state.EstimatedAverageSpeed = lstArg.Average(x => x.VelocityData.SpeedKmh);
			else
				state.EstimatedAverageSpeed = current.VelocityData.SpeedKmh;

			current.VelocityData.SpeedKmh = state.EstimatedAverageSpeed;
		}

		private GeoData GetCorrectedData(IEnumerable<GeoData> previous, GeoData current, ProviderState state)
		{
			if (previous == null) throw new ArgumentNullException("previous");
			if (current == null) throw new ArgumentNullException("current");
			if (state == null) throw new ArgumentNullException("state");

			var correctedData = new GeoData();

			correctedData.VelocityData.SpeedKmh = current.VelocityData.SpeedKmh;
			correctedData.PositionData.Utc = current.PositionData.Utc;
			EstimatePositionFromCurrentPFModel(previous, correctedData, state);

			return correctedData;
		}

		private void EstimatePositionFromCurrentPFModel(IEnumerable<GeoData> previous, GeoData current, ProviderState state)
		{
			if (previous == null) throw new ArgumentNullException("previous");
			if (current == null) throw new ArgumentNullException("current");
			if (state == null) throw new ArgumentNullException("state");

			GeoData last = previous.Last();

			// keep direction obtained from buffer data, but use lastDistance to normalize the vector
			long timeDistortionGap = EvaluateTimeDistortion(last.PositionData.Utc, current.PositionData.Utc, this.Frequency);

			int safeTimeDistortionGap;
			if (timeDistortionGap >> 32 == 0)
				safeTimeDistortionGap = (int) timeDistortionGap;
			else
			{
				current.PositionData.Latitude = last.PositionData.Latitude;
				current.PositionData.Longitude = last.PositionData.Longitude;
				current.PositionData.Altitude = last.PositionData.Altitude;
				return;
			}

			state.EstimateCpt++;

			Vector<double> newPosition;
			if (state.CircleData.CircleRadius > 0)
			{
				newPosition = GetCircleNextPosition(previous, current, state, safeTimeDistortionGap);
			}
			else if (state.LineData.ResidualsNorm != 0)
			{
				newPosition = GetLineNextPosition(previous, current, state, safeTimeDistortionGap, state.EstimateCpt == 1);
			}
			else
			{
				// when points are scattered
				// take the last point instead of making an absurd estimate
				newPosition = Vector<double>.Build.Dense(new[] { last.PositionData.Latitude, last.PositionData.Longitude });
			}

			double newAltitude = last.PositionData.Altitude + state.EstimatedAverageGapAlt;
			current.PositionData.Latitude = newPosition[0];
			current.PositionData.Longitude = newPosition[1];
			current.PositionData.Altitude = newAltitude;
		}

		private Vector<double> GetCircleNextPosition(IEnumerable<GeoData> previous, GeoData current, ProviderState state, int pointCount)
		{
			if (previous == null) throw new ArgumentNullException("previous");
			if (current == null) throw new ArgumentNullException("current");
			if (state == null) throw new ArgumentNullException("state");
			if (pointCount < 0) throw new ArgumentOutOfRangeException("pointCount");

			double currentSpeed = current.VelocityData.SpeedKmh;
			var last = previous.Last();

			double distance = CalculateAverageDistanceBySpeed(currentSpeed, this.Frequency);

			var currentPosition = GpsHelper.ConvertGeoCoordinateToCartesian(last.PositionData.Latitude, last.PositionData.Longitude);
			var newPosition = PrimitivesFitting.GetCartesianPoint(currentPosition, state.CircleData, pointCount, distance);

			return GpsHelper.ConvertCartesianToGeoCoordinate(newPosition[0], newPosition[1], newPosition[2]);
		}

		private Vector<double> GetLineNextPosition(IEnumerable<GeoData> previous, GeoData current, ProviderState state, int pointCount, Boolean isFirstEstimatedPoint)
		{
			if (previous == null) throw new ArgumentNullException("previous");
			if (current == null) throw new ArgumentNullException("current");
			if (state == null) throw new ArgumentNullException("state");
			if (pointCount < 0) throw new ArgumentOutOfRangeException("pointCount");

			double currentSpeed = current.VelocityData.SpeedKmh;
			var last = previous.Last();

			Vector<double> currentPosition;
			double distance = CalculateAverageDistanceBySpeed(currentSpeed, this.Frequency);

			if (isFirstEstimatedPoint)
				currentPosition = state.LineData.IntersectionPoint;
			else
				currentPosition = GpsHelper.ConvertGeoCoordinateToCartesian(last.PositionData.Latitude, last.PositionData.Longitude);

			var newPosition = PrimitivesFitting.GetCartesianPoint(currentPosition, state.LineData, pointCount, distance);

			return GpsHelper.ConvertCartesianToGeoCoordinate(newPosition[0], newPosition[1], newPosition[2]);
		}

		private bool IsPositionReliable(IEnumerable<GeoData> previous, GeoData current, ProviderState state)
		{
			if (previous == null) throw new ArgumentNullException("previous");
			if (current == null) throw new ArgumentNullException("current");
			if (state == null) throw new ArgumentNullException("state");

			// if previous.count > 1
			if (previous.Skip(1).Any())
			{
				var position = current.PositionData;
				var speed = current.VelocityData.SpeedKmh;
				var last = previous.Last();
				var secondToLast = previous.Reverse().Skip(1).Take(1).Single();

				var oldVector = GpsHelper.ConvertGeoCoordinateToCartesian(secondToLast.PositionData.Latitude, secondToLast.PositionData.Longitude);
				var newVector = GpsHelper.ConvertGeoCoordinateToCartesian(last.PositionData.Latitude, last.PositionData.Longitude);
				var currentVector = GpsHelper.ConvertGeoCoordinateToCartesian(position.Latitude, position.Longitude);

				// only make verification if new position is more recent
				if (last.PositionData.Utc < position.Utc)
				{
					oldVector = newVector.Subtract(oldVector);
					oldVector = oldVector.Normalize(2);
					currentVector = currentVector.Subtract(newVector);
					currentVector = currentVector.Normalize(2);

					// calculate angle between vector of last two points
					// and vector of last and current points
					double radAngle = Math.Acos(oldVector.DotProduct(currentVector));
					double degreeAngle = radAngle * 180 / Math.PI;

					if (degreeAngle > 25)
					{
						return false;
					}

					// calculate distance between last and current points
					double distance = GpsHelper.OrthodromicDistance(position.Latitude, position.Longitude, last.PositionData.Latitude, last.PositionData.Longitude);
					long timeDistortionGap = EvaluateTimeDistortion(last.PositionData.Utc, position.Utc, this.Frequency);
					double averageDistance = CalculateAverageDistanceBySpeed(speed, this.Frequency);

					if (distance > averageDistance * timeDistortionGap + averageDistance)
					{
						return false;
					}
				}
			}

			return true;
		}

		private static long EvaluateTimeDistortion(DateTime oldestTime, DateTime newestTime, int frequency)
		{
			if (frequency < 1) throw new ArgumentOutOfRangeException("frequency", "frequency must be greater than 0.");

			TimeSpan time = newestTime - oldestTime;
			return Convert.ToInt64(Math.Truncate(time.TotalMilliseconds / frequency));
		}

		private static double CalculateAverageDistanceBySpeed(double speedKmh, int frequency)
		{
			if (speedKmh < 0) throw new ArgumentOutOfRangeException("speedKmh", "speedKmh must be greater than or equal to 0.");
			if (frequency < 1) throw new ArgumentOutOfRangeException("frequency", "frequency must be greater than 0.");

			// = speed in meters/hour * frequency in hours
			return speedKmh * 1000 * Decimal.ToDouble(Decimal.Divide(frequency, 3600000));
		}
	}
}