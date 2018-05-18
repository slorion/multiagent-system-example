using DLC.Framework.Reactive;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using DLC.Scientific.Core.Geocoding;
using DLC.Scientific.Core.Geocoding.Bgr;
using DLC.Scientific.Core.Geocoding.Gps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Core.AcquisitionProviders
{
	public abstract class BgrProvider
		: AcquisitionProvider<BgrData>
	{
		public BgrProvider()
			: base()
		{
			this.AllowSkipIfProcessing = true;
		}

		public bool AllowSkipIfProcessing { get; set; }
		public BgrDataTypes AllowedBgrDataTypes { get; set; }
		public IObservable<LocalisationData> LocalisationDataSource { get; set; }

		public bool UseGpsTime { get; set; }
		public int MinSearchRadiusInMeters { get; set; }
		public int MaxSearchRadiusInMeters { get; set; }
		public int CurrentSearchRadiusInMeters { get; private set; }

		public int DirectionBufferSize { get; set; }

		public abstract GeoCoordinate GeoCodage(IRtssc rtssc);
		public abstract double GetSectionLength(IRtssc rtssc);
		public abstract IEnumerable<IRtssc> GetRtssFromRoute(string route);
		public abstract IEnumerable<string> SelectRoutes(GeoCoordinate coord, double searchRadius, int? maxRouteNumber);

		protected abstract IEnumerable<IRtssc> SelectRtssc(GeoCoordinate coord, double searchRadiusInMeters, DateTime? date);
		protected abstract IRtssc ObtenirRtsscSuivant(GeoCoordinate coord, double searchRadiusInMeters, DirectionBgr direction);

		protected override void ValidateConfigurationCore()
		{
			base.ValidateConfigurationCore();

			if (this.LocalisationDataSource == null) throw new InvalidOperationException("LocalisationDataSource is mandatory.");
			if (this.MinSearchRadiusInMeters < 0) throw new InvalidOperationException("MinSearchRadiusInMeters must be greater than or equal to 0.");
			if (this.MaxSearchRadiusInMeters < 0) throw new InvalidOperationException("MaxSearchRadiusInMeters must be greater than or equal to 0.");
			if (this.MaxSearchRadiusInMeters < this.MinSearchRadiusInMeters) throw new InvalidOperationException("MaxSearchRadiusInMeters must be greater than or equal to MinSearchRadius.");
			if (this.DirectionBufferSize < 0) throw new InvalidOperationException("DirectionBufferSize must be greater than or equal to 0.");
		}

		protected override Task<IObservable<BgrData>> InitializeCore()
		{
			this.CurrentSearchRadiusInMeters = this.MinSearchRadiusInMeters;

			return Task.Run(
				() => this.LocalisationDataSource.Select(data => data.CorrectedData.PositionData)
					.GetBgrTraceFromGps(this.DirectionBufferSize, this.AllowedBgrDataTypes, this.AllowSkipIfProcessing, GeoCodageInverse)
					.Select(rtssc => new BgrData { Rtssc = rtssc, Direction = rtssc.Direction }));
		}

		public IRtssc GetNextRtsscSameDirection(GeoCoordinate coord)
		{
			if (coord == null) throw new ArgumentNullException("coord");

			var direction = this.CurrentData == null ? DirectionBgr.Unknown : this.CurrentData.Direction;

			//TODO: le 40,000 sort d'où ?
			return ObtenirRtsscSuivant(coord, 40000, direction);
		}

		protected IEnumerable<IRtssc> ApplyActiveFilters(IEnumerable<IRtssc> rtsscCollection)
		{
			return rtsscCollection
				.Where(
					rtssc => {
						if (RtsscHelper.EstRoute(rtssc))
							return this.AllowedBgrDataTypes.HasFlag(BgrDataTypes.Routes);
						else if (RtsscHelper.EstBretelle(rtssc))
							return this.AllowedBgrDataTypes.HasFlag(BgrDataTypes.Bretelles);
						else if (RtsscHelper.EstCarrefourGiratoire(rtssc))
							return this.AllowedBgrDataTypes.HasFlag(BgrDataTypes.CarrefoursGiratoires);
						else
							return false;
					});
		}

		protected IRtssc GeoCodageInverse(GeoCoordinate coord)
		{
			if (coord == null) throw new ArgumentNullException("coord");

			return GeoCodageInverse(coord, Enumerable.Empty<IRtssc>());
		}

		private IRtssc GeoCodageInverse(GeoCoordinate coord, IEnumerable<IRtssc> buffer)
		{
			if (coord == null) throw new ArgumentNullException("coord");
			if (buffer == null) throw new ArgumentNullException("buffer");

			IRtssc rtssc;
			do
			{
				var candidates = SelectRtssc(coord, this.CurrentSearchRadiusInMeters, this.UseGpsTime ? ((PositionData) coord).Utc : DateTime.Now);

				if (!candidates.Skip(1).Any())
					rtssc = candidates.FirstOrDefault();
				else
				{
					rtssc =
						candidates
							.GroupBy(c => c.Route)
							.OrderByDescending(
								g => buffer.Concat(new[] { candidates.First() }).Count(r => r.Route == g.Key))
							.Select(g => g.FirstOrDefault())
							.FirstOrDefault();
				}

				if (rtssc == null)
					this.CurrentSearchRadiusInMeters = Math.Min(this.CurrentSearchRadiusInMeters * 2, this.MaxSearchRadiusInMeters + 1);
			} while (rtssc == null && this.CurrentSearchRadiusInMeters <= this.MaxSearchRadiusInMeters);

			return rtssc;
		}
	}
}