using DLC.Framework;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Modules.TriggerModule
{
	public class TriggerSimulator : TriggerProvider
	{
		public int FrequencyInMs { get; set; }
		private bool _armed = false;
		private TriggerMode _armTriggerMode;

		protected override void ValidateConfigurationCore()
		{
			base.ValidateConfigurationCore();

			if (this.FrequencyInMs < 1)
				throw new InvalidOperationException("FrequencyInMs must be greater than 0.");
		}

		protected override Task<IObservable<TriggerData>> InitializeCore()
		{
			return Task.FromResult(
				Observable.Timer(DateTimeOffset.Now, TimeSpan.FromMilliseconds(this.FrequencyInMs))
				.Where(data => _armed)
					.Select(ticks => new TriggerData { TriggerMode = _armTriggerMode, Timestamp = DateTimePrecise.Now }));
		}

		protected override Task SetStartTriggerStateCore(bool armed)
		{
			_armed = armed;
			_armTriggerMode = TriggerMode.Start;
			return Task.FromResult(0);
		}

		protected override Task SetStopTriggerStateCore(bool armed)
		{
			_armed = armed;
			_armTriggerMode = TriggerMode.Stop;
			return Task.FromResult(0);
		}
	}
}