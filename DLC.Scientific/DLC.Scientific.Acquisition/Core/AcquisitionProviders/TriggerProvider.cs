using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Core.AcquisitionProviders
{
	public abstract class TriggerProvider
		: AcquisitionProvider<TriggerData>
	{
		[Flags]
		private enum TriggerStates
		{
			None = 0,
			Start = 1,
			Stop = 2
		}

		private TriggerStates _triggerState = TriggerStates.None;

		public bool IsArmed { get { return _triggerState != TriggerStates.None; } }

		public async Task SetStartTriggerState(bool armed)
		{
			await SetStartTriggerStateCore(armed).ConfigureAwait(false);

			if (armed)
				_triggerState |= TriggerStates.Start;
			else
				_triggerState &= ~TriggerStates.Start;
		}

		protected abstract Task SetStartTriggerStateCore(bool armed);

		public async Task SetStopTriggerState(bool armed)
		{
			await SetStopTriggerStateCore(armed).ConfigureAwait(false);

			if (armed)
				_triggerState |= TriggerStates.Stop;
			else
				_triggerState &= ~TriggerStates.Stop;
		}

		protected abstract Task SetStopTriggerStateCore(bool armed);

		protected override IObservable<TriggerData> CreateDataSource(IObservable<TriggerData> rawDataSource)
		{
			return base.CreateDataSource(rawDataSource)
				.Select(
					data =>
					{
						data.TriggerMode = _triggerState.HasFlag(TriggerStates.Start) ? TriggerMode.Start : TriggerMode.Stop;

#pragma warning disable 4014
						if (data.TriggerMode == TriggerMode.Start)
							this.SetStartTriggerState(false);
						else
							this.SetStopTriggerState(false);
#pragma warning restore 4014

						return data;
					}).Publish().RefCount();
		}
	}
}