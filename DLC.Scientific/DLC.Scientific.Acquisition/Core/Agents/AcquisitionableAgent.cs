using DLC.Framework;
using DLC.Framework.IO;
using DLC.Multiagent;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using DLC.Scientific.Acquisition.Core.Agents.Model;
using DLC.Scientific.Acquisition.Core.Configuration;
using DLC.Scientific.Core.Journalisation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Core.Agents
{
	public abstract class AcquisitionableAgent<TProvider, TData, TAgentConfiguration, TModuleConfiguration>
		: ProviderAgent<TProvider, TData, TAgentConfiguration, TModuleConfiguration>, IAcquisitionableAgent
		where TProvider : AcquisitionProvider<TData>
		where TData : ProviderData
		where TAgentConfiguration : AcquisitionAgentConfiguration
		where TModuleConfiguration : AcquisitionModuleConfiguration
	{
		private const string DefaultConfigurationJournalFileExtension = ".cjx";
		private const string DefaultEventJournalFileExtension = ".ejx";
		private const string DefaultFileJournalFileExtension = ".fjx";

		private JournalBufferedRecorder _eventJournalRecorder;
		private readonly List<FileJournalEntry> _fileJournalEntries = new List<FileJournalEntry>();

		public AcquisitionableAgent()
		{
			this.ConfigurationJournalFileExtension = DefaultConfigurationJournalFileExtension;
			this.EventJournalFileExtension = DefaultEventJournalFileExtension;
			this.FileJournalFileExtension = DefaultFileJournalFileExtension;
		}

		protected override void SetupAgentOperationalCommunicationsCore()
		{
			base.SetupAgentOperationalCommunicationsCore();

			if (this.DeviceDistanceFromStartTriggerPoint > 0 || this.DeviceDistanceFromStopTriggerPoint > 0)
				TrackDependencyOperationalState<IDistanceAgent>(isMandatory: false, canFailover: true);
		}

		public double DeviceDistanceFromStartTriggerPoint { get; protected set; }
		public double DeviceDistanceFromStopTriggerPoint { get; protected set; }
		public int LogGap { get; protected set; }
		public virtual int Priority { get { return 50; } }

		protected IEventJournal EventJournal { get; private set; }

		public string AgentFolderPath { get; protected set; }
		public string AgentUniversalName { get; protected set; }

		public string SequenceId { get; private set; }
		public string RootPath { get; protected set; }
		public string JournalRootPath { get; private set; }
		public string JournalAbsoluteSavePath { get; private set; }
		public string JournalRelativeSavePath { get; private set; }

		public string ConfigurationJournalRelativePath { get; private set; }
		public string EventJournalRelativePath { get; private set; }
		public string FileJournalRelativePath { get; private set; }

		public string ConfigurationJournalFileExtension { get; protected set; }
		public string EventJournalFileExtension { get; protected set; }
		public string FileJournalFileExtension { get; protected set; }

		protected override void ConfigureAgent()
		{
			base.ConfigureAgent();

			this.RootPath = this.Configuration.Agent.Journalisation.RootPath;
			this.DeviceDistanceFromStartTriggerPoint = this.Configuration.Agent.Journalisation.DeviceDistanceFromStartTriggerPoint;
			this.DeviceDistanceFromStopTriggerPoint = this.Configuration.Agent.Journalisation.DeviceDistanceFromStopTriggerPoint;
			this.LogGap = this.Configuration.Agent.Journalisation.LogGap;
			this.AgentFolderPath = this.Configuration.Agent.Journalisation.AgentFolderPath;
		}

		protected override async Task<bool> DeactivateCore()
		{
			AcquisitionActionResult result = null;
			if (this.ProviderState >= ProviderState.StartingRecord)
				result = await this.StopRecord(new StopRecordParameter() { SequenceId = this.SequenceId, TriggerMode = AcquisitionTriggerMode.Unknown }).ConfigureAwait(false);

			if ((result == null || result.IsSuccessful) && (this.ProviderState >= ProviderState.InitializingRecord))
				result = await this.UninitializeRecord(new UninitializeRecordParameter() { SequenceId = this.SequenceId }).ConfigureAwait(false);

			return await base.DeactivateCore().ConfigureAwait(false) && (result == null || result.IsSuccessful);
		}

		public Task<AcquisitionActionResult> InitializeRecord(InitializeRecordParameter parameters)
		{
			return MakeStateTransition(AcquisitionStep.InitializeRecord, this.Provider.InitializeRecord, parameters, InternalInitializeRecord);
		}
		protected virtual Task<AcquisitionActionResult> InitializeRecordCore(InitializeRecordParameter parameters, AcquisitionActionResult result) { return Task.FromResult(result); }

		public Task<AcquisitionActionResult> StartRecord(StartRecordParameter parameters)
		{
			return MakeStateTransition(AcquisitionStep.StartRecord, this.Provider.StartRecord, parameters, InternalStartRecord);
		}
		protected virtual Task<AcquisitionActionResult> StartRecordCore(StartRecordParameter parameters, AcquisitionActionResult result) { return Task.FromResult(result); }

		public Task<AcquisitionActionResult> StopRecord(StopRecordParameter parameters)
		{
			return MakeStateTransition(AcquisitionStep.StopRecord, this.Provider.StopRecord, parameters, InternalStopRecord);
		}
		protected virtual Task<AcquisitionActionResult> StopRecordCore(StopRecordParameter parameters, AcquisitionActionResult result) { return Task.FromResult(result); }

		public Task<AcquisitionActionResult> UninitializeRecord(UninitializeRecordParameter parameters)
		{
			return MakeStateTransition(AcquisitionStep.UninitializeRecord, this.Provider.UninitializeRecord, parameters, InternalUninitializeRecord);
		}
		protected virtual Task<AcquisitionActionResult> UninitializeRecordCore(UninitializeRecordParameter parameters, AcquisitionActionResult result) { return Task.FromResult(result); }

		private async Task<AcquisitionActionResult> InternalInitializeRecord(InitializeRecordParameter parameters, AcquisitionActionResult result)
		{
			if (string.IsNullOrEmpty(parameters.DefaultRootPath)) throw new InvalidOperationException("RootPath is mandatory.");
			if (string.IsNullOrEmpty(this.AgentFolderPath)) throw new InvalidOperationException("AgentFolderPath is mandatory.");
			if (string.IsNullOrEmpty(this.AgentUniversalName)) throw new InvalidOperationException("AgentUniversalName is mandatory.");
			if (string.IsNullOrEmpty(this.EventJournalFileExtension)) throw new InvalidOperationException("EventJournalFileExtension is mandatory.");
			if (string.IsNullOrEmpty(this.FileJournalFileExtension)) throw new InvalidOperationException("FileJournalFileExtension is mandatory.");
			if (string.IsNullOrEmpty(parameters.SequenceId)) throw new InvalidOperationException("SequenceId is mandatory.");

			_fileJournalEntries.Clear();

			this.JournalRootPath = string.IsNullOrEmpty(this.RootPath) ? parameters.DefaultRootPath : this.RootPath;

			this.SequenceId = parameters.SequenceId;
			this.Provider.SequenceId = parameters.SequenceId;

			this.JournalRelativeSavePath = this.AgentFolderPath;
			this.JournalAbsoluteSavePath = Path.Combine(this.JournalRootPath, parameters.SequenceId, this.JournalRelativeSavePath);

			int rootLength = 1 + Path.Combine(this.JournalRootPath, parameters.SequenceId).Length;
			string filename = string.IsNullOrWhiteSpace(this.Configuration.Agent.Journalisation.CharacterizationFileName) ? this.AgentUniversalName : string.Format("{0}.{1}", this.Configuration.Agent.Journalisation.CharacterizationFileName, this.AgentUniversalName);
			string fjxCompleteFilePath = Path.Combine(this.JournalAbsoluteSavePath, parameters.SequenceId + "." + filename + this.FileJournalFileExtension);
			this.FileJournalRelativePath = fjxCompleteFilePath.Substring(rootLength);

			string ejxCompleteFilePath = InitializeEventJournal(this.JournalAbsoluteSavePath, parameters);
			this.EventJournalRelativePath = string.IsNullOrEmpty(ejxCompleteFilePath) ? string.Empty : ejxCompleteFilePath.Substring(rootLength);

			string cjxCompleteFilePath = InitializeConfigurationJournal(this.JournalAbsoluteSavePath, parameters);
			this.ConfigurationJournalRelativePath = string.IsNullOrEmpty(cjxCompleteFilePath) ? string.Empty : cjxCompleteFilePath.Substring(rootLength);

			result.ConfigurationJournalRelativePath = this.ConfigurationJournalRelativePath;
			result.EventJournalRelativePath = this.EventJournalRelativePath;
			result.FileJournalRelativePath = this.FileJournalRelativePath;

			this.Provider.SaveFolderAbsolutePath = this.JournalAbsoluteSavePath;

			result = await InitializeRecordCore(parameters, result).ConfigureAwait(false);

			return result;
		}

		private async Task<AcquisitionActionResult> InternalStartRecord(StartRecordParameter parameters, AcquisitionActionResult result)
		{
			// delayed start
			if (this.DeviceDistanceFromStartTriggerPoint > 0 && this.IsDependencyOperational<IDistanceAgent>())
				await AgentBroker.Instance.ObserveAny<IDistanceAgent, DistanceData>("DataSource").Where(data => data.AbsoluteDistance >= this.DeviceDistanceFromStartTriggerPoint).Take(1).ToTask().ConfigureAwait(false);

			result = await StartRecordCore(parameters, result).ConfigureAwait(false);

			return result;
		}

		private async Task<AcquisitionActionResult> InternalStopRecord(StopRecordParameter parameters, AcquisitionActionResult result)
		{
			// delayed stop
			if (this.DeviceDistanceFromStopTriggerPoint > 0 && this.IsDependencyOperational<IDistanceAgent>())
			{
				var distanceData = await AgentBroker.Instance.TryExecuteOnFirst<IDistanceAgent, DistanceData>(a => a.CurrentData).GetValueOrDefault().ConfigureAwait(false);
				await AgentBroker.Instance.ObserveAny<IDistanceAgent, DistanceData>("DataSource").Where(data => data.AbsoluteDistance >= distanceData.AbsoluteDistance + this.DeviceDistanceFromStopTriggerPoint).Take(1).ToTask().ConfigureAwait(false);
			}

			result = await StopRecordCore(parameters, result).ConfigureAwait(false);

			return result;
		}

		private async Task<AcquisitionActionResult> InternalUninitializeRecord(UninitializeRecordParameter parameters, AcquisitionActionResult result)
		{
			if (parameters == null) throw new ArgumentNullException("parameters");
			if (result == null) throw new ArgumentNullException("result");

			result = await UninitializeRecordCore(parameters, result).ConfigureAwait(false);

			var currentTransfer = this.Provider.CurrentFileTransfer;
			if (currentTransfer != null)
				await currentTransfer.ConfigureAwait(false);

			CloseEventJournal();

			GenerateFileJournal(parameters);

			return result;
		}

		private string InitializeConfigurationJournal(string savePath, InitializeRecordParameter parameters)
		{
			if (string.IsNullOrEmpty(savePath)) throw new ArgumentNullException("savePath");
			if (parameters == null) throw new ArgumentNullException("parameters");

			if (string.IsNullOrEmpty(this.ConfigurationFilePath) || !File.Exists(this.ConfigurationFilePath))
				return null;
			else
			{
				Directory.CreateDirectory(savePath);

				var filename = string.IsNullOrWhiteSpace(this.Configuration.Agent.Journalisation.CharacterizationFileName) ? this.AgentUniversalName : string.Format("{0}.{1}", this.Configuration.Agent.Journalisation.CharacterizationFileName, this.AgentUniversalName);
				var completeFileName = Path.Combine(savePath, parameters.SequenceId + "." + filename + this.ConfigurationJournalFileExtension);
				File.Copy(this.ConfigurationFilePath, completeFileName);

				return completeFileName;
			}
		}

		protected abstract IEventJournal CreateEventJournal(InitializeRecordParameter parameters);

		private string InitializeEventJournal(string savePath, InitializeRecordParameter parameters)
		{
			if (string.IsNullOrEmpty(savePath)) throw new ArgumentNullException("savePath");
			if (parameters == null) throw new ArgumentNullException("parameters");

			string xmlCompleteFileName = string.Empty;

			this.EventJournal = CreateEventJournal(parameters);
			if (this.EventJournal != null)
			{
				Directory.CreateDirectory(savePath);

				this.EventJournal.EventJournalHeader.CreationSource = this.AgentUniversalName;
				this.EventJournal.EventJournalHeader.Id = parameters.SequenceId;
				this.EventJournal.EventJournalHeader.Sequenceur = parameters.SequenceId;
				this.EventJournal.EventJournalHeader.AcquisitionOffset = this.Configuration.Agent.Journalisation.AcquisitionOffSet;

				string filename = string.IsNullOrWhiteSpace(this.Configuration.Agent.Journalisation.CharacterizationFileName) ? this.AgentUniversalName : string.Format("{0}.{1}", this.Configuration.Agent.Journalisation.CharacterizationFileName, this.AgentUniversalName);
				xmlCompleteFileName = Path.Combine(savePath, parameters.SequenceId + "." + filename + this.EventJournalFileExtension);
				_eventJournalRecorder = new JournalBufferedRecorder(xmlCompleteFileName, this.EventJournal, true);
			}

			return xmlCompleteFileName;
		}

		protected void AddEventJournalEntry(JournalEntry entry)
		{
			if (entry == null) throw new ArgumentNullException("entry");
			if (this.EventJournal == null) throw new InvalidOperationException("No event journal has been created.");

			this.EventJournal.Add(entry);
		}

		private void CloseEventJournal()
		{
			if (_eventJournalRecorder != null)
				_eventJournalRecorder.Close();

			_eventJournalRecorder = null;
			this.EventJournal = null;
		}

		private void GenerateFileJournal(AcquisitionParameter parameters)
		{
			if (parameters == null) throw new ArgumentNullException("parameters");

			if (!Directory.Exists(this.JournalAbsoluteSavePath))
				return;

			var now = DateTimePrecise.Now;

			var fileJournal = new FileJournal();
			fileJournal.JournalHeader.CreationDateTime = now;
			fileJournal.JournalHeader.CreationSource = this.AgentUniversalName;
			fileJournal.JournalHeader.Id = parameters.SequenceId;
			fileJournal.JournalHeader.Sequenceur = parameters.SequenceId;
			fileJournal.JournalHeader.Root = Path.Combine(this.JournalRootPath, parameters.SequenceId);

			var filename = string.IsNullOrWhiteSpace(this.Configuration.Agent.Journalisation.CharacterizationFileName) ? this.AgentUniversalName : string.Format("{0}.{1}", this.Configuration.Agent.Journalisation.CharacterizationFileName, this.AgentUniversalName);
			var xmlCompleteFileName = Path.Combine(this.JournalAbsoluteSavePath, parameters.SequenceId + "." + filename + this.FileJournalFileExtension);
			int rootLength = 1 + Path.Combine(this.JournalRootPath, parameters.SequenceId).Length;

			using (var recorder = new JournalBufferedRecorder(xmlCompleteFileName, fileJournal, forceFlush: true))
			{
				try
				{
					foreach (var file in SafeFileEnumerator.EnumerateFiles(this.JournalAbsoluteSavePath, "*.*", SearchOption.AllDirectories).Where(file => !Path.GetExtension(file).Equals(".fjx", StringComparison.InvariantCultureIgnoreCase)))
						fileJournal.Add(new FileJournalEntry { FileName = Path.GetFileName(file), RelativePath = file.Substring(rootLength), DateTime = now });

					foreach (var journalEntry in _fileJournalEntries)
						fileJournal.Add(journalEntry);
				}
				finally
				{
					fileJournal.Close();
				}
			}
		}

		protected void AddFileJournalEntry(string relativeFilePath)
		{
			if (string.IsNullOrEmpty(relativeFilePath)) throw new ArgumentNullException("relativeFilePath");

			AddFileJournalEntry(new FileJournalEntry { FileName = Path.GetFileName(relativeFilePath), RelativePath = relativeFilePath, DateTime = DateTimeOffset.Now.DateTime });
		}

		protected void AddFileJournalEntry(FileJournalEntry journalEntry)
		{
			if (journalEntry == null) throw new ArgumentNullException("journalEntry");

			_fileJournalEntries.Add(journalEntry);
		}

		protected override void DisposeCore(bool disposing)
		{
			this.CloseEventJournal();

			base.DisposeCore(disposing);
		}
	}
}