using DLC.Framework;
using DLC.Framework.Reactive;
using DLC.Multiagent;
using DLC.Scientific.Acquisition.Agents.EventPanelAgent.Configuration;
using DLC.Scientific.Acquisition.Agents.EventPanelAgent.UI;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using DLC.Scientific.Acquisition.Core.Agents;
using DLC.Scientific.Acquisition.Core.Agents.Model;
using DLC.Scientific.Acquisition.Core.Configuration;
using DLC.Scientific.Core.Journalisation;
using DLC.Scientific.Core.Journalisation.Journals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DLC.Scientific.Acquisition.Agents.EventPanelAgent
{
	public class EventPanelAgent
		: AcquisitionableAgent<ManualProvider<RoadEventData>, RoadEventData, EventPanelAgentConfiguration, AcquisitionModuleConfiguration>, IEventPanelAgent, IVisibleAgent, IInternalEventPanelAgent
	{
		private readonly BehaviorSubjectSlim<bool> _hotkeyModeEnabledSubject = new BehaviorSubjectSlim<bool>(false);

		private int _roadEventSequence = 0;

		private Orientation _uiOrientation;
		private int _splitterDistanceHorizontalMode;
		private int _splitterDistanceVerticalMode;

		public IObservable<bool> HotkeyModeEnabledDataSource { get { return _hotkeyModeEnabledSubject.DistinctUntilChanged(); } }

		protected override IEventJournal CreateEventJournal(InitializeRecordParameter parameters)
		{
			return new EventPanelAgentEventJournal();
		}

		protected override ManualProvider<RoadEventData> CreateAndConfigureProvider()
		{
			return new ManualProvider<RoadEventData>();
		}

		protected override void ConfigureAgent()
		{
			base.ConfigureAgent();

			this.AutoShowUI = this.Configuration.Agent.AutoShowUI;
			this.MainUITypeName = typeof(EventPanelUI).AssemblyQualifiedName;
			this.MainUIAgentTypeName = typeof(IInternalEventPanelAgent).AssemblyQualifiedName;
			this.AgentUniversalName = "EventPanelAgent";

			_uiOrientation = this.Configuration.Agent.UIOrientation;
			_splitterDistanceHorizontalMode = this.Configuration.Agent.SplitterDistanceHorizontalMode;
			_splitterDistanceVerticalMode = this.Configuration.Agent.SplitterDistanceVerticalMode;

			this.ToggleHotkeyModeKey = (Keys) new KeysConverter().ConvertFromString(this.Configuration.Agent.ToggleHotkeyModeKey);
		}

		protected override void SetupAgentOperationalCommunicationsCore()
		{
			base.SetupAgentOperationalCommunicationsCore();

			this.TrackDependencyOperationalState<IDistanceAgent>(false);
			this.TrackDependencyOperationalState<ILocalisationAgent>(false);
		}

		protected async override Task<AcquisitionActionResult> StartRecordCore(StartRecordParameter parameters, AcquisitionActionResult result)
		{
			result = await base.StartRecordCore(parameters, result).ConfigureAwait(false);
			if (!result.IsSuccessful)
				return result;

			_roadEventSequence = 0;

			// we stop recording at UninitializeReocrd, not StopRecord
			// to allow operators to add comments before they validate the acquisition
			this.RegisterObserver(
				this.DataSource.Subscribe(
					data =>
					{
						if (data.IsJournalised)
							AddEventJournalEntry(CreateEjxJournalEntry(data));
					}), AcquisitionStep.UninitializeRecord);

			return result;
		}

		private EventPanelAgentEventJournalEntry CreateEjxJournalEntry(RoadEventData data)
		{
			var gps = data.Localisation == null ? null : data.Localisation.PositionData;

			return new EventPanelAgentEventJournalEntry {
				Id = data.Id,
				Comment = data.Comment,
				DateTime = data.Timestamp,
				EventState = "Start", // for backward compatibility with old journal format
				EventType = data.EventType,
				IsSnapShot = data.IsSnapshot,
				ManualCorrection = data.DistanceManualCorrection,
				Progress = data.Distance - this.DeviceDistanceFromStartTriggerPoint,
				Severity = data.Severity,
				AbsoluteDistance = data.Distance,
				Latitude = gps == null ? null : (double?) gps.Latitude,
				Longitude = gps == null ? null : (double?) gps.Longitude,
				Altitude = gps == null ? null : (double?) gps.Altitude,
				GpsTime = gps == null ? null : (DateTime?) gps.Utc
			};
		}

		private async Task<Tuple<DistanceData, LocalisationData>> GetCurrentPosition()
		{
			if (this.ProviderState < ProviderState.StartedRecord)
				return Tuple.Create((DistanceData) null, (LocalisationData) null);
			else
			{
				DistanceData distance = await AgentBroker.Instance.TryExecuteOnFirst<IDistanceAgent, DistanceData>(a => a.CurrentData).GetValueOrDefault().ConfigureAwait(false);
				LocalisationData localisation = await AgentBroker.Instance.TryExecuteOnFirst<ILocalisationAgent, LocalisationData>(a => a.CurrentData).GetValueOrDefault().ConfigureAwait(false);

				return Tuple.Create(distance, localisation);
			}
		}

		#region IInternalEventPanelAgent

		public Orientation UIOrientation
		{
			get { return _uiOrientation; }
			set
			{
				_uiOrientation = value;
				AcquisitionConfigurationFactory.Instance.Update(this.ConfigurationFilePath, "Agent.UIOrientation", value.ToString());
			}
		}

		public int SplitterDistanceHorizontalMode
		{
			get { return _splitterDistanceHorizontalMode; }
			set
			{
				_splitterDistanceHorizontalMode = value;
				AcquisitionConfigurationFactory.Instance.Update(this.ConfigurationFilePath, "Agent.SplitterDistanceHorizontalMode", value.ToString());
			}
		}

		public int SplitterDistanceVerticalMode
		{
			get { return _splitterDistanceVerticalMode; }
			set
			{
				_splitterDistanceVerticalMode = value;
				AcquisitionConfigurationFactory.Instance.Update(this.ConfigurationFilePath, "Agent.SplitterDistanceVerticalMode", value.ToString());
			}
		}

		public Keys ToggleHotkeyModeKey { get; set; }

		public void OnHotkeyModeChanged(bool enabled)
		{
			_hotkeyModeEnabledSubject.OnNext(enabled);
		}

		public async Task OnRoadEvent(RoadEventData data, bool isNew = true, double? progress = null)
		{
			if (data == null) throw new ArgumentNullException("data");

			if (isNew)
			{
				data.Id = Interlocked.Increment(ref _roadEventSequence);

				var position = await GetCurrentPosition().ConfigureAwait(false);

				if (progress.HasValue)
					data.Distance = progress.Value;
				else if (position.Item1 != null)
					data.Distance = position.Item1.AbsoluteDistance;

				if (position.Item2 != null)
					data.Localisation = position.Item2.CorrectedData;
			}

			this.Provider.OnNext(data);
		}

		public IEnumerable<RoadEvent> ReadRoadEventConfiguration()
		{
			return this.Configuration.Agent.RoadEvents;
		}

		public RoadEventData CloneRoadEvent(RoadEventData data)
		{
			if (data == null) throw new ArgumentNullException("data");

			RoadEventData clone;

			var serializer = new DataContractSerializer(typeof(RoadEventData));
			using (var ms = new MemoryStream())
			{
				serializer.WriteObject(ms, data);
				ms.Position = 0;
				clone = (RoadEventData) serializer.ReadObject(ms);
			}

			clone.Timestamp = DateTimePrecise.Now;

			return clone;
		}

		#endregion
	}
}