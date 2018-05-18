using DLC.Framework;
using DLC.Multiagent;
using DLC.Scientific.Acquisition.Agents.DistanceAgent.UI;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using DLC.Scientific.Acquisition.Core.Agents;
using DLC.Scientific.Acquisition.Core.Agents.Model;
using DLC.Scientific.Acquisition.Core.Configuration;
using DLC.Scientific.Core.Journalisation;
using DLC.Scientific.Core.Journalisation.Journals;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Agents.DistanceAgent
{
	public class DistanceAgent
		: AcquisitionableAgent<DistanceProvider, DistanceData, AcquisitionAgentConfiguration, AcquisitionModuleConfiguration>, IDistanceAgent, IVisibleAgent
	{
		public override int Priority { get { return 70; } }

		protected override IEventJournal CreateEventJournal(InitializeRecordParameter parameters)
		{
			var journal = new DistanceAgentEventJournal();
			journal.JournalHeader.ActiveMode = "AbsoluteTime";

			journal.JournalHeader.Ppkm = this.Provider.EncoderNumber == 1 ? this.Provider.PPKMLeft : this.Provider.EncoderNumber == 2 ? this.Provider.PPKMRight : -1;

			return journal;
		}

		protected override void ConfigureAgent()
		{
			base.ConfigureAgent();

			this.AutoShowUI = this.Configuration.Agent.AutoShowUI;
			this.MainUITypeName = typeof(CalibrationUI).AssemblyQualifiedName;
			this.MainUIAgentTypeName = typeof(IDistanceAgent).AssemblyQualifiedName;
			this.AgentUniversalName = "DistanceAgent";

			this.DeviceDistanceFromStartTriggerPoint = this.Configuration.Agent.Journalisation.DeviceDistanceFromStartTriggerPoint;
			this.DeviceDistanceFromStopTriggerPoint = this.Configuration.Agent.Journalisation.DeviceDistanceFromStopTriggerPoint;
		}

		protected override async Task<AcquisitionActionResult> StartRecordCore(StartRecordParameter parameters, AcquisitionActionResult result)
		{
			result = await base.StartRecordCore(parameters, result).ConfigureAwait(false);
			if (!result.IsSuccessful)
				return result;

			this.RegisterObserver(
				this.DataSource
					.Where((data, i) => i % this.LogGap == 0)
					.Subscribe(data => AddEventJournalEntry(CreateEjxJournalEntry(data))), AcquisitionStep.StopRecord);

			return result;
		}

		private DistanceAgentEventJournalEntry CreateEjxJournalEntry(DistanceData distanceData)
		{
			return new DistanceAgentEventJournalEntry {
				Comment = "Distance",
				DateTime = DateTimePrecise.Now,
				Progress = Convert.ToInt32(distanceData.AbsoluteDistance - this.DeviceDistanceFromStartTriggerPoint),
				AbsoluteTime = distanceData.Timestamp,
				AbsoluteElapsedTime = -1, // not used
				RelativeElapsedTime = -1, // not used
			};
		}

		#region IDistanceAgent members

		public int ReferenceEncoderNumber { get { return this.Provider.EncoderNumber; } }
		public int PPKMLeft { get { return this.Provider.PPKMLeft; } }
		public int PPKMRight { get { return this.Provider.PPKMRight; } }
		public int IntervalLength { get { return this.Provider.IntervalLength; } }

		#endregion

		#region ICalibrableAgent members

		public IObservable<CalibrationData> CalibrationDataSource { get { return this.Provider.CalibrationDataSource; } }

		public Task StartCalibration()
		{
			return this.Provider.StartCalibration();
		}

		public Task<CalibrationData> StopCalibration()
		{
			return this.Provider.StopCalibration();
		}

		public async Task<Exception> ProcessCalibrationData(bool isAccepted, CalibrationData calibrationData)
		{
			var result = await this.Provider.ProcessCalibrationData(isAccepted, calibrationData).ConfigureAwait(false);

			if (isAccepted)
			{
				AcquisitionConfigurationFactory.Instance.Update(this.ConfigurationFilePath, string.Format("Module.Providers.{0}.PPKMLeft", this.Configuration.Module.ActiveProviderName), ((DistanceCalibrationData) calibrationData).PpkmLeft);
				AcquisitionConfigurationFactory.Instance.Update(this.ConfigurationFilePath, string.Format("Module.Providers.{0}.PPKMRight", this.Configuration.Module.ActiveProviderName), ((DistanceCalibrationData) calibrationData).PpkmRight);
			}

			return result;
		}

		#endregion
	}
}