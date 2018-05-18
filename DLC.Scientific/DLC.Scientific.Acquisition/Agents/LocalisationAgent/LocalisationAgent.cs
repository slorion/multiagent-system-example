using DLC.Multiagent;
using DLC.Scientific.Acquisition.Agents.LocalisationAgent.Configuration;
using DLC.Scientific.Acquisition.Agents.LocalisationAgent.UI;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using DLC.Scientific.Acquisition.Core.Agents;
using DLC.Scientific.Acquisition.Core.Agents.Model;
using DLC.Scientific.Acquisition.Core.Configuration;
using DLC.Scientific.Core.Geocoding.Gps;
using DLC.Scientific.Core.Journalisation;
using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Agents.LocalisationAgent
{
	public class LocalisationAgent
		: AcquisitionableAgent<LocalisationProvider, LocalisationData, LocalisationAgentConfiguration, AcquisitionModuleConfiguration>, ILocalisationAgent, IVisibleAgent, IFileTransferAgent
	{
		private GpxTracer _tracer;

		public double GpsDistanceFromTriggerPoint { get; private set; }
		public int GpsFrequency { get { return this.Provider.Frequency; } }
		public bool UseOnlyTrustworthyData { get; set; }

		private bool Append { get; set; }

		protected override IEventJournal CreateEventJournal(InitializeRecordParameter parameters)
		{
			return null;
		}

		protected override void ConfigureAgent()
		{
			base.ConfigureAgent();

			this.AutoShowUI = this.Configuration.Agent.AutoShowUI;
			this.MainUITypeName = typeof(LocalisationUI).AssemblyQualifiedName;
			this.MainUIAgentTypeName = typeof(ILocalisationAgent).AssemblyQualifiedName;

			this.GpsDistanceFromTriggerPoint = this.Configuration.Agent.OffsetFromTriggerPoint;
			this.UseOnlyTrustworthyData = this.Configuration.Agent.UseOnlyTrustworthyData;

			this.Append = false;
			this.EventJournalFileExtension = "gpx";
			this.AgentUniversalName = "TraceAgent";
		}

		protected override async Task<AcquisitionActionResult> InitializeRecordCore(InitializeRecordParameter parameters, AcquisitionActionResult result)
		{
			result = await base.InitializeRecordCore(parameters, result).ConfigureAwait(false);

			if (this.UseOnlyTrustworthyData)
			{
				try { await this.DataSource.Where(data => data.GpsStatus != GpsStatus.Initializing).Take(1).ToTask(this.Provider.CurrentStateTransitionCancellationToken).ConfigureAwait(false); }
				catch (TaskCanceledException ex)
				{
					result.IsSuccessful = false;
					result.Exception = ex;
					return result;
				}
			}

			// create tracer & start tracing
			_tracer = new GpxTracer(this.EventJournalFileExtension, this.Append);

			var deviceType = this.CurrentData == null ? GpsDeviceType.Unknown : this.CurrentData.RawData.DeviceType;
			_tracer.StartTracing(parameters.SequenceId, this.JournalAbsoluteSavePath, deviceType);

			this.Provider.SequenceId = parameters.SequenceId;
			this.Provider.SaveFolderAbsolutePath = this.JournalAbsoluteSavePath;

			return result;
		}

		protected async override Task<AcquisitionActionResult> StartRecordCore(StartRecordParameter parameters, AcquisitionActionResult result)
		{
			result = await base.StartRecordCore(parameters, result).ConfigureAwait(false);
			if (!result.IsSuccessful)
				return result;

			this.RegisterObserver(
				this.DataSource
					.Where((data, i) => i % this.LogGap == 0)
					.Subscribe(data => LogGpsData(data)), AcquisitionStep.StopRecord);

			return result;
		}

		protected override async Task<AcquisitionActionResult> UninitializeRecordCore(UninitializeRecordParameter parameters, AcquisitionActionResult result)
		{
			if (_tracer != null)
			{
				lock (_tracer)
				{
					_tracer.StopTracing();
					_tracer.Dispose();
				}
			}

			return await base.UninitializeRecordCore(parameters, result).ConfigureAwait(false);
		}

		private void LogGpsData(LocalisationData data)
		{
			if (data == null) throw new ArgumentNullException("data");

			lock (_tracer)
			{
				if (!_tracer.IsClosed)
					_tracer.Trace(data.RawData, data.CorrectedData, data.GpsStatus, data.Timestamp);
			}
		}

		#region IFileTransferAgent members

		public IObservable<FileTransferData> FileTransferDataSource { get { return this.Provider.FileTransferDataSource; } }
		public bool IsTransferring { get { return this.Provider.IsTransferring; } }
		public IObservable<bool> IsTransferringDataSource { get { return this.Provider.IsTransferringDataSource; } }

		public void StartTransferring()
		{
		}

		public Task StopTransferring()
		{
			return this.Provider.CurrentFileTransfer ?? Task.FromResult(0);
		}

		#endregion
	}
}