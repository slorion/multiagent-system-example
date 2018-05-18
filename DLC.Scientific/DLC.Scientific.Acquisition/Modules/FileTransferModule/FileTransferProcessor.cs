using DLC.Framework.IO;
using DLC.Framework.IO.Monitoring;
using DLC.Framework.Reactive;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using NLog.Fluent;
using System;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Modules.FileTransferModule
{
	internal class FileTransferProcessor
		: IDisposable
	{
		private readonly object _monitorLock = new object();
		private readonly object _transferLock = new object();
		private readonly object _scanLock = new object();

		private readonly DeferredSubject<FileTransferData> _fileTransferSubject = new SubjectSlim<FileTransferData>().ToDeferred();

		private readonly ManualResetEventSlim _newFileEvent = new ManualResetEventSlim();

		private FileMonitor _monitor;
		private IDisposable _monitorObserver;

		private Task _fileTransferTask = Task.FromResult(0);
		private Task _fileScanTask = Task.FromResult(0);

		private CancellationTokenSource _fileScanCts = new CancellationTokenSource();
		private CancellationTokenSource _fileTranferLoopCts = new CancellationTokenSource();
		private CancellationTokenSource _fileCopyCts = new CancellationTokenSource();

		internal FileTransferSettings Settings { get; set; }
		internal IObservable<FileTransferData> FtsSubject { get { return _fileTransferSubject; } }

		internal void Initialize()
		{
			ValidateConfig();

			_monitor = new FileMonitor(this.Settings.SourceFolder, filter: this.Settings.Filter, watchedChangeTypes: WatcherChangeTypes.Created | WatcherChangeTypes.Changed);
			_monitor.IncludeSubdirectories = true;

			_monitorObserver =
				_monitor.FileChangedDataSource
					.ObserveOn(TaskPoolScheduler.Default)
					.Subscribe(
						fileEvent =>
						{
							if (fileEvent.ChangeType == WatcherChangeTypes.Created)
							{
								foreach (var folder in this.Settings.DestinationFolders)
								{
									var newFile = new FileTransferData {
										MachineName = Environment.MachineName,
										MonitoredFolderPath = _monitor.Path,
										DestinationFolderPath = folder,
										FileName = fileEvent.Name,
										TotalBytes = new FileInfo(fileEvent.FullPath).Length
									};

									_fileTransferSubject.OnNext(newFile);
								}
							}

							_newFileEvent.Set();
						},
						exception =>
						{
							Log.Warn().Exception(exception).Write();

							var errorData = new FileTransferData {
								MachineName = Environment.MachineName,
								MonitoredFolderPath = _monitor.Path,
								Exception = exception
							};

							_fileTransferSubject.OnNext(errorData);
						});
		}

		private bool ValidateConfig()
		{
			if (string.IsNullOrWhiteSpace(this.Settings.SourceFolder)) throw new InvalidOperationException("SourceFolder is mandatory.");
			if (this.Settings.DestinationFolders.Count == 0) throw new InvalidOperationException("DestinationFolders is mandatory.");

			return true;
		}

		internal void StartMonitoring()
		{
			lock (_monitorLock)
			{
				_monitor.StartWatch();
			}
		}

		internal void StopMonitoring()
		{
			lock (_monitorLock)
			{
				_fileScanCts.Cancel();
				_monitor.StopWatch();
			}
		}

		internal void StartTransferring()
		{
			lock (_transferLock)
			{
				StartMonitoring();

				if (_fileTransferTask.IsCanceled || _fileTransferTask.IsFaulted || _fileTransferTask.IsCompleted)
				{
					_fileScanCts = new CancellationTokenSource();
					ScanExistingFilesToProcess(_fileScanCts.Token);

					_fileTranferLoopCts = new CancellationTokenSource();
					_fileCopyCts = new CancellationTokenSource();

					var fileTransferLoopCt = _fileTranferLoopCts.Token;
					var fileCopyCt = _fileCopyCts.Token;

					_fileTransferTask = Task.Run(
						async () =>
						{
							while (!fileTransferLoopCt.IsCancellationRequested)
							{
								foreach (var file in SafeFileEnumerator.EnumerateFiles(this.Settings.SourceFolder, this.Settings.Filter, SearchOption.AllDirectories))
								{
									if (fileTransferLoopCt.IsCancellationRequested)
										break;

									await TransferFile(file, fileCopyCt).ConfigureAwait(false);
								}

								try
								{
									// force a resynchronisation every 30 seconds
									// and wait an additional short delay to let Windows release handles
									// (e.g. acquired by FileMonitor)
									if (_newFileEvent.Wait(TimeSpan.FromSeconds(30), fileTransferLoopCt))
										await Task.Delay(TimeSpan.FromMilliseconds(250)).ConfigureAwait(false);
								}
								catch (OperationCanceledException)
								{
								}

								_newFileEvent.Reset();
							}
						});
				}
			}
		}

		internal Task StopTransferring(bool force = false)
		{
			lock (_transferLock)
			{
				StopMonitoring();

				_fileScanCts.Cancel();
				_fileTranferLoopCts.Cancel();

				if (force)
					_fileCopyCts.Cancel();

				return _fileTransferTask;
			}
		}

		private Task ScanExistingFilesToProcess(CancellationToken ct)
		{
			lock (_scanLock)
			{
				if (_fileScanTask.IsCanceled || _fileScanTask.IsCompleted || _fileScanTask.IsFaulted)
				{
					_fileScanTask = Task.Run(
						() =>
						{
							foreach (string filePath in SafeFileEnumerator.EnumerateFiles(this.Settings.SourceFolder, this.Settings.Filter, SearchOption.AllDirectories))
							{
								if (ct.IsCancellationRequested)
									break;

								FileInfo sourceInfo;
								try
								{
									sourceInfo = new FileInfo(filePath);
								}
								catch (IOException)
								{
									continue;
								}

								foreach (var folder in this.Settings.DestinationFolders)
								{
									// do not stop the processing here in case of cancellation
									// because we must stay coherent when sending events by file
									// i.e. we must be careful not to send a partial destination list for a file
									var destInfo = new FileInfo(GetDestinationFilePath(filePath, folder));

									if (!IOHelper.AreSameFile(sourceInfo, destInfo))
									{
										var newFileData = new FileTransferData {
											MachineName = Environment.MachineName,
											MonitoredFolderPath = _monitor.Path,
											DestinationFolderPath = folder,
											FileName = sourceInfo.Name,
											TotalBytes = sourceInfo.Length
										};

										_fileTransferSubject.OnNext(newFileData);
									}
								}
							}
						});
				}
			}

			return _fileScanTask;
		}

		private async Task TransferFile(string sourceFilePath, CancellationToken ct)
		{
			if (string.IsNullOrEmpty(sourceFilePath)) throw new ArgumentNullException("sourceFilePath");

			var sourceFilename = Path.GetFileName(sourceFilePath);
			if (string.IsNullOrEmpty(sourceFilename)) throw new InvalidOperationException("sourceFilePath does not contain a filename.");

			try
			{
				var results = await Task.WhenAll(
					this.Settings.DestinationFolders.Select(
						async destinationFolder =>
						{
							try
							{
								string destinationFilePath = GetDestinationFilePath(sourceFilePath, destinationFolder);

								await IOHelper.Copy(sourceFilePath, destinationFilePath, OverwriteMode.OverwriteIfDifferent, CopyOptions.AllowHardLinkCreation | CopyOptions.DisableBuffering, ct,
									(copiedBytes, totalBytes) =>
									{
										var fileProgressData = new FileTransferData {
											MachineName = Environment.MachineName,
											MonitoredFolderPath = _monitor.Path,
											DestinationFolderPath = destinationFolder,
											FileName = Path.GetFileName(sourceFilePath),
											CopiedBytes = copiedBytes,
											TotalBytes = totalBytes
										};

										_fileTransferSubject.OnNext(fileProgressData);
									}).ConfigureAwait(false);

								return true;
							}
							catch (OperationCanceledException)
							{
								return false;
							}
							catch (Exception ex)
							{
								// level set to Trace because it generates a lot of events
								Log.Trace().Exception(ex).Write();

								long fileLength = 0;
								try
								{
									fileLength = new FileInfo(sourceFilePath).Length;
								}
								catch { }

								var errorData = new FileTransferData {
									MachineName = Environment.MachineName,
									DestinationFolderPath = destinationFolder,
									MonitoredFolderPath = _monitor.Path,
									FileName = Path.GetFileName(sourceFilePath),
									TotalBytes = fileLength,
									Exception = ex
								};

								_fileTransferSubject.OnNext(errorData);

								return false;
							}
						})).ConfigureAwait(false);

				if (!ct.IsCancellationRequested && results.All(result => result))
					File.Delete(sourceFilePath);
			}
			catch (Exception ex)
			{
				Log.Warn().Exception(ex).Write();
			}
		}

		private string GetDestinationFilePath(string sourceFilePath, string destinationFolderPath)
		{
			if (string.IsNullOrEmpty(sourceFilePath)) throw new ArgumentNullException("sourceFilePath");
			if (string.IsNullOrEmpty(destinationFolderPath)) throw new ArgumentNullException("destinationFolderPath");

			string relativePath = Path.GetDirectoryName(sourceFilePath.Substring(this.Settings.SourceFolder.Length, sourceFilePath.Length - this.Settings.SourceFolder.Length));
			return Path.Combine(destinationFolderPath, relativePath.TrimStart('/', '\\'), Path.GetFileName(sourceFilePath));
		}

		#region IDisposable members

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_fileTransferSubject != null)
				_fileTransferSubject.Dispose();

			if (_monitorObserver != null)
				_monitorObserver.Dispose();

			if (_monitor != null)
				_monitor.Dispose();

			if (_fileScanCts != null)
			{
				_fileScanCts.Cancel();
				_fileScanCts.Dispose();
			}

			if (_fileCopyCts != null)
			{
				_fileCopyCts.Cancel();
				_fileCopyCts.Dispose();
			}

			if (_fileTranferLoopCts != null)
			{
				_fileTranferLoopCts.Cancel();
				_fileTranferLoopCts.Dispose();
			}
		}

		~FileTransferProcessor()
		{
			Log.Warn().Message("Object was not disposed correctly.").Write();
			Dispose(false);
		}

		#endregion
	}
}