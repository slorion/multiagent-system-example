using DLC.Framework.Reactive;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace DLC.Scientific.Core.Geocoding.Bgr
{
	public static class RtsscHelper
	{
		public static IObservable<IRtssc> GetBgrTraceFromGps(this IObservable<GeoCoordinate> gpsTrace, int bufferSize, BgrDataTypes allowedDataTypes, bool allowSkipIfProcessing, Func<GeoCoordinate, IEnumerable<IRtssc>, IRtssc> geocodageInverse)
		{
			if (gpsTrace == null) throw new ArgumentNullException("gpsTrace");
			if (bufferSize < 0) throw new ArgumentOutOfRangeException("bufferSize", bufferSize, "bufferSize doit être supérieur ou égal à 0.");
			if (geocodageInverse == null) throw new ArgumentNullException("geocodageInverse");

			var buffer = new Queue<IRtssc>(bufferSize + 1);
			GeoCoordinate lastCoord = null;
			IRtssc previous = null;

			Func<GeoCoordinate, IRtssc> convert =
				coord =>
				{
					IRtssc currentRtssc;

					if (previous != null && coord.Latitude == lastCoord.Latitude && coord.Longitude == lastCoord.Longitude)
						currentRtssc = previous;
					else
						currentRtssc = geocodageInverse(coord, buffer);

					if (currentRtssc == null)
						return null;

					bool previousAndCurrentAreSameRtss = currentRtssc.Equals(previous, ignoreChainage: true);

					currentRtssc.Direction = GetEncodedDirection(currentRtssc);

					// insert into the buffer the original RTSSC
					// not the current one which will possibly be updated with the most common direction in the buffer
					var originalRtssc = new Rtssc(currentRtssc);

					// insert the RTSSC into the buffer only if it is different from the previous RTSS or if the rounded 'chaînage' has changed
					if (previous == null || !previousAndCurrentAreSameRtss || Convert.ToInt32(originalRtssc.Chainage.GetValueOrDefault()) - Convert.ToInt32(previous.Chainage.GetValueOrDefault()) != 0)
						buffer.Enqueue(originalRtssc);

					if (buffer.Count > bufferSize)
						buffer.Dequeue();

					if (currentRtssc.Direction == DirectionBgr.Unknown)
					{
						if (previous != null)
						{
							if (previousAndCurrentAreSameRtss)
							{
								int compare = currentRtssc.Chainage.GetValueOrDefault().CompareTo(previous.Chainage.GetValueOrDefault());

								if (compare > 0)
									originalRtssc.Direction = DirectionBgr.ForwardChaining;
								else if (compare < 0)
									originalRtssc.Direction = DirectionBgr.BackwardChaining;
								else
									originalRtssc.Direction = DirectionBgr.Unknown;
							}

							currentRtssc.Direction =
								buffer
									.Where(r => r.Direction != DirectionBgr.Unknown && r.Equals(currentRtssc, ignoreChainage: true))
									.Select(data => (DirectionBgr) data.Direction)
									.GroupBy(d => d)
									.OrderByDescending(g => g.Count())
									.Select(g => g.Key)
									.FirstOrDefault();
						}
					}

					lastCoord = coord;
					previous = originalRtssc;

					return currentRtssc;
				};

			IObservable<IRtssc> output;

			if (allowSkipIfProcessing)
				output = gpsTrace.SkipIfProcessing(coord => Task.Run(() => convert(coord)));
			else
				output = gpsTrace.Select(convert);

			return output
				.Where(data => data != null)
				.Publish()
				.RefCount();
		}

		public static IObservable<Tuple<long, IRtssc, double?, double?>> GetItineraryFromBgrTrace(this IObservable<IRtssc> bgrTrace)
		{
			if (bgrTrace == null) throw new ArgumentNullException("bgrTrace");

			Func<IRtssc, long> getKey =
				rtssc => (long) Tuple.Create(rtssc.Route, rtssc.Troncon, rtssc.Section, rtssc.SousRoute, rtssc.Direction).GetHashCode();

			Tuple<long, IRtssc, double?, double?> current = null;

			return bgrTrace.Select(
				rtssc =>
				{
					var key = getKey(rtssc);

					if (current == null || key != current.Item1)
						current = Tuple.Create(key, rtssc, rtssc.Chainage, rtssc.Chainage);
					else
						current = Tuple.Create(key, rtssc, current.Item3, rtssc.Chainage);

					return current;
				});
		}

		public static IObservable<Tuple<long, IRtssc, double, double>> AutoCorrectItinerary(this IObservable<Tuple<long, IRtssc, double?, double?>> itinerary, double autoCorrectDelta)
		{
			if (itinerary == null) throw new ArgumentNullException("itinerary");

			Func<double?, double, double> adjust =
				(chainage, longueur) =>
				{
					double value = chainage ?? 0;

					if (value <= autoCorrectDelta)
						value = 0;
					else if (Math.Abs(value - longueur) <= autoCorrectDelta)
						value = longueur;

					return value;
				};

			return itinerary
				.Select(t => Tuple.Create(t.Item1, t.Item2, adjust(t.Item3, t.Item2.Longueur), adjust(t.Item4, t.Item2.Longueur)))
				.Where(t => Math.Abs(t.Item3 - t.Item4) > autoCorrectDelta);
		}

		public static IObservable<string> ConvertToItiFormat(this IObservable<Tuple<long, IRtssc, double, double>> itinerary)
		{
			if (itinerary == null) throw new ArgumentNullException("itinerary");

			return itinerary
				.GroupByUntilChanged(t => t.Item1)
				.Select(g => g.Item2.Last())
				.Select(
					t => string.Format("{0,1} {1} {2} {3} {4} {5,1} {6,6} {7,6}",
						(int) t.Item2.Direction,
						t.Item2.Route.PadLeft(5, '0'),
						t.Item2.Troncon.PadLeft(2, '0'),
						t.Item2.Section.PadLeft(3, '0'),
						t.Item2.SousRoute.PadLeft(4, '0'),
						t.Item2.Voie,
						Convert.ToInt32(Math.Min(t.Item3, t.Item4)),
						Convert.ToInt32((int) Math.Max(t.Item3, t.Item4))));
		}

		public static DirectionBgr GetEncodedDirection(IRtssc rtssc)
		{
			if (rtssc == null)
				return DirectionBgr.Unknown;

			if (rtssc.CodeSousRoute == "3" || EstCarrefourGiratoire(rtssc))
				return DirectionBgr.ForwardChaining;

			// driving on right side
			if (rtssc.CodeCoteChaussee == "D")
				return DirectionBgr.ForwardChaining;

			// driving on left side
			if (rtssc.CodeCoteChaussee == "G")
				return DirectionBgr.BackwardChaining;

			// by default, the direction remains unknown
			return DirectionBgr.Unknown;
		}

		public static bool EstRoute(IRtssc rtssc)
		{
			if (rtssc == null) throw new ArgumentNullException("rtssc");

			// I = chaussée créée par la présence d'un îlot séparateur ou déviateur
			return rtssc.CodeSousRoute == "0" || rtssc.CodeSousRoute == "I";
		}

		public static bool EstBretelle(IRtssc rtssc)
		{
			if (rtssc == null) throw new ArgumentNullException("rtssc");

			// bretelle = "3", "4" ou "V"
			// V = chaussée parallèle à une autoroute qui assure la circulation de transit
			return rtssc.CodeSousRoute == "3" || rtssc.CodeSousRoute == "4" || rtssc.CodeSousRoute == "V";
		}

		public static bool EstCarrefourGiratoire(IRtssc rtssc)
		{
			if (rtssc == null) throw new ArgumentNullException("rtssc");

			// carrefour = "G"
			return rtssc.CodeSousRoute == "G";
		}
	}
}