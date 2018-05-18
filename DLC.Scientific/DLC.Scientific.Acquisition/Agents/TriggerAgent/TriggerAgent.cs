using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using DLC.Scientific.Acquisition.Core.Agents;
using DLC.Scientific.Acquisition.Core.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Agents.TriggerAgent
{
	public class TriggerAgent
		: ProviderAgent<TriggerProvider, TriggerData, AcquisitionAgentConfiguration, AcquisitionModuleConfiguration>, ITriggerAgent
	{
		private TaskCompletionSource<TriggerData> _waitForTriggerTcs;

		public async Task WaitForTrigger(TriggerMode mode)
		{
			var newTcs = new TaskCompletionSource<TriggerData>();

			var currentTcs = Interlocked.CompareExchange(ref _waitForTriggerTcs, newTcs, null);
			if (currentTcs != null)
				await currentTcs.Task.ConfigureAwait(false);
			else
			{
				using (
					this.Provider.DataSource.Subscribe(
						data =>
						{
							if (data.TriggerMode == mode)
								newTcs.SetResult(data);
							else
								newTcs.SetException(new InvalidOperationException(string.Format("Received TriggerMode ('{0}') was not the one expected ('{1}')", data.TriggerMode, mode)));
						}))
				{
					switch (mode)
					{
						case TriggerMode.Start:
							await this.Provider.SetStartTriggerState(true).ConfigureAwait(false);
							break;
						case TriggerMode.Stop:
							await this.Provider.SetStopTriggerState(true).ConfigureAwait(false);
							break;
						default:
							throw new NotSupportedException(string.Format("TriggerMode '{0}' is not supported.", mode));
					}

					await newTcs.Task.ContinueWith(t => _waitForTriggerTcs = null).ConfigureAwait(false);
				}
			}
		}

		public Task CancelWaitForTrigger(TriggerMode mode)
		{
			var currentTcs = _waitForTriggerTcs;

			if (currentTcs == null)
				throw new InvalidOperationException(string.Format("Not currently waiting on a trigger with mode equal to '{0}'.", mode));

			currentTcs.SetCanceled();

			switch (mode)
			{
				case TriggerMode.Start:
					return this.Provider.SetStartTriggerState(false).ContinueWith(t => _waitForTriggerTcs = null);
				case TriggerMode.Stop:
					return this.Provider.SetStopTriggerState(false).ContinueWith(t => _waitForTriggerTcs = null);
				default:
					throw new NotSupportedException(string.Format("TriggerMode '{0}' is not supported.", mode));
			}
		}
	}
}