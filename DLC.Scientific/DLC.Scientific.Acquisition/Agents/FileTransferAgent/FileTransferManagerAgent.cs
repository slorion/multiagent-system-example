using DLC.Framework.Reactive;
using DLC.Multiagent;
using DLC.Scientific.Acquisition.Agents.FileTransferAgent.Configuration;
using DLC.Scientific.Acquisition.Agents.FileTransferAgent.UI;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using DLC.Scientific.Acquisition.Core.Agents;
using DLC.Scientific.Acquisition.Core.Agents.Model;
using DLC.Scientific.Acquisition.Core.Configuration;
using DLC.Scientific.Core.Agents;
using DLC.Scientific.Core.Journalisation;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Agents.FileTransferAgent
{
	public class FileTransferManagerAgent
		: AcquisitionableAgent<ManualProvider<ProviderData>, ProviderData, FileTransferManagerAgentConfiguration, AcquisitionModuleConfiguration>, IFileTransferManagerAgent, IVisibleAgent
	{
		private readonly BehaviorSubjectSlim<bool> _isTransferringSubject = new BehaviorSubjectSlim<bool>(false);
		private readonly SubjectSlim<FileTransferData> _fileTransferSubject = new SubjectSlim<FileTransferData>();

		public bool AutoCollapseGrid { get; private set; }
		public override int Priority { get { return 100; } }

		protected override void ConfigureAgent()
		{
			base.ConfigureAgent();

			this.AutoCollapseGrid = this.Configuration.Agent.AutoCollapseGrid;

			this.AutoShowUI = this.Configuration.Agent.AutoShowUI;
			this.MainUITypeName = typeof(FtsUI).AssemblyQualifiedName;
			this.MainUIAgentTypeName = typeof(IFileTransferManagerAgent).AssemblyQualifiedName;
			this.AgentUniversalName = "FileTransferManagerAgent";
		}

		protected override ManualProvider<ProviderData> CreateAndConfigureProvider()
		{
			return new ManualProvider<ProviderData>();
		}

		protected override IEventJournal CreateEventJournal(InitializeRecordParameter parameters)
		{
			return null;
		}

		protected override async Task<AcquisitionActionResult> InitializeCore(InitializeAcquisitionParameter parameters, AcquisitionActionResult result)
		{
			result = await base.InitializeCore(parameters, result).ConfigureAwait(false);

			// get initial value of IsTransferring
			await Task.WhenAll(AgentBroker.Instance.TryExecuteOnAll<IFileTransferAgent, bool>(a => a.IsTransferring).GetValueOrDefault())
				.ContinueWith(t => _isTransferringSubject.OnNext(t.Result.Any(_ => _)), TaskContinuationOptions.OnlyOnRanToCompletion)
				.ConfigureAwait(false);

			return result;
		}

		protected override async Task<AcquisitionActionResult> StartCore(StartAcquisitionParameter parameters, AcquisitionActionResult result)
		{
			result = await base.StartCore(parameters, result).ConfigureAwait(false);

			this.RegisterObserver(
				AgentBroker.Instance.ObserveAll<IFileTransferAgent, FileTransferData>("FileTransferDataSource")
					.SelectMany(t => t.Item2)
					.Subscribe(_fileTransferSubject),
				AcquisitionStep.Stop);

			var acquisitionManagerState = await AgentBroker.Instance.TryExecuteOnFirst<IAcquisitionManagerAgent, ProviderState>(a => a.ProviderState).GetValueOrDefault().ConfigureAwait(false);

#pragma warning disable 4014
			if (acquisitionManagerState <= ProviderState.Started)
				StartTransferring();
#pragma warning restore 4014

			// start transferring when AcquisitionManager is not recording (ProviderState <= Started)
			this.RegisterObserver(
				AgentBroker.Instance.ObserveAny<IAcquisitionManagerAgent, ProviderState>("ProviderStateDataSource")
					.Where(state => state <= ProviderState.Started && this.OperationalState == OperationalAgentStates.None)
					.Subscribe(_ => StartTransferring()),
				AcquisitionStep.Stop);

			// for each new FileTransferAgent, start transferring if applicable
			this.RegisterObserver(
				AgentBroker.Instance.ObserveAll<IFileTransferAgent, ProviderState>("ProviderStateDataSource")
					.SelectMany(t => t.Item2.Select(state => Tuple.Create(t.Item1, state)))
					.Where(t => t.Item2 == ProviderState.Started)
					.Subscribe(
						t =>
						{
							if (this.IsTransferring)
								AgentBroker.Instance.TryExecuteOnOne<IFileTransferAgent>(t.Item1.AgentId, a => a.StartTransferring());
						}),
				AcquisitionStep.Stop);

			return result;
		}

		protected override async Task<AcquisitionActionResult> InitializeRecordCore(InitializeRecordParameter parameters, AcquisitionActionResult result)
		{
			result = await base.InitializeRecordCore(parameters, result).ConfigureAwait(false);
			await StopTransferring().ConfigureAwait(false);

			return result;
		}

		protected override async Task<AcquisitionActionResult> UninitializeCore(UninitializeAcquisitionParameter parameters, AcquisitionActionResult result)
		{
			await StopTransferring().ConfigureAwait(false);

			return await base.UninitializeCore(parameters, result).ConfigureAwait(false);
		}

		public IObservable<FileTransferData> FileTransferDataSource { get { return _fileTransferSubject; } }

		public IObservable<bool> IsTransferringDataSource { get { return _isTransferringSubject.DistinctUntilChanged(); } }
		public bool IsTransferring { get { return _isTransferringSubject.Value; } }

		public Task<ExecutionResult[]> StartTransferring()
		{
			lock (_isTransferringSubject)
			{
				if (_isTransferringSubject.Value)
					return Task.FromResult(new[] { new ExecutionResult() });
				else
				{
					_isTransferringSubject.OnNext(true);
					return Task.WhenAll(AgentBroker.Instance.TryExecuteOnAll<IFileTransferAgent>(a => a.StartTransferring()));
				}
			}
		}

		public Task<ExecutionResult[]> StopTransferring()
		{
			return Task.WhenAll(AgentBroker.Instance.TryExecuteOnAll<IFileTransferAgent>(a => a.StopTransferring()))
				.ContinueWith(
					t =>
					{
						_isTransferringSubject.OnNext(false);
						return t.Result;
					},
					TaskContinuationOptions.OnlyOnRanToCompletion);
		}
	}
}