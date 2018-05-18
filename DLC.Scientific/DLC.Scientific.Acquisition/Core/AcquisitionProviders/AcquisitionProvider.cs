using DLC.Framework.Reactive;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Core.AcquisitionProviders
{
	public abstract class AcquisitionProvider<TData>
		: IAcquisitionProvider
		where TData : ProviderData
	{
		private readonly List<IDisposable> _observers = new List<IDisposable>();

		private readonly BehaviorSubjectSlim<ProviderState> _providerStateSubject = new BehaviorSubjectSlim<ProviderState>(ProviderState.Created);
		private readonly SubjectSlim<CalibrationData> _calibrationSubject = new SubjectSlim<CalibrationData>();

		// The raw data source is not available until initialization is complete,
		// but since we want the DataSource property to be always available (incl. immediately after object construction),
		// we use a proxy subject
		private readonly SubjectSlim<TData> _dataSourceProxySubject = new SubjectSlim<TData>();

		private IObservable<TData> _rawDataSource;
		private IDisposable _rawDataSourceObserver;

		private CancellationTokenSource _currentStateTransitionCts;
		private Task _currentStateTransitionTask;

		public ProviderState State { get { return _providerStateSubject.Value; } }
		public IObservable<ProviderState> ProviderStateDataSource { get { return _providerStateSubject.DistinctUntilChanged().ObserveOn(NewThreadScheduler.Default); } }

		ProviderData IAcquisitionProvider.CurrentData { get { return this.CurrentData; } }
		IObservable<ProviderData> IAcquisitionProvider.DataSource { get { return this.DataSource; } }

		public IObservable<TData> DataSource { get { return _dataSourceProxySubject; } }
		public TData CurrentData { get; private set; }
		public long DataReceivedCount { get; private set; }

		public string SequenceId { get; set; }

		public CancellationToken CurrentStateTransitionCancellationToken
		{
			get
			{
				var cts = _currentStateTransitionCts;
				return cts == null ? CancellationToken.None : cts.Token;
			}
		}

		private Task CancelCurrentStateTransition()
		{
			var cts = _currentStateTransitionCts;
			var task = _currentStateTransitionTask ?? Task.FromResult(0);

			if (cts != null && !cts.IsCancellationRequested)
			{
				try
				{
					cts.Cancel();
				}
				catch (ObjectDisposedException) { }
			}

			return task;
		}

		protected void RegisterObserver(IDisposable observer)
		{
			if (observer == null) throw new ArgumentNullException("observer");

			_observers.Add(observer);
		}

		private Task MakeStateTransition(ProviderState startState, ProviderState intermediateState, ProviderState endState, Func<Task> action, Func<Task<bool>> before = null, Func<Task> after = null)
		{
			return MakeStateTransition(startState, intermediateState, endState,
				async () =>
				{
					if (action != null)
						await action().ConfigureAwait(false);

					return 0;
				},
				before, after);
		}

		private Task<TResult> MakeStateTransition<TResult>(ProviderState startState, ProviderState intermediateState, ProviderState endState, Func<Task<TResult>> action, Func<Task<bool>> before = null, Func<Task> after = null)
		{
			if (action == null) throw new ArgumentNullException("action");

			var originalState = this.State;

			if (originalState != startState)
				throw new InvalidStateTransitionException(string.Format("Current state '{0}' is invalid, it must be '{1}'.", originalState, startState));

			var cts = new CancellationTokenSource();
			_currentStateTransitionCts = cts;
			cts.Token.Register(() => _providerStateSubject.OnNext(originalState));

			Func<Task<TResult>> doTransition =
				async () =>
				{
					try
					{
						Log.Debug().Message("ProviderState intermediate transition: {0}->{1}.", originalState, intermediateState).Write();
						_providerStateSubject.OnNext(intermediateState);

						// if before function call fails, rollback and return immediately
						if (before != null && !(await before().ConfigureAwait(false)))
						{
							_providerStateSubject.OnNext(originalState);
							return default(TResult);
						}

						var result = await action().ConfigureAwait(false);

						if (after != null)
							await after().ConfigureAwait(false);

						Log.Debug().Message("ProviderState transition successful: {0}->{1}->{2}.", originalState, intermediateState, endState).Write();
						_providerStateSubject.OnNext(endState);

						return result;
					}
					catch
					{
						Log.Warn().Message("ProviderState transition failed: {0}->{1}->{2}.", originalState, intermediateState, endState).Write();
						_providerStateSubject.OnNext(ProviderState.Failed);
						throw;
					}
					finally
					{
						// will also dispose subscriptions to CTS cancellation token
						cts.Dispose();
					}
				};

			var task = doTransition();
			_currentStateTransitionTask = task;

			return task;
		}

		private void ValidateConfiguration()
		{
			ValidateConfigurationCore();
		}

		protected virtual void ValidateConfigurationCore() { }

		public Task Initialize(Func<Task<bool>> before = null, Func<Task> after = null)
		{
			ValidateConfiguration();

			return MakeStateTransition(ProviderState.Created, ProviderState.Initializing, ProviderState.Initialized,
				() => InitializeCore().ContinueWith(
					t =>
					{
						_rawDataSource = CreateDataSource(t.Result);

						if (_rawDataSource != null)
						{
							_rawDataSourceObserver = _rawDataSource.Subscribe(
								data => _dataSourceProxySubject.OnNext(data),
								error =>
								{
									_dataSourceProxySubject.OnError(error);
									Log.Error().Exception(error).Write();
									_providerStateSubject.OnNext(ProviderState.Failed);
								});
						}

						return t.Result;
					}, TaskContinuationOptions.OnlyOnRanToCompletion),
				before, after);
		}

		protected abstract Task<IObservable<TData>> InitializeCore();

		protected virtual IObservable<TData> CreateDataSource(IObservable<TData> rawDataSource) { return rawDataSource; }

		public Task Start(Func<Task<bool>> before = null, Func<Task> after = null)
		{
			return MakeStateTransition(ProviderState.Initialized, ProviderState.Starting, ProviderState.Started,
				() => StartCore().ContinueWith(
					t =>
					{
						this.RegisterObserver(this.DataSource.Subscribe(
							data =>
							{
								this.CurrentData = data;
								this.DataReceivedCount++;
							}));
					}, TaskContinuationOptions.OnlyOnRanToCompletion),
				before, after);
		}

		protected virtual Task StartCore() { return Task.FromResult(0); }

		public Task InitializeRecord(Func<Task<bool>> before = null, Func<Task> after = null)
		{
			if (this.CurrentFileTransfer != null && !(this.CurrentFileTransfer.IsCompleted || this.CurrentFileTransfer.IsCanceled || this.CurrentFileTransfer.IsFaulted))
				throw new InvalidOperationException("Acquisition cannot be prepared because an acquisition file transfer is pending.");

			return MakeStateTransition(ProviderState.Started, ProviderState.InitializingRecord, ProviderState.InitializedRecord, InitializeRecordCore, before, after);
		}

		protected virtual Task InitializeRecordCore() { return Task.FromResult(0); }

		public Task StartRecord(Func<Task<bool>> before = null, Func<Task> after = null)
		{
			return MakeStateTransition(ProviderState.InitializedRecord, ProviderState.StartingRecord, ProviderState.StartedRecord, StartRecordCore, before, after);
		}

		protected virtual Task StartRecordCore() { return Task.FromResult(0); }

		public Task StopRecord(Func<Task<bool>> before = null, Func<Task> after = null)
		{
			if (this.State < ProviderState.StartingRecord)
				return Task.FromResult(0);
			else if (this.State == ProviderState.StartingRecord)
				return CancelCurrentStateTransition();
			else
				return MakeStateTransition(ProviderState.StartedRecord, ProviderState.StoppingRecord, ProviderState.InitializedRecord, StopRecordCore, before, after);
		}

		protected virtual Task StopRecordCore() { return Task.FromResult(0); }

		public Task UninitializeRecord(Func<Task<bool>> before = null, Func<Task> after = null)
		{
			if (this.State < ProviderState.InitializingRecord)
				return Task.FromResult(0);
			else if (this.State == ProviderState.InitializingRecord)
				return CancelCurrentStateTransition();
			else
				return MakeStateTransition(ProviderState.InitializedRecord, ProviderState.UninitializingRecord, ProviderState.Started, UninitializeRecordCore, before, after);
		}

		protected virtual Task UninitializeRecordCore() { return Task.FromResult(0); }

		public Task Stop(Func<Task<bool>> before = null, Func<Task> after = null)
		{
			if (this.State < ProviderState.Starting)
				return Task.FromResult(0);
			else if (this.State == ProviderState.Starting)
				return CancelCurrentStateTransition();
			else
			{
				this.CurrentData = default(TData);
				this.DataReceivedCount = 0;

				return MakeStateTransition(ProviderState.Started, ProviderState.Stopping, ProviderState.Initialized, StopCore, before, after);
			}
		}

		protected virtual Task StopCore() { return Task.FromResult(0); }

		public Task Uninitialize(Func<Task<bool>> before = null, Func<Task> after = null)
		{
			if (this.State < ProviderState.Initializing)
				return Task.FromResult(0);
			else if (this.State == ProviderState.Initializing)
				return CancelCurrentStateTransition();
			else
			{
				return MakeStateTransition(ProviderState.Initialized, ProviderState.Uninitializing, ProviderState.Created,
					() => UninitializeCore().ContinueWith(
						t =>
						{
							if (_rawDataSourceObserver != null)
								_rawDataSourceObserver.Dispose();

							_rawDataSourceObserver = null;
							_rawDataSource = null;
						}, TaskContinuationOptions.OnlyOnRanToCompletion),
					before, after);
			}
		}

		protected virtual Task UninitializeCore() { return Task.FromResult(0); }

		public virtual bool CanCalibrate { get { return false; } }

		public IObservable<CalibrationData> CalibrationDataSource { get { return _calibrationSubject; } }

		public Task StartCalibration(Func<Task<bool>> before = null, Func<Task> after = null)
		{
			return MakeStateTransition(ProviderState.Started, ProviderState.Started, ProviderState.Calibrating, StartCalibrationCore, before, after);
		}

		protected virtual Task StartCalibrationCore() { return Task.FromResult(0); }

		public Task<CalibrationData> StopCalibration(Func<Task<bool>> before = null, Func<Task> after = null)
		{
			return MakeStateTransition(ProviderState.Calibrating, ProviderState.Calibrating, ProviderState.Started, StopCalibrationCore, before, after);
		}

		protected virtual Task<CalibrationData> StopCalibrationCore() { return null; }

		public Task<Exception> ProcessCalibrationData(bool isAccepted, CalibrationData calibrationData, Func<Task<bool>> before = null, Func<Task> after = null)
		{
			if (calibrationData == null) throw new ArgumentNullException("calibrationData");

			return MakeStateTransition(
				ProviderState.Started,
				ProviderState.Started,
				ProviderState.Started,
				async () =>
				{
					var result = await ProcessCalibrationDataCore(isAccepted, calibrationData).ConfigureAwait(false);

					if (isAccepted)
						_calibrationSubject.OnNext(calibrationData);

					return result;
				},
				before, after);
		}

		protected virtual Task<Exception> ProcessCalibrationDataCore(bool isAccepted, CalibrationData calibrationData) { return Task.FromResult(default(Exception)); }

		#region File Transfer stubs

		private readonly BehaviorSubjectSlim<FileTransferData> _fileTransferSubject = new BehaviorSubjectSlim<FileTransferData>();
		private readonly BehaviorSubjectSlim<bool> _isTransferringSubject = new BehaviorSubjectSlim<bool>(false);

		public string SaveFolderAbsolutePath { get; set; }
		public Task CurrentFileTransfer { get; protected set; }

		public FileTransferData LastTransferredFile
		{
			get { return _fileTransferSubject.Value; }
			set { _fileTransferSubject.OnNext(value); }
		}

		public IObservable<FileTransferData> FileTransferDataSource
		{
			get { return _fileTransferSubject.Where(data => data != null); }
		}

		public bool IsTransferring
		{
			get { return _isTransferringSubject.Value; }
			protected set { _isTransferringSubject.OnNext(value); }
		}

		public IObservable<bool> IsTransferringDataSource
		{
			get { return _isTransferringSubject.DistinctUntilChanged(); }
		}

		#endregion

		#region IDisposable members

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (this.State == ProviderState.Disposed)
				return;

			if (disposing && this.State != ProviderState.Created)
				Log.Warn().Message("Dispose called without first calling Stop and/or Uninitialize.").Write();

			if (_observers != null)
			{
				foreach (var observer in _observers)
					observer.Dispose();
				_observers.Clear();
			}

			if (_rawDataSourceObserver != null)
				_rawDataSourceObserver.Dispose();

			this.DisposeCore(disposing);

			if (disposing)
			{
#pragma warning disable 4014
				MakeStateTransition(this.State, this.State, ProviderState.Disposed, null);
#pragma warning restore 4014
			}
		}

		protected virtual void DisposeCore(bool disposing) { }

		~AcquisitionProvider()
		{
			Log.Warn().Message("Object was not disposed correctly.").Write();
			Dispose(false);
		}

		#endregion
	}
}