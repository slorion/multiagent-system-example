using DLC.Framework.IO;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Modules.FileTransferModule
{
	public class FileSystemTransferProvider
		: FileTransferProvider
	{
		private readonly List<FileTransferProcessor> _ftProcessors = new List<FileTransferProcessor>();

		public List<FileTransferSettings> FileTransferSettings { get; set; }

		protected override void ValidateConfigurationCore()
		{
			base.ValidateConfigurationCore();

			if (this.FileTransferSettings == null) throw new InvalidOperationException("No configuration was provided.");
			if (this.FileTransferSettings.Any(setting => string.IsNullOrWhiteSpace(setting.SourceFolder))) throw new InvalidOperationException("A source folder is missing in the configuration.");

			var invalidDestinationFolders = this.FileTransferSettings
				.Where(setting => setting.DestinationFolders == null || setting.DestinationFolders.Count <= 0);

			if (invalidDestinationFolders.Any())
				throw new InvalidOperationException(string.Format("Missing destination folder for the following sources: {0}.", string.Join(",", invalidDestinationFolders)));
		}

		protected override Task<IObservable<FileTransferData>> InitializeCore()
		{
			return Task.Run(
				() =>
				{
					var errorSubject = new ReplaySubject<FileTransferData>();

					foreach (var setting in this.FileTransferSettings)
					{
						var processor = new FileTransferProcessor { Settings = setting };

						try
						{
							processor.Initialize();
						}
						catch (Exception ex)
						{
							Log.Error().Exception(ex).Write();

							errorSubject.OnNext(new FileTransferData {
								MachineName = Environment.MachineName,
								MonitoredFolderPath = setting.SourceFolder,
								Exception = ex
							});

							processor.Dispose();
							continue;
						}

						_ftProcessors.Add(processor);
					}

					return Observable.Merge(Enumerable.Concat(new[] { errorSubject }, _ftProcessors.Select(processor => processor.FtsSubject)));
				});
		}

		protected override async Task StartCore()
		{
			await base.StartCore().ConfigureAwait(false);

			StartMonitoring();
		}

		protected override async Task StopCore()
		{
			StopMonitoring();
			await StopTransferringCore(true).ConfigureAwait(false);

			await base.StopCore().ConfigureAwait(false);
		}

		protected override Task UninitializeCore()
		{
			foreach (var processor in _ftProcessors)
				processor.Dispose();
			_ftProcessors.Clear();

			return base.UninitializeCore();
		}

		protected override void StartTransferringCore()
		{
			foreach (var processor in _ftProcessors)
				processor.StartTransferring();
		}

		protected override Task StopTransferringCore()
		{
			return StopTransferringCore(false);
		}

		private async Task StopTransferringCore(bool force)
		{
			await Task.WhenAll(_ftProcessors.Select(ft => ft.StopTransferring(force))).ConfigureAwait(false);
			await DeleteEmptyFolders().ConfigureAwait(false);
		}

		private void StartMonitoring()
		{
			foreach (var processor in _ftProcessors)
				processor.StartMonitoring();
		}

		private void StopMonitoring()
		{
			foreach (var processor in _ftProcessors)
				processor.StopMonitoring();
		}

		private Task DeleteEmptyFolders()
		{
			return Task.Run(
				() =>
				{
					foreach (string sourceFolder in this.FileTransferSettings.Select(setting => setting.SourceFolder).Distinct())
						IOHelper.DeleteEmptyDirectories(sourceFolder, deleteRoot: false);
				});
		}

		protected override void DisposeCore(bool disposing)
		{
			if (_ftProcessors != null)
			{
				foreach (var processor in _ftProcessors)
					processor.Dispose();
				_ftProcessors.Clear();
			}

			base.DisposeCore(disposing);
		}
	}
}