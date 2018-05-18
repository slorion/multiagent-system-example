using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using DLC.Scientific.Acquisition.Modules.BgrModule;
using DLC.Scientific.Acquisition.Modules.LocalisationModule;
using DLC.Scientific.Core.Geocoding.Bgr;
using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DLC.Scientific.Acquisition.Tools.ItineraryGenerator
{
	public partial class MainUI
		: Form
	{
		class BgrFiltersDisplay
		{
			public BgrDataTypes Value { get; set; }
			public string Text { get; set; }

			public override string ToString()
			{
				return this.Text;
			}
		}

		private CancellationTokenSource _generationCts;

		public MainUI()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			clstAllowedBGRDataTypes.Items.AddRange(
				new[] {
					new BgrFiltersDisplay { Value = BgrDataTypes.Routes, Text = "Routes" },
					new BgrFiltersDisplay { Value = BgrDataTypes.Bretelles, Text = "Bretelles" },
					new BgrFiltersDisplay { Value = BgrDataTypes.CarrefoursGiratoires, Text = "Carrefours giratoires" }
				});

			clstAllowedBGRDataTypes.SetItemChecked(0, Properties.Settings.Default.AllowedBgrDataTypes.HasFlag(BgrDataTypes.Routes));
			clstAllowedBGRDataTypes.SetItemChecked(1, Properties.Settings.Default.AllowedBgrDataTypes.HasFlag(BgrDataTypes.Bretelles));
			clstAllowedBGRDataTypes.SetItemChecked(2, Properties.Settings.Default.AllowedBgrDataTypes.HasFlag(BgrDataTypes.CarrefoursGiratoires));
		}

		private void MainUI_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
		}

		private void MainUI_DragDrop(object sender, DragEventArgs e)
		{
			var files = (string[]) e.Data.GetData(DataFormats.FileDrop);
			lstInputFiles.Items.AddRange(files);
		}

		private void lstInputFiles_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
			{
				for (int i = lstInputFiles.Items.Count - 1; i >= 0; i--)
				{
					if (lstInputFiles.GetSelected(i))
						lstInputFiles.Items.RemoveAt(i);
				}
			}
			else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.A)
			{
				lstInputFiles.BeginUpdate();

				for (int i = 0; i < lstInputFiles.Items.Count; i++)
					lstInputFiles.SetSelected(i, true);

				lstInputFiles.EndUpdate();
			}
		}

		private void btnBrowseDestination_Click(object sender, EventArgs e)
		{
			using (var dlg = new FolderBrowserDialog())
			{
				dlg.Description = "Select ITI files destination folder";
				dlg.ShowNewFolderButton = true;

				if (dlg.ShowDialog(this) == DialogResult.OK)
					txtDestination.Text = dlg.SelectedPath;
			}
		}

		private async void btnGenerateItiFiles_Click(object sender, EventArgs e)
		{
			if (_generationCts != null)
			{
				try
				{
					_generationCts.Cancel();
					return;
				}
				catch (ObjectDisposedException) { }
			}

			if (lstInputFiles.Items.Count <= 0)
				return;

			if (string.IsNullOrWhiteSpace(txtDestination.Text))
			{
				MessageBox.Show(this, "Destination folder must be provided.", "Validation error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			try
			{
				using (_generationCts = new CancellationTokenSource())
				{
					btnGenerateItiFiles.Text = "Stop generation";

					var files = lstInputFiles.Items.Cast<string>();
					var destinationFolderPath = txtDestination.Text.Trim();

					var gpsLogGap = Properties.Settings.Default.GpsLogGap;

					var bgrConnectionString = Properties.Settings.Default.BgrConnectionString;
					var autoCorrectDelta = Properties.Settings.Default.AutoCorrectDelta;
					var minSearchRadiusInMeters = Properties.Settings.Default.MinSearchRadiusInMeters;
					var maxSearchRadiusInMeters = Properties.Settings.Default.MaxSearchRadiusInMeters;
					var directionBufferSize = Properties.Settings.Default.DirectionBufferSize;

					// SelectedItems property is not updated correctly if items are selected programmatically,
					// so we manually obtain the selected items
					var allowedBgrDataTypes = clstAllowedBGRDataTypes.Items
						.Cast<BgrFiltersDisplay>()
						.Where((item, i) => clstAllowedBGRDataTypes.GetItemChecked(i))
						.Select(f => f.Value)
						.Aggregate((a, b) => a | b);

					Directory.CreateDirectory(destinationFolderPath);

					var ctx = WindowsFormsSynchronizationContext.Current;

					foreach (var file in files)
					{
						_generationCts.Token.ThrowIfCancellationRequested();

						try { File.Delete(Path.Combine(destinationFolderPath, Path.ChangeExtension(Path.GetFileName(file), ".bgrtrace"))); }
						catch { }

						lblCurrentGeneratedFile.Text = file;
						pbGeneration.Value = 0;
						pbGeneration.Maximum = int.MaxValue;

						using (var gps = new GpxFileReaderProvider())
						using (var bgr = new PgsqlBgrProvider())
						{
							gps.FilePath = file;
							gps.ReaderFrequencyInMs = 0;
							gps.InitialDelayInMs = 10;

							bgr.LocalisationDataSource = gps.DataSource.Where((data, index) => index % gpsLogGap == 0);

							bgr.AllowSkipIfProcessing = false;
							bgr.ConnectionString = bgrConnectionString;
							bgr.MinSearchRadiusInMeters = minSearchRadiusInMeters;
							bgr.MaxSearchRadiusInMeters = maxSearchRadiusInMeters;
							bgr.DirectionBufferSize = directionBufferSize;
							bgr.AllowedBgrDataTypes = allowedBgrDataTypes;
							bgr.UseGpsTime = rdoGeocodageDateGps.Checked;

							var iti = bgr.DataSource
								.Materialize()
								.Select(
									notification =>
									{
										if (gps.DataReceivedCount >= gps.DataSourceCount - gpsLogGap)
											return Notification.CreateOnCompleted<BgrData>();
										else
											return notification;
									})
								.Dematerialize()
								.DistinctUntilChanged(data => (int?) data.Rtssc.Chainage)
								.Select(data => data.Rtssc)
								.GetItineraryFromBgrTrace()
								.AutoCorrectItinerary(autoCorrectDelta)
								.ConvertToItiFormat();

							using (bgr.DataSource.Subscribe(data => File.AppendAllLines(Path.Combine(destinationFolderPath, Path.ChangeExtension(Path.GetFileName(file), ".bgrtrace")), new[] { string.Format("{0} - {1}", data.Rtssc, data.Rtssc.Direction) })))
							using (var output = new StreamWriter(Path.Combine(destinationFolderPath, Path.ChangeExtension(Path.GetFileName(file), ".iti"))))
							using (iti.Subscribe(line => output.WriteLine(line)))
							using (gps.DataSource.Take(1).ObserveOn(WindowsFormsSynchronizationContext.Current).Subscribe(_ => { pbGeneration.Maximum = (int) gps.DataSourceCount; }))
							using (gps.DataSource.ObserveOn(WindowsFormsSynchronizationContext.Current).Subscribe(_ => pbGeneration.Value = (int) gps.DataReceivedCount))
							{
								await Task.Run(
									async () =>
									{
										await bgr.Initialize().ConfigureAwait(false);
										await bgr.Start().ConfigureAwait(false);
										await gps.Initialize().ConfigureAwait(false);
										await gps.Start().ConfigureAwait(false);

										await iti.LastOrDefaultAsync().ToTask(_generationCts.Token).ConfigureAwait(false);

										await bgr.Stop().ConfigureAwait(false);
										await bgr.Uninitialize().ConfigureAwait(false);
										await gps.Stop().ConfigureAwait(false);
										await gps.Uninitialize().ConfigureAwait(false);
									});
							}
						}
					}

					MessageBox.Show(this, "The generation was successful.", "Generation result", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			catch (OperationCanceledException)
			{
				MessageBox.Show("The generation was canceled.", "Generation result", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, string.Format("The generation has failed with the following error:\n '{0}'.", ex), "Generation result", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				btnGenerateItiFiles.Text = "Generate ITI files";
				_generationCts = null;
			}
		}
	}
}