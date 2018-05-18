using DLC.Framework.Reactive;
using DLC.Multiagent;
using DLC.Scientific.Acquisition.Agents.SpeedAgent.Configuration;
using DLC.Scientific.Acquisition.Agents.SpeedAgent.UI;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using DLC.Scientific.Acquisition.Core.Agents;
using DLC.Scientific.Acquisition.Core.Agents.Model;
using DLC.Scientific.Acquisition.Core.Configuration;
using DLC.Scientific.Acquisition.Modules.SpeedModule;
using DLC.Scientific.Core.Journalisation;
using DLC.Scientific.Core.Journalisation.Journals;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Agents.SpeedAgent
{
	public class SpeedAgent
		: AcquisitionableAgent<SpeedProvider, SpeedData, SpeedAgentConfiguration, AcquisitionModuleConfiguration>, ISpeedAgent, IVisibleAgent
	{
		private readonly SubjectSlim<DistanceData> _distanceSubject = new SubjectSlim<DistanceData>();
		private readonly SubjectSlim<LocalisationData> _localisationSubject = new SubjectSlim<LocalisationData>();

		public DistanceAcquisitionMode AcquisitionMode { get; set; }

		public TimeSpan SavingInterval { get; set; }

		protected override IEventJournal CreateEventJournal(InitializeRecordParameter parameters)
		{
			var journal = new SpeedAgentEventJournal();

			var calculated = this.Provider as CalculatedSpeedProvider;
			if (calculated != null)
			{
				journal.JournalHeader.BorneInferieure = calculated.Threshold - calculated.Hysteresis;
				journal.JournalHeader.BorneSuperieure = calculated.Threshold + calculated.Hysteresis;
				journal.JournalHeader.Tolerance = calculated.Hysteresis;
			}

			return journal;
		}

		private SpeedAgentEventJournalEntry CreateEjxJournalEntry(SpeedData data)
		{
			return new SpeedAgentEventJournalEntry {
				Comment = "Speed",
				DateTime = data.Timestamp,
				Progress = (int) (data.CurrentDistance == null ? -1 : data.CurrentDistance - this.DeviceDistanceFromStartTriggerPoint),
				VitesseGps = data.GpsSpeed.GetValueOrDefault(-1),
				VitesseDistance = data.DistanceSpeed.GetValueOrDefault(-1),
				ActiveMode = data.SpeedSource.ToString()
			};
		}

		protected override SpeedProvider CreateAndConfigureProvider()
		{
			var calculated = this.Configuration.Module.Provider as CalculatedSpeedProvider;
			if (calculated != null)
			{
				calculated.DistanceDataSource = _distanceSubject;
				calculated.IsDistanceOperationalCallback = IsDependencyOperational<IDistanceAgent>;
				calculated.LocalisationDataSource = _localisationSubject;
				calculated.IsLocalisationOperationalCallback = IsDependencyOperational<ILocalisationAgent>;
			}

			return (SpeedProvider) this.Configuration.Module.Provider;
		}

		protected override void ConfigureAgent()
		{
			base.ConfigureAgent();

			this.AutoShowUI = this.Configuration.Agent.AutoShowUI;
			this.MainUITypeName = typeof(SpeedUI).AssemblyQualifiedName;
			this.MainUIAgentTypeName = typeof(ISpeedAgent).AssemblyQualifiedName;
			this.AgentUniversalName = "SpeedAgent";

			this.AcquisitionMode = this.Configuration.Agent.AcquisitionMode;
			this.SavingInterval = TimeSpan.FromMilliseconds(this.Configuration.Agent.AcquisitionTimeSavingIntervalInMs);
		}

		protected override async Task<AcquisitionActionResult> StartCore(StartAcquisitionParameter parameters, AcquisitionActionResult result)
		{
			result = await base.StartCore(parameters, result).ConfigureAwait(false);
			if (!result.IsSuccessful)
				return result;

			this.RegisterObserver(AgentBroker.Instance.ObserveAny<ILocalisationAgent, LocalisationData>("DataSource").Subscribe(data => _localisationSubject.OnNext(data)));
			this.RegisterObserver(AgentBroker.Instance.ObserveAny<IDistanceAgent, DistanceData>("DataSource").Subscribe(data => _distanceSubject.OnNext(data)));

			return result;
		}

		protected override async Task<AcquisitionActionResult> StartRecordCore(StartRecordParameter parameters, AcquisitionActionResult result)
		{
			result = await base.StartRecordCore(parameters, result).ConfigureAwait(false);
			if (!result.IsSuccessful)
				return result;

			switch (this.AcquisitionMode)
			{
				case DistanceAcquisitionMode.Time:
					this.RegisterObserver(
						this.DataSource
							.Sample(this.SavingInterval)
							.Buffer(this.SavingInterval, 1)
							.Select(buffer => buffer.Count > 0 ? buffer[0] : new SpeedData { CurrentSpeed = -1, SpeedSource = SpeedActiveMode.None })
							.Subscribe(data => AddEventJournalEntry(CreateEjxJournalEntry(data))), AcquisitionStep.StopRecord);
					break;
				case DistanceAcquisitionMode.Distance:
					this.RegisterObserver(this.DataSource
						.Where(data => data.CurrentDistance != null)
						.DistinctUntilChanged() // only for new values
						.Subscribe(data => AddEventJournalEntry(CreateEjxJournalEntry(data))), AcquisitionStep.StopRecord);
					break;
				default:
					throw new NotSupportedException(string.Format("Acquisition mode '{0}' is not supported.", this.AcquisitionMode));
			}

			return result;
		}

		protected override void SetupAgentOperationalCommunicationsCore()
		{
			base.SetupAgentOperationalCommunicationsCore();

			TrackDependencyOperationalState<IDistanceAgent>(false);
			TrackDependencyOperationalState<ILocalisationAgent>(false);
		}
	}
}