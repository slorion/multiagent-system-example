using DLC.Framework;
using DLC.Framework.Extensions;
using DLC.Framework.Reactive;
using DLC.Multiagent;
using DLC.Multiagent.Logging;
using DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.Configuration;
using DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using DLC.Scientific.Acquisition.Core.Agents;
using DLC.Scientific.Acquisition.Core.Agents.Model;
using DLC.Scientific.Acquisition.Core.Configuration;
using DLC.Scientific.Core.Agents;
using DLC.Scientific.Core.Geocoding;
using DLC.Scientific.Core.Geocoding.Bgr;
using DLC.Scientific.Core.Geocoding.Gps;
using DLC.Scientific.Core.Journalisation;
using DLC.Scientific.Core.Journalisation.Journals;
using MathNet.Numerics.LinearAlgebra;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent
{
	public class AcquisitionManagerAgent
		: AcquisitionableAgent<ManualProvider, ProviderData, AcquisitionManagerAgentConfiguration, AcquisitionModuleConfiguration>, IAcquisitionManagerAgent, IVisibleAgent, IInternalAcquisitionManagerAgent
	{
		private readonly SubjectSlim<AcquisitionManagerStateChangedResult> _acquisitionManagerStateSubject = new SubjectSlim<AcquisitionManagerStateChangedResult>();
		private CancellationTokenSource _acquisitionTriggerCts;

		public double StartStopTriggerCalculationRadiusInMeters { get; set; }
		public string DefaultRootPath { get; set; }
		public List<string> Drivers { get; private set; }
		public List<string> Operators { get; private set; }
		public List<Tuple<string, string>> Vehicles { get; private set; }
		public List<string> SequenceTypes { get; private set; }
		public Dictionary<int, string> ProximityRanges { get; private set; }
		public string SelectedDriver { get; private set; }
		public string SelectedOperator { get; private set; }
		public string SelectedVehicle { get; private set; }
		public string SelectedSequenceType { get; private set; }
		public double StartRtsscSearchRadiusInMeters { get; private set; }
		public double StopRtsscSearchRadiusInMeters { get; private set; }
		public AcquisitionTriggerMode LastSelectedStartTriggerMode { get; private set; }
		public AcquisitionTriggerMode LastSelectedStopTriggerMode { get; private set; }
		public bool EnableUseOfOdometricCompensationAlgorithm { get; set; }
		public double MinDistanceToSwitchToGps { get; set; }

		public IObservable<AcquisitionManagerStateChangedResult> AcquisitionManagerStateDataSource { get { return _acquisitionManagerStateSubject; } }

		protected override void ConfigureAgent()
		{
			base.ConfigureAgent();

			this.AutoShowUI = this.Configuration.Agent.AutoShowUI;
			this.MainUITypeName = typeof(AcquisitionManagerUI).AssemblyQualifiedName;
			this.MainUIAgentTypeName = typeof(IInternalAcquisitionManagerAgent).AssemblyQualifiedName;
			this.DefaultRootPath = this.Configuration.Agent.DefaultRootPath;
			this.Drivers = this.Configuration.Agent.Drivers;
			this.SelectedDriver = this.Configuration.Agent.SelectedDriver;
			this.Operators = this.Configuration.Agent.Operators;
			this.SelectedOperator = this.Configuration.Agent.SelectedOperator;
			this.Vehicles = this.Configuration.Agent.Vehicles.Select(vehiclesDesc => new Tuple<string, string>(string.Format("{0}, {1}, {2}", vehiclesDesc.Number, vehiclesDesc.Name, vehiclesDesc.Type), vehiclesDesc.Name)).ToList();
			this.SelectedVehicle = this.Configuration.Agent.SelectedVehicle;
			this.SequenceTypes = this.Configuration.Agent.SequenceTypes;
			this.SelectedSequenceType = this.Configuration.Agent.SelectedSequenceType;
			this.ProximityRanges = this.Configuration.Agent.ProximityRanges.ToDictionary(key => key, key => string.Format("{0} m", key));
			this.StartStopTriggerCalculationRadiusInMeters = this.Configuration.Agent.StartStopTriggerRadius;
			this.AgentUniversalName = "AcquisitionManagerAgent";
			string lastTriggerMode = this.Configuration.Agent.LastSelectedStartTriggerMode ?? AcquisitionTriggerMode.Manual.ToString();
			this.LastSelectedStartTriggerMode = Enum.IsDefined(typeof(AcquisitionTriggerMode), lastTriggerMode) ? (AcquisitionTriggerMode) Enum.Parse(typeof(AcquisitionTriggerMode), lastTriggerMode) : AcquisitionTriggerMode.Manual;
			lastTriggerMode = this.Configuration.Agent.LastSelectedStopTriggerMode ?? AcquisitionTriggerMode.Manual.ToString();
			this.LastSelectedStopTriggerMode = Enum.IsDefined(typeof(AcquisitionTriggerMode), lastTriggerMode) ? (AcquisitionTriggerMode) Enum.Parse(typeof(AcquisitionTriggerMode), lastTriggerMode) : AcquisitionTriggerMode.Manual;
			this.EnableUseOfOdometricCompensationAlgorithm = this.Configuration.Agent.EnableUseOfOdometricCompensationAlgorithm;
			this.MinDistanceToSwitchToGps = this.Configuration.Agent.MinDistanceToSwitchToGps;
			this.StartRtsscSearchRadiusInMeters = 10000;
			this.StopRtsscSearchRadiusInMeters = 50000;
		}

		protected override ManualProvider CreateAndConfigureProvider()
		{
			return new ManualProvider();
		}

		protected override IEventJournal CreateEventJournal(InitializeRecordParameter parameters)
		{
			var ejxJournal = new AcquisitionManagerAgentEventJournal();
			var header = ejxJournal.JournalHeader;
			header.Sequenceur = parameters.SequenceId;
			header.AcquisitionDate = DateTimePrecise.Now;
			header.DriverFirstName = parameters.DriverFirstName;
			header.DriverLastName = parameters.DriverLastName;
			header.OperatorFirstName = parameters.OperatorFirstName;
			header.OperatorLastName = parameters.OperatorLastName;
			header.VehicleNumber = parameters.VehicleId;
			header.VehicleName = parameters.VehicleName;
			header.VehicleType = parameters.VehicleType;
			header.TestType = parameters.TestType;

			return ejxJournal;
		}

		protected override void SetupAgentOperationalCommunicationsCore()
		{
			base.SetupAgentOperationalCommunicationsCore();

			TrackDependencyOperationalState<IBgrDirectionalAgent>(isMandatory: false, canFailover: true);
			TrackDependencyOperationalState<ILocalisationAgent>(isMandatory: false, canFailover: true);
			TrackDependencyOperationalState<IDistanceAgent>(isMandatory: false, canFailover: true);
			TrackDependencyOperationalState<ITriggerAgent>(isMandatory: false, canFailover: true);
			//TrackDependencyOperationalState<IFileTransferManagerAgent>(isMandatory: false, canFailover: true);
			TrackDependencyOperationalState<ISpeedAgent>(isMandatory: false, canFailover: true);
		}

		protected override async Task OnBeforeStateTransition(AcquisitionStep step, AcquisitionParameter parameters, AcquisitionActionResult result)
		{
			await base.OnBeforeStateTransition(step, parameters, result).ConfigureAwait(false);

			if (step == AcquisitionStep.InitializeRecord)
				this.Provider.CurrentStateTransitionCancellationToken.Register(async () => await ExecuteOnAllAgentsByPriority(new UninitializeRecordParameter { SequenceId = parameters.SequenceId }, result, UninitializeRecordOnAgent, null).ConfigureAwait(false));
		}

		protected override async Task OnAfterStateTransition(AcquisitionStep step, AcquisitionParameter parameters, AcquisitionActionResult result)
		{
			await base.OnAfterStateTransition(step, parameters, result).ConfigureAwait(false);

			_acquisitionManagerStateSubject.OnNext(new AcquisitionManagerStateChangedResult { Parameters = parameters, ProviderState = this.ProviderState, Result = result });
		}

		public Dictionary<AcquisitionTriggerMode, Tuple<string, string>> GetAcquisitionTriggerModes(bool isStartMode)
		{
			var configModes = isStartMode ? this.Configuration.Agent.StartTriggerModes : this.Configuration.Agent.StopTriggerModes;

			var modes = configModes
					.Where(mode => Enum.IsDefined(typeof(AcquisitionTriggerMode), mode.Name) && mode.Enabled)
					.Select(mode => (AcquisitionTriggerMode) Enum.Parse(typeof(AcquisitionTriggerMode), mode.Name))
					.Where(mode => mode != AcquisitionTriggerMode.Unknown)
					.ToDictionary(
						mode => mode,
						mode =>
						{
							string type;
							string description;
							switch (mode)
							{
								case AcquisitionTriggerMode.Manual:
									type = null;
									description = "Manual";
									break;
								case AcquisitionTriggerMode.Rtssc:
									type = typeof(IBgrDirectionalAgent).AssemblyQualifiedName;
									description = "RTSSC (straight line)";
									break;
								case AcquisitionTriggerMode.RtsscProximity:
									type = typeof(IBgrDirectionalAgent).AssemblyQualifiedName;
									description = "RTSSC (with an angle)";
									break;
								case AcquisitionTriggerMode.EndSection:
									type = typeof(IBgrDirectionalAgent).AssemblyQualifiedName;
									description = isStartMode ? "Next section start" : "End of current section";
									break;
								case AcquisitionTriggerMode.Photocellule:
									type = typeof(ITriggerAgent).AssemblyQualifiedName;
									description = "Photocell";
									break;
								case AcquisitionTriggerMode.Distance:
									type = typeof(IDistanceAgent).AssemblyQualifiedName;
									description = "Distance";
									break;
								default:
									throw new NotSupportedException();
							}

							return Tuple.Create(type, description);
						});

			// Distance trigger not supported as start mode
			if (isStartMode)
			{
				modes.Remove(AcquisitionTriggerMode.Distance);
			}

			return modes;
		}

		private bool ValidateAcquisitionTriggerParameters(bool isStartMode, TriggeredAcquisitionParameter parameters)
		{
			if (parameters == null) throw new ArgumentNullException("parameters");

			List<string> errorsMessages = new List<string>();

			if (this.OperationalState != OperationalAgentStates.None)
				errorsMessages.Add("AcquisitionManager agent is not operational.");

			if (_acquisitionTriggerCts != null)
				errorsMessages.Add("Cannot define acquisition parameters since already subscribed to a starting mode.");

			if (isStartMode)
			{
				if (this.ProviderState != ProviderState.StartingRecord)
					errorsMessages.Add(string.Format("Cannot define acquisition parameters since current state is '{0}'. It should be equal to 'StartingRecord'.", this.ProviderState));
			}
			else
			{
				if (this.ProviderState != ProviderState.StoppingRecord)
					errorsMessages.Add(string.Format("Cannot define acquisition parameters since current state is '{0}'. It should be equal to 'StoppingRecord'.", this.ProviderState));
			}

			if (string.IsNullOrEmpty(this.SequenceId))
				errorsMessages.Add("Cannot define acquisition parameters since SequenceId is not available. Suggestion: prepare the acquisition again.");

			if (parameters.TriggerMode == AcquisitionTriggerMode.Rtssc || parameters.TriggerMode == AcquisitionTriggerMode.RtsscProximity)
			{
				if (parameters.Rtssc == null)
					errorsMessages.Add("Cannot define acquisition parameters since start mode is RTSS, but a RTSSC has not been provided.");

				if (parameters.DirectionBgr != DirectionBgr.ForwardChaining && parameters.DirectionBgr != DirectionBgr.BackwardChaining)
					errorsMessages.Add("Cannot define acquisition parameters since the start mode is RTSS, but a direction has not been specified.");
			}

			if (parameters.TriggerMode == AcquisitionTriggerMode.RtsscProximity)
			{
				if (parameters.ProximityRange == null || parameters.ProximityRange <= 0)
					errorsMessages.Add("Cannot define acquisition parameters since start mode is RTSS by proximity, but a proximity range has not been provided or is not greater than 0.");
			}

			if (parameters.TriggerMode == AcquisitionTriggerMode.Distance)
			{
				if (parameters.Distance == null || parameters.Distance <= 0)
					errorsMessages.Add("Cannot define acquisition parameters since the distance value must be greater than 0 when the selected trigger mode is 'Distance'.");
			}

			foreach (var message in errorsMessages)
				Log.Error().Message(message).WithAgent(this.Id).Write();

			return !errorsMessages.Any();
		}

		private async Task<bool> WaitForAcquisitionTrigger(bool isStartMode, TriggeredAcquisitionParameter parameters, AcquisitionActionResult result)
		{
			if (parameters == null) throw new ArgumentNullException("parameters");
			if (result == null) throw new ArgumentNullException("result");

			if (!ValidateAcquisitionTriggerParameters(isStartMode, parameters))
				return false;

			bool success = true;
			_acquisitionTriggerCts = new CancellationTokenSource();
			try
			{
				switch (parameters.TriggerMode)
				{
					case AcquisitionTriggerMode.Manual:
						Log.Info().Message("---=== Manual trigger mode subscription ===---").WithAgent(this.Id).Write();
						break;
					case AcquisitionTriggerMode.Rtssc:
						{
							Log.Info().Message("---=== RTSS trigger mode subscription ===---").WithAgent(this.Id).Write();

							DirectionBgr currentDirection = await AgentBroker.Instance.TryExecuteOnFirst<IBgrDirectionalAgent, DirectionBgr>(a => a.CurrentData == null ? DirectionBgr.Unknown : a.CurrentData.Direction).GetValueOrDefault(DirectionBgr.Unknown).ConfigureAwait(false);

							success &= await WaitForRtssc(isStartMode, parameters, result, parameters.Rtssc, currentDirection, _acquisitionTriggerCts.Token).ConfigureAwait(false);
						}
						break;
					case AcquisitionTriggerMode.EndSection:
						{
							Log.Info().Message("---=== Next section trigger mode subscription ===---").WithAgent(this.Id).Write();

							var currentBgrData = await AgentBroker.Instance.TryExecuteOnFirst<IBgrDirectionalAgent, BgrData>(a => a.CurrentData).GetValueOrDefault(null).ConfigureAwait(false);

							if (currentBgrData == null)
								currentBgrData = await AgentBroker.Instance.ObserveAny<IBgrDirectionalAgent, BgrData>("DataSource").Take(1).ToTask(_acquisitionTriggerCts.Token).ConfigureAwait(false);

							if (currentBgrData == null)
							{
								Log.Error().Message("Cannot get next section start RTSSC.").WithAgent(this.Id).Write();
								return false;
							}

							Rtssc endSectionRtssc;
							switch (currentBgrData.Direction)
							{
								case DirectionBgr.ForwardChaining:
									double chainage = await AgentBroker.Instance.TryExecuteOnFirst<IBgrDirectionalAgent, double>(agent => agent.GetSectionLength(currentBgrData.Rtssc)).GetValueOrDefault(-1).ConfigureAwait(false);
									endSectionRtssc = new Rtssc(currentBgrData.Rtssc, chainage);
									break;
								case DirectionBgr.BackwardChaining:
									endSectionRtssc = new Rtssc(currentBgrData.Rtssc, 0);
									break;
								case DirectionBgr.Unknown:
								default:
									Log.Error().Message("Cannot get the current direction which is required to get the RTSSC of the next section start. Possible solution: Vehicle must be moving to determine the current direction when the current 'sous-route' type is [000C].").WithAgent(this.Id).Write();
									return false;
							}

							success &= await WaitForRtssc(isStartMode, parameters, result, endSectionRtssc, currentBgrData.Direction, _acquisitionTriggerCts.Token).ConfigureAwait(false);
						}
						break;
					case AcquisitionTriggerMode.RtsscProximity:
						{
							Log.Info().Message("---=== RTSSC by proximity trigger mode subscription ===---").WithAgent(this.Id).Write();

							var coordRtssc = await AgentBroker.Instance.TryExecuteOnFirst<IBgrDirectionalAgent, GeoCoordinate>(a => a.GeoCodage(parameters.Rtssc)).GetValueOrDefault(null).ConfigureAwait(false);
							if (coordRtssc == null)
							{
								Log.Error().Message("Cannot get the GPS coordinate of the selected RTSSC.").WithAgent(this.Id).Write();
								return false;
							}

							parameters.GeoCoordinate = coordRtssc;
							_acquisitionManagerStateSubject.OnNext(new AcquisitionManagerStateChangedResult { ProviderState = this.ProviderState, Parameters = parameters, Result = result });

							var gpsDistanceFromTrigger = await AgentBroker.Instance.TryExecuteOnFirst<ILocalisationAgent, double>(a => a.GpsDistanceFromTriggerPoint).GetOrThrow().ConfigureAwait(false);

							await GpsHelper.DetectProximityToTarget(
									AgentBroker.Instance.ObserveAny<ILocalisationAgent, LocalisationData>("DataSource")
										.Select(data => new Tuple<GeoData, GpsStatus>(data.CorrectedData, data.GpsStatus)),
									this.EnableUseOfOdometricCompensationAlgorithm
										? AgentBroker.Instance.ObserveAny<IDistanceAgent, DistanceData>("DataSource").Select(data => data.AbsoluteDistance)
										: null,
									coordRtssc,
									this.MinDistanceToSwitchToGps,
									gpsDistanceFromTrigger,
									Convert.ToDouble(parameters.ProximityRange.Value))
								.ObserveOn(TaskPoolScheduler.Default)
								.Where(data => data)
								.Take(1)
								.ToTask(_acquisitionTriggerCts.Token)
								.ConfigureAwait(false);
						}
						break;
					case AcquisitionTriggerMode.Photocellule:
						{
							Log.Info().Message("---=== Photocell trigger mode subscription ===---").WithAgent(this.Id).Write();

							if (!IsDependencyOperational<ITriggerAgent>())
							{
								Log.Error().Message("Photocell agent is not operational.").WithAgent(this.Id).Write();
								return false;
							}

							_acquisitionManagerStateSubject.OnNext(new AcquisitionManagerStateChangedResult { ProviderState = this.ProviderState, Parameters = parameters, Result = result });

							var photocellTriggerMode = isStartMode ? TriggerMode.Start : TriggerMode.Stop;

							using (
								_acquisitionTriggerCts.Token.Register(
									async () =>
									{
										var r = await AgentBroker.Instance.TryExecuteOnFirst<ITriggerAgent>(a => a.CancelWaitForTrigger(photocellTriggerMode)).ConfigureAwait(false);
										if (!r.IsSuccessful)
										{
											Log.Error().Message("An error occurred while trying to cancel the photocell trigger.").WithAgent(this.Id).Write();
											success = false;
										}
									}))
							{
								var r = await AgentBroker.Instance.TryExecuteOnFirst<ITriggerAgent>(a => a.WaitForTrigger(photocellTriggerMode)).ConfigureAwait(false);
								if (!r.IsSuccessful)
								{
									Log.Error().Message("An error occurred while waiting for the photocell trigger.").WithAgent(this.Id).Write();
									success = false;
								}
							}
						}
						break;
					case AcquisitionTriggerMode.Distance:
						{
							Log.Info().Message("---=== Distance trigger mode subscription ===---").WithAgent(this.Id).Write();

							if (!IsDependencyOperational<IDistanceAgent>())
							{
								Log.Error().Message("Distance agent is not operational.").WithAgent(this.Id).Write();
								return false;
							}

							_acquisitionManagerStateSubject.OnNext(new AcquisitionManagerStateChangedResult { ProviderState = this.ProviderState, Parameters = parameters, Result = result });

							await AgentBroker.Instance.ObserveAny<IDistanceAgent, DistanceData>("DataSource").Where(data => data.AbsoluteDistance >= parameters.Distance.Value).Take(1).ToTask(_acquisitionTriggerCts.Token).ConfigureAwait(false);
						}
						break;
				}
			}
			catch (TaskCanceledException) { }
			finally
			{
				if (_acquisitionTriggerCts.IsCancellationRequested)
				{
					Log.Info().Message("Acquisition {0} has been cancelled.", isStartMode ? "start" : "stop").WithAgent(this.Id).Write();
					success = false;
				}

				_acquisitionTriggerCts.Dispose();
				_acquisitionTriggerCts = null;
			}

			return success;
		}

		private async Task<bool> WaitForRtssc(bool isStartMode, TriggeredAcquisitionParameter parameters, AcquisitionActionResult result, IRtssc rtssc, DirectionBgr direction, CancellationToken ct)
		{
			if (rtssc == null) throw new ArgumentNullException("rtssc");

			var coordRtssc = await AgentBroker.Instance.TryExecuteOnFirst<IBgrDirectionalAgent, GeoCoordinate>(a => a.GeoCodage(rtssc)).GetValueOrDefault(null).ConfigureAwait(false);
			if (coordRtssc == null)
			{
				Log.Error().Message("Cannot get the GPS coordinate of the selected RTSSC.").WithAgent(this.Id).Write();
				return false;
			}

			Vector<double> bgrDirectionVector = await GetDirectionVector(rtssc, direction).ConfigureAwait(false);
			if (bgrDirectionVector == null)
			{
				Log.Error().Message("Cannot get the current direction which is required to get the RTSSC of the next section start. Possible solution: Vehicle must be moving to determine the current direction when the current 'sous-route' type is [000C]..").WithAgent(this.Id).Write();
				return false;
			}

			parameters.Rtssc = rtssc;
			parameters.GeoCoordinate = coordRtssc;
			_acquisitionManagerStateSubject.OnNext(new AcquisitionManagerStateChangedResult { ProviderState = this.ProviderState, Parameters = parameters, Result = result });

			var gpsDistanceFromTrigger = await AgentBroker.Instance.TryExecuteOnFirst<ILocalisationAgent, double>(a => a.GpsDistanceFromTriggerPoint).GetOrThrow().ConfigureAwait(false);
			var gpsFrequency = await AgentBroker.Instance.TryExecuteOnFirst<ILocalisationAgent, int>(a => a.GpsFrequency).GetOrThrow().ConfigureAwait(false);

			await GpsHelper.DetectPositionExceededByAngleAndDistance(
					AgentBroker.Instance.ObserveAny<ILocalisationAgent, LocalisationData>("DataSource")
						.Select(data => new Tuple<GeoData, GpsStatus>(data.CorrectedData, data.GpsStatus)),
					this.EnableUseOfOdometricCompensationAlgorithm ?
						AgentBroker.Instance.ObserveAny<IDistanceAgent, DistanceData>("DataSource").Select(data => data.AbsoluteDistance) :
						null,
					coordRtssc,
					this.MinDistanceToSwitchToGps,
					gpsDistanceFromTrigger,
					!isStartMode,
					this.StartStopTriggerCalculationRadiusInMeters,
					gpsFrequency,
					bgrDirectionVector)
				.ObserveOn(TaskPoolScheduler.Default)
				.Retry()
				.Where(data => data)
				.Take(1)
				.ToTask(ct)
				.ConfigureAwait(false);

			return true;
		}

		private void WriteFileVersionJournal(string savePath, AcquisitionParameter parameters)
		{
			if (string.IsNullOrEmpty(savePath)) throw new ArgumentNullException("savePath");
			if (parameters == null) throw new ArgumentNullException("parameters");

			Directory.CreateDirectory(savePath);

			var fvxJournal = new FileVersionJournal();
			fvxJournal.JournalHeader.CreationDateTime = DateTimePrecise.Now;
			fvxJournal.JournalHeader.CreationSource = this.AgentUniversalName;
			fvxJournal.JournalHeader.Id = parameters.SequenceId;
			fvxJournal.JournalHeader.Sequenceur = parameters.SequenceId;
			fvxJournal.JournalHeader.Root = savePath;

			string xmlCompleteFileName = Path.Combine(savePath, parameters.SequenceId + "." + this.AgentUniversalName + ".fvx");

			using (var recorder = new JournalBufferedRecorder(xmlCompleteFileName, fvxJournal, forceFlush: true))
			{
				fvxJournal.Add(new FileVersionJournalEntry {
					FileName = Path.GetFileName(this.FileJournalRelativePath),
					RelativePath = this.FileJournalRelativePath,
					DateTime = DateTimePrecise.Now
				});

				fvxJournal.Close();
			}
		}

		private async Task<AcquisitionActionResult> ExecuteAcquisitionStepOnAgent<TParameter>(AcquisitionStep step, Func<IAcquisitionableAgent, TParameter, Task<AcquisitionActionResult>> stepAction, IAcquisitionableAgent agent, string agentSignature, TParameter parameters)
			where TParameter : AcquisitionParameter
		{
			if (agent == null) throw new ArgumentNullException("agent");
			if (string.IsNullOrWhiteSpace(agentSignature)) throw new ArgumentNullException("agentSignature");
			if (parameters == null) throw new ArgumentNullException("parameters");

			try
			{
				return await AgentBroker.Instance.TryExecuteOnOne<IAcquisitionableAgent, AcquisitionActionResult>(agent.Id, a => stepAction(a, parameters)).GetOrThrow().ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				return new AcquisitionActionResult(parameters.SequenceId, step, agentSignature, agent.DisplayData.Name) { Exception = ex };
			}
		}

		private Task<AcquisitionActionResult> InitializeRecordOnAgent(IAcquisitionableAgent agent, string agentSignature, InitializeRecordParameter parameters)
		{
			if (agent == null) throw new ArgumentNullException("agent");
			if (string.IsNullOrEmpty(agentSignature)) throw new ArgumentNullException("agentSignature");
			if (parameters == null) throw new ArgumentNullException("parameters");

			return ExecuteAcquisitionStepOnAgent(AcquisitionStep.InitializeRecord, (a, p) => a.InitializeRecord((InitializeRecordParameter) p), agent, agentSignature, parameters);
		}

		private Task<AcquisitionActionResult> StartRecordOnAgent(IAcquisitionableAgent agent, string agentSignature, StartRecordParameter parameters)
		{
			if (agent == null) throw new ArgumentNullException("agent");
			if (string.IsNullOrEmpty(agentSignature)) throw new ArgumentNullException("agentSignature");
			if (parameters == null) throw new ArgumentNullException("parameters");

			return ExecuteAcquisitionStepOnAgent(AcquisitionStep.StartRecord, (a, p) => a.StartRecord((StartRecordParameter) p), agent, agentSignature, parameters);
		}

		private Task<AcquisitionActionResult> StopRecordOnAgent(IAcquisitionableAgent agent, string agentSignature, StopRecordParameter parameters)
		{
			if (agent == null) throw new ArgumentNullException("agent");
			if (string.IsNullOrEmpty(agentSignature)) throw new ArgumentNullException("agentSignature");
			if (parameters == null) throw new ArgumentNullException("parameters");

			return ExecuteAcquisitionStepOnAgent(AcquisitionStep.StopRecord, (a, p) => a.StopRecord((StopRecordParameter) p), agent, agentSignature, parameters);
		}

		private Task<AcquisitionActionResult> UninitializeRecordOnAgent(IAcquisitionableAgent agent, string agentSignature, UninitializeRecordParameter parameters)
		{
			if (agent == null) throw new ArgumentNullException("agent");
			if (string.IsNullOrEmpty(agentSignature)) throw new ArgumentNullException("agentSignature");
			if (parameters == null) throw new ArgumentNullException("parameters");

			return ExecuteAcquisitionStepOnAgent(AcquisitionStep.UninitializeRecord, (a, p) => a.UninitializeRecord((UninitializeRecordParameter) p), agent, agentSignature, parameters);
		}

		private async Task<Vector<double>> GetDirectionVector(IRtssc currentRtssc, DirectionBgr direction)
		{
			if (currentRtssc == null) throw new ArgumentNullException("currentRtssc");

			bool isReversed;
			Rtssc previousRtssc;

			switch (direction)
			{
				case DirectionBgr.ForwardChaining:
					if (currentRtssc.Chainage == 0)
					{
						isReversed = true;
						previousRtssc = new Rtssc(currentRtssc, currentRtssc.Chainage + 1);
					}
					else
					{
						isReversed = false;
						previousRtssc = new Rtssc(currentRtssc, currentRtssc.Chainage - 1);
					}
					break;
				case DirectionBgr.BackwardChaining:
					if (currentRtssc.Chainage == await AgentBroker.Instance.TryExecuteOnFirst<IBgrDirectionalAgent, double>(a => a.GetSectionLength(currentRtssc)).GetValueOrDefault(-1).ConfigureAwait(false))
					{
						isReversed = true;
						previousRtssc = new Rtssc(currentRtssc, currentRtssc.Chainage - 1);
					}
					else
					{
						isReversed = false;
						previousRtssc = new Rtssc(currentRtssc, currentRtssc.Chainage + 1);
					}
					break;
				case DirectionBgr.Unknown:
				default:
					return null;
			}

			GeoCoordinate rtsscCurrentCoord = await AgentBroker.Instance.TryExecuteOnFirst<IBgrDirectionalAgent, GeoCoordinate>(a => a.GeoCodage(currentRtssc)).GetValueOrDefault(null).ConfigureAwait(false);
			if (rtsscCurrentCoord == null || rtsscCurrentCoord.Equals(new GeoCoordinate()))
				return null;

			GeoCoordinate rtsscPreviousCoordinate = await AgentBroker.Instance.TryExecuteOnFirst<IBgrDirectionalAgent, GeoCoordinate>(a => a.GeoCodage(previousRtssc)).GetValueOrDefault(null).ConfigureAwait(false);
			if (rtsscPreviousCoordinate == null || rtsscPreviousCoordinate.Equals(new GeoCoordinate()))
				return null;

			var currentCartesianCoord = GpsHelper.ConvertGeoCoordinateToCartesian(rtsscCurrentCoord.Latitude, rtsscCurrentCoord.Longitude);
			var previousCartesianCoord = GpsHelper.ConvertGeoCoordinateToCartesian(rtsscPreviousCoordinate.Latitude, rtsscPreviousCoordinate.Longitude);
			var bgrDirectionVector = currentCartesianCoord.Subtract(previousCartesianCoord);

			if (isReversed)
				bgrDirectionVector *= -1;

			return bgrDirectionVector;
		}

		private async Task<AcquisitionActionResult> ExecuteOnAllAgentsByPriority<TParameters, TResult>(TParameters parameters, TResult result, Func<IAcquisitionableAgent, string, TParameters, Task<TResult>> action, Func<TParameters, TResult, Task<AcquisitionActionResult>> compensatingAction = null)
			where TParameters : AcquisitionParameter
			where TResult : AcquisitionActionResult
		{
			if (parameters == null) throw new ArgumentNullException("parameters");
			if (result == null) throw new ArgumentNullException("result");
			if (action == null) throw new ArgumentNullException("action");

			var agentsByPriority = AgentBroker.Instance.GetAgents<IAcquisitionableAgent>(ExecutionScopeOptions.All).Where(t => t.Item1.IsReachable).Select(t => t.Item2)
				.Where(a => a != this && a.State == AgentState.Activated)
				.GroupBy(a => a.Priority)
				.OrderByDescending(g => g.Key);

			var results = new List<AcquisitionActionResult>();
			foreach (var agentGroup in agentsByPriority)
				results.AddRange(await Task.WhenAll(agentGroup.Select(a => action(a, a.Id, parameters))).ConfigureAwait(false));

			var faultedResults = results.Where(r => r.Exception != null);
			if (faultedResults.Any())
			{
				foreach (var faultedResult in faultedResults)
					Log.Error().Message(string.Format("{0} - {1}", faultedResult.AgentId, faultedResult.Exception.Message)).WithAgent(this.Id).Write();

				result.Exception = new AggregateException(faultedResults.Select(r => r.Exception));
				result.IsSuccessful = false;
			}
			if (results.Any(agentResult => !agentResult.IsSuccessful))
				result.IsSuccessful = false;

			if (!result.IsSuccessful && compensatingAction != null)
			{
				var compensatingResult = await compensatingAction(parameters, result).ConfigureAwait(false);
				if (compensatingResult.Exception != null)
				{
					if (compensatingResult.Exception is AggregateException)
						foreach (var ex in ((AggregateException) compensatingResult.Exception).InnerExceptions)
							Log.Error().Message(ex.Message).WithAgent(this.Id).Write();
					else
						Log.Error().Message(compensatingResult.Exception.Message).WithAgent(this.Id).Write();
				}
			}

			return result;
		}

		#region IAcquisitionManagerAgent members

		public int MinimumSpeed
		{
			get { return this.Configuration.Agent.MinimumSpeed; }
		}
		public int MaximumSpeed
		{
			get { return this.Configuration.Agent.MaximumSpeed; }
		}

		public async Task<AcquisitionActionResult> PrepareRecord(string driverFullName, string operatorFullName, string vehicleFullName, string sequenceType)
		{
			if (string.IsNullOrWhiteSpace(driverFullName)) throw new ArgumentNullException("driverFullName");
			if (string.IsNullOrWhiteSpace(operatorFullName)) throw new ArgumentNullException("operatorFullName");
			if (string.IsNullOrWhiteSpace(vehicleFullName)) throw new ArgumentNullException("vehicleFullName");
			if (string.IsNullOrWhiteSpace(sequenceType)) throw new ArgumentNullException("sequenceType");

			await Task.WhenAll(AgentBroker.Instance.TryExecuteOnAll<IFileTransferManagerAgent>(a => a.StopTransferring())).ConfigureAwait(false);

			var timestamp = DateTimePrecise.Now;

			var driverCompleteName = driverFullName.Split(',');
			var driverLastName = driverCompleteName.First().Trim();
			var driverFirstName = (driverCompleteName.Skip(1).FirstOrDefault() ?? string.Empty).Trim();

			var operatorCompleteName = operatorFullName.Split(',');
			var operatorLastName = operatorCompleteName.First().Trim();
			var operatorFirstName = (operatorCompleteName.Skip(1).FirstOrDefault() ?? string.Empty).Trim();

			var vehicleCompleteName = vehicleFullName.Split(',');
			string vehicleId = vehicleCompleteName.First().Trim();
			string vehicleName = (vehicleCompleteName.Skip(1).FirstOrDefault() ?? string.Empty).Trim();
			string vehicleType = (vehicleCompleteName.Skip(2).FirstOrDefault() ?? string.Empty).Trim();

			var sequenceId = string.Format("{0}_{1:yyyyMMdd_HHmmss}", vehicleId.RemoveDiacritics(), timestamp);

			var parameters = new InitializeRecordParameter {
				SequenceId = sequenceId,
				TestType = sequenceType,
				DefaultRootPath = this.DefaultRootPath,
				DriverFullName = driverFullName,
				DriverFirstName = driverFirstName,
				DriverLastName = driverLastName,
				OperatorFullName = operatorFullName,
				OperatorFirstName = operatorFirstName,
				OperatorLastName = operatorLastName,
				VehicleFullName = vehicleFullName,
				VehicleId = vehicleId,
				VehicleName = vehicleName,
				VehicleType = vehicleType
			};

			return await this.InitializeRecord(parameters).ConfigureAwait(false);
		}

		protected override async Task<AcquisitionActionResult> InitializeRecordCore(InitializeRecordParameter parameters, AcquisitionActionResult result)
		{
			result = await base.InitializeRecordCore(parameters, result).ConfigureAwait(false);

			WriteFileVersionJournal(Path.Combine(parameters.DefaultRootPath, parameters.SequenceId), parameters);

			// prepare all agents in priority order
			// and then write the AcquisitionActionResult in the acquisition journal

			result = await ExecuteOnAllAgentsByPriority(parameters, result,
				(agent, signature, priority) =>
				{
					return InitializeRecordOnAgent(agent, signature, priority)
						.ContinueWith(
							t =>
							{
								if (t.Result.IsSuccessful && this.ProviderState >= ProviderState.InitializingRecord)
									AddFileJournalEntry(t.Result.FileJournalRelativePath);

								return t.Result;
							});
				},
				(p, r) =>
				{
					return ExecuteOnAllAgentsByPriority(new UninitializeRecordParameter { SequenceId = p.SequenceId }, new AcquisitionActionResult(r.SequenceId, r.AcquisitionStep, r.AgentId, r.AgentName), UninitializeRecordOnAgent, null);
				}).ConfigureAwait(false);

			return result;
		}

		public async Task<AcquisitionActionResult> EngageStartRecord(AcquisitionTriggerMode triggerMode, DirectionBgr? direction, IRtssc rtssc, int? proximityRange)
		{
			var parameters = new StartRecordParameter {
				SequenceId = this.SequenceId,
				TriggerMode = triggerMode,
				DirectionBgr = direction.HasValue ? direction.Value : DirectionBgr.Unknown,
				Rtssc = rtssc,
				ProximityRange = proximityRange,
				Distance = null
			};

			var result = await this.StartRecord(parameters).ConfigureAwait(false);
			if (!result.IsSuccessful)
				return result;

			var currentRtssc = parameters.Rtssc ?? await AgentBroker.Instance.TryExecuteOnFirst<IBgrDirectionalAgent, IRtssc>(a => a.CurrentData == null ? null : a.CurrentData.Rtssc).GetValueOrDefault(null).ConfigureAwait(false);

			var entry = new AcquisitionManagerAgentEventJournalEntry {
				Comment = triggerMode.ToString(),
				DateTime = DateTimePrecise.Now,
				Progress = 0,
				Action = "StartAcquisition",
				RTSSCDebut = Convert.ToString(currentRtssc),
				Distance = string.Format("{0:##.##}", parameters.Distance)
			};

			AddEventJournalEntry(entry);

			AcquisitionConfigurationFactory.Instance.Update(this.ConfigurationFilePath, "Agent.LastSelectedStartTriggerMode", triggerMode.ToString());

			return result;
		}

		protected override async Task<AcquisitionActionResult> StartCore(StartAcquisitionParameter parameters, AcquisitionActionResult result)
		{
			// if AcquisitionManager restart, make sure that no agent is in acquisition mode
			await Task.WhenAll(
				AgentBroker.Instance.TryExecuteOnAll<IAcquisitionableAgent>(
					async a =>
					{
						if (a == this || string.IsNullOrEmpty(a.SequenceId))
							return;

						await a.StopRecord(new StopRecordParameter { SequenceId = a.SequenceId, TriggerMode = AcquisitionTriggerMode.Manual }).ConfigureAwait(false);
						await a.UninitializeRecord(new UninitializeRecordParameter { SequenceId = a.SequenceId }).ConfigureAwait(false);
					})).ConfigureAwait(false);

			return result;
		}

		protected override async Task<AcquisitionActionResult> StartRecordCore(StartRecordParameter parameters, AcquisitionActionResult result)
		{
			result.IsSuccessful &= await WaitForAcquisitionTrigger(true, parameters, result).ConfigureAwait(false);
			if (!result.IsSuccessful)
				return result;

			result = await base.StartRecordCore(parameters, result).ConfigureAwait(false);
			if (!result.IsSuccessful)
				return result;

			result = await ExecuteOnAllAgentsByPriority(parameters, result,
				StartRecordOnAgent,
				(p, r) =>
				{
					return ExecuteOnAllAgentsByPriority(new StopRecordParameter { SequenceId = p.SequenceId }, new AcquisitionActionResult(r.SequenceId, r.AcquisitionStep, r.AgentId, r.AgentName), StopRecordOnAgent, null);
				}).ConfigureAwait(false);

			return result;
		}

		public async Task<AcquisitionActionResult> EngageStopRecord(AcquisitionTriggerMode triggerMode, DirectionBgr? direction, IRtssc rtssc, int? proximityRange, double? distance)
		{
			var parameters = new StopRecordParameter {
				SequenceId = this.SequenceId,
				TriggerMode = triggerMode,
				DirectionBgr = direction.HasValue ? direction.Value : DirectionBgr.Unknown,
				Rtssc = rtssc,
				ProximityRange = proximityRange,
				Distance = distance
			};

			var result = await this.StopRecord(parameters).ConfigureAwait(false);
			if (!result.IsSuccessful)
				return result;

			var currentRtssc = parameters.Rtssc ?? await AgentBroker.Instance.TryExecuteOnFirst<IBgrDirectionalAgent, IRtssc>(a => a.CurrentData == null ? null : a.CurrentData.Rtssc).GetValueOrDefault(null).ConfigureAwait(false);

			var entry = new AcquisitionManagerAgentEventJournalEntry {
				Comment = triggerMode.ToString(),
				DateTime = DateTimePrecise.Now,
				Progress = Convert.ToInt32(parameters.Distance.GetValueOrDefault()),
				Action = "StopAcquisition",
				RTSSCDebut = Convert.ToString(currentRtssc),
				Distance = string.Format("{0:##.##}", parameters.Distance)
			};

			AddEventJournalEntry(entry);

			AcquisitionConfigurationFactory.Instance.Update(this.ConfigurationFilePath, "Agent.LastSelectedStopTriggerMode", triggerMode.ToString());

			return result;
		}

		protected override async Task<AcquisitionActionResult> StopRecordCore(StopRecordParameter parameters, AcquisitionActionResult result)
		{
			result.IsSuccessful &= await WaitForAcquisitionTrigger(false, parameters, result).ConfigureAwait(false);
			if (!result.IsSuccessful)
				return result;

			result = await base.StopRecordCore(parameters, result).ConfigureAwait(false);
			if (!result.IsSuccessful)
				return result;

			return await ExecuteOnAllAgentsByPriority(parameters, result, StopRecordOnAgent, null).ConfigureAwait(false);
		}

		protected override async Task<AcquisitionActionResult> UninitializeRecordCore(UninitializeRecordParameter parameters, AcquisitionActionResult result)
		{
			result = await base.UninitializeRecordCore(parameters, result).ConfigureAwait(false);
			if (!result.IsSuccessful)
				return result;

			result = await ExecuteOnAllAgentsByPriority(parameters, result, UninitializeRecordOnAgent, null).ConfigureAwait(false);

			var results = await Task.WhenAll(AgentBroker.Instance.GetBrokerLogs(true));

			foreach (var logResult in results)
			{
				string logFilename = logResult.AgentId + ".log";
				logFilename = string.Join("_", logFilename.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
				logFilename = Path.Combine(this.JournalAbsoluteSavePath, "logs", logFilename);

				Directory.CreateDirectory(Path.GetDirectoryName(logFilename));
				File.WriteAllText(logFilename, logResult.Result);
			}

			return result;
		}

		public bool Disengage(bool isStartMode)
		{
			if ((isStartMode && this.ProviderState < ProviderState.StartingRecord) || (!isStartMode && this.ProviderState > ProviderState.StoppingRecord))
				return true;

			var cts = _acquisitionTriggerCts;
			if (cts == null)
			{
				Log.Error().Message("---=== Unsubscription: acquisition is already {0} ===---", isStartMode ? "started" : "stopped").WithAgent(this.Id).Write();
				return false;
			}
			else
			{
				Log.Info().Message(string.Format("---=== Unsubscription: {0} ===---", this.SequenceId)).WithAgent(this.Id).Write();
				cts.Cancel();
			}

			return true;
		}

		public Task<bool> ValidateRecord(bool success, string comment)
		{
			if (this.ProviderState != ProviderState.InitializedRecord)
			{
				Log.Error().Message(string.Format("Cannot validate the acquisition since the current state is '{0}'. It should be equal to 'InitializedRecord'.", this.ProviderState)).WithAgent(this.Id).Write();
				return Task.FromResult(false);
			}

			var parameters = new ValidateRecordParameter();

			_acquisitionManagerStateSubject.OnNext(new AcquisitionManagerStateChangedResult { ProviderState = this.ProviderState, Parameters = parameters, Result = null });
			try
			{
				((AcquisitionManagerAgentEventJournal) this.EventJournal).SetFooterSpecificContent(success, comment);
				return Task.FromResult(true);
			}
			finally
			{
				_acquisitionManagerStateSubject.OnNext(new AcquisitionManagerStateChangedResult { ProviderState = this.ProviderState, Parameters = parameters, Result = new AcquisitionActionResult(this.SequenceId, AcquisitionStep.ValidateRecord, this.Id, this.DisplayData.Name) });
			}
		}

		#endregion

		#region IInternalAcquisitionManagerAgent members

		public void SaveDriversToConfig(IEnumerable<string> fullNames, string selectedFullName)
		{
			if (fullNames == null) throw new ArgumentNullException("fullNames");
			if (string.IsNullOrEmpty(selectedFullName)) throw new ArgumentNullException("selectedFullName");

			fullNames = fullNames.ToArray();
			this.Drivers.Clear();
			this.Drivers.AddRange(fullNames);

			AcquisitionConfigurationFactory.Instance.Update(this.ConfigurationFilePath, "Agent.Drivers", this.Drivers);
			AcquisitionConfigurationFactory.Instance.Update(this.ConfigurationFilePath, "Agent.SelectedDriver", selectedFullName);
		}

		public void SaveOperatorsToConfig(IEnumerable<string> fullNames, string selectedFullName)
		{
			if (fullNames == null) throw new ArgumentNullException("fullNames");
			if (string.IsNullOrEmpty(selectedFullName)) throw new ArgumentNullException("selectedFullName");

			fullNames = fullNames.ToArray();
			this.Operators.Clear();
			this.Operators.AddRange(fullNames);

			AcquisitionConfigurationFactory.Instance.Update(this.ConfigurationFilePath, "Agent.Operators", this.Operators);
			AcquisitionConfigurationFactory.Instance.Update(this.ConfigurationFilePath, "Agent.SelectedOperator", selectedFullName);
		}

		public void SaveSequenceTypeToConfig(string selectedSequenceType)
		{
			if (string.IsNullOrWhiteSpace(selectedSequenceType)) throw new ArgumentNullException("selectedSequenceType");

			AcquisitionConfigurationFactory.Instance.Update(this.ConfigurationFilePath, "Agent.SelectedSequenceType", selectedSequenceType);
		}

		#endregion
	}
}