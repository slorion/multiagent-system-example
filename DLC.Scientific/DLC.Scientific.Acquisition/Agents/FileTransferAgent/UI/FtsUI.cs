using CodeBits;
using DLC.Framework.Reactive;
using DLC.Multiagent;
using DLC.Scientific.Acquisition.Agents.FileTransferAgent.Properties;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using DLC.Scientific.Acquisition.Core.Agents;
using DLC.Scientific.Acquisition.Core.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.Themes;
using Telerik.WinControls.UI;

namespace DLC.Scientific.Acquisition.Agents.FileTransferAgent.UI
{
	public partial class FtsUI
		: AcquisitionStickyForm
	{
		class GroupedFileTransferData
		{
			private readonly HashSet<string> _filesInError = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);

			public string MachineName { get; set; }
			public string MonitoredFolderPath { get; set; }
			public string DestinationFolderPath { get; set; }

			public int RemainingFileCount { get; set; }
			public long CopiedBytes { get; set; }
			public long TotalBytes { get; set; }

			public HashSet<string> FilesInError { get { return _filesInError; } }
		}

		private new IFileTransferManagerAgent ParentAgent { get { return (IFileTransferManagerAgent) base.ParentAgent; } }

		private readonly Color _defaultProgressBarColor = Color.FromArgb(255, 51, 153, 255);

		private readonly DataTable _fileInfos;
		private readonly Dictionary<int, DataRow> _fileInfosIndex = new Dictionary<int, DataRow>();
		private IDisposable _fileTransferSubscription;

		public FtsUI()
			: base()
		{
			InitializeComponent();

			_fileInfos = new DataTable("fileInfos");
			_fileInfos.Columns.Add(new DataColumn("machine", typeof(string)) { Caption = "Machine" });
			_fileInfos.Columns.Add(new DataColumn("sourceFolder", typeof(string)) { Caption = "Source Folder" });
			_fileInfos.Columns.Add(new DataColumn("destinationFolder", typeof(string)) { Caption = "Destination Folder" });
			_fileInfos.Columns.Add(new DataColumn("progressBar", typeof(double)) { Caption = "Progress" });
			_fileInfos.Columns.Add(new DataColumn("remainingFileCount", typeof(int)) { Caption = "remainingFileCount (internal)" });
			_fileInfos.Columns.Add(new DataColumn("copiedBytes", typeof(long)) { Caption = "copiedBytes (internal)" });
			_fileInfos.Columns.Add(new DataColumn("totalBytes", typeof(long)) { Caption = "totalBytes (internal)" });
			_fileInfos.Columns.Add(new DataColumn("hasErrors", typeof(bool)) { Caption = "State" });
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			using (var theme = new VisualStudio2012DarkTheme())
				gridFiles.ThemeName = theme.ThemeName;

			gridFiles.DataSource = _fileInfos.DefaultView;
			gridFiles.Columns["remainingFileCount"].IsVisible = false;
			gridFiles.Columns["copiedBytes"].IsVisible = false;
			gridFiles.Columns["totalBytes"].IsVisible = false;

			gridFiles.ShowFilteringRow = false;
			gridFiles.ShowGroupPanel = false;

			gridFiles.AutoExpandGroups = !this.ParentAgent.AutoCollapseGrid;
			gridFiles.GroupDescriptors.Add("machine", ListSortDirection.Ascending);
			gridFiles.GroupDescriptors.Add("sourceFolder", ListSortDirection.Ascending);

			var groupsExpanded = new Dictionary<DataGroup, bool>();

			this.RegisterObserver(
				AgentBroker.Instance.ObserveOne<bool>(this.ParentAgent.Id, "IsTransferringDataSource")
					.ObserveOn(WindowsFormsSynchronizationContext.Current)
					.Subscribe(
						isTransferring =>
						{
							if (isTransferring)
							{
								var subscription = _fileTransferSubscription;
								if (subscription != null)
									subscription.Dispose();

								_fileTransferSubscription = SubscribeToFileTransferDataSource();
							}
							else
							{
								_fileInfos.Rows.Clear();
								groupsExpanded.Clear();
							}
						}));

			// fix a bug where the grid will expand all groups when a new row is added

			_fileInfos.RowChanging +=
				(ss, ee) =>
				{
					if (ee.Action == DataRowAction.Add)
					{
						foreach (DataGroup group in RadGridViewHelper.GetGroups(gridFiles.Groups))
						{
							if (groupsExpanded.ContainsKey(group))
								groupsExpanded[group] = group.IsExpanded;
							else
								groupsExpanded[group] = gridFiles.AutoExpandGroups;
						}
					}
				};

			_fileInfos.RowChanged +=
				(ss, ee) =>
				{
					if (ee.Action == DataRowAction.Add)
					{
						foreach (var ge in groupsExpanded)
						{
							if (ge.Value)
								ge.Key.Expand();
							else
								ge.Key.Collapse();
						}
					}
				};
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			base.OnFormClosing(e);

			if (!e.Cancel)
			{
				if (_fileTransferSubscription != null)
					_fileTransferSubscription.Dispose();
			}
		}

		private void gridFiles_CreateCell(object sender, GridViewCreateCellEventArgs e)
		{
			if (e.CellType == typeof(GridGroupContentCellElement))
				e.CellElement = new CustomGroupCellElement(e.Column, e.Row);
		}

		//TODO: review UI reset of cells (they are reused by the gridview)
		private void gridFiles_CellFormatting(object sender, CellFormattingEventArgs e)
		{
			if (e.Row.DataBoundItem == null)
				return;

			e.CellElement.ToolTipText = e.CellElement.Text;

			if (string.Equals(e.CellElement.ColumnInfo.FieldName, "progressBar", StringComparison.Ordinal))
			{
				RadProgressBarElement pbElement;
				if (e.CellElement.Children.Count == 0)
				{
					pbElement = new RadProgressBarElement {
						SmoothingMode = SmoothingMode.AntiAlias,
						Padding = new Padding(5),
						StretchHorizontally = true
					};

					e.CellElement.Children.Add(pbElement);
				}
				else
				{
					pbElement = e.CellElement.Children[0] as RadProgressBarElement;
					pbElement.Visibility = ElementVisibility.Visible;
				}

				var dataRowView = e.Row.DataBoundItem as DataRowView;

				if (dataRowView != null && dataRowView.Row.RowState != DataRowState.Detached)
				{
					var progress = (double) dataRowView.Row["progressBar"];
					var remainingFileCount = (int) dataRowView.Row["remainingFileCount"];
					var copiedBytes = (long) dataRowView.Row["copiedBytes"];
					var totalBytes = (long) dataRowView.Row["totalBytes"];

					progress *= 100;

					pbElement.Value1 = Convert.ToInt32(progress);
					pbElement.Text = string.Format("{0} remaining ({1}/{2})", remainingFileCount, ByteSizeFriendlyName.Build(copiedBytes), ByteSizeFriendlyName.Build(totalBytes));

					pbElement.IndicatorElement1.BackColor = progress < 100 ? _defaultProgressBarColor : Color.LimeGreen;
				}

				e.CellElement.DrawText = false;
				e.CellElement.DrawImage = false;
			}
			else
			{
				if (e.CellElement.Children.Count > 0)
					e.CellElement.Children[0].Visibility = ElementVisibility.Hidden;

				if (string.Equals(e.CellElement.ColumnInfo.FieldName, "hasErrors", StringComparison.Ordinal))
				{
					e.CellElement.DrawImage = true;
					e.CellElement.DrawText = false;

					var row = ((DataRowView) e.Row.DataBoundItem).Row;

					if (row.RowState != DataRowState.Detached)
					{
						if ((bool) row["hasErrors"])
							e.CellElement.Image = Resources.ExclamationRed;
						else
							e.CellElement.Image = Resources.CheckedGreen;
					}
				}
				else
				{
					e.CellElement.DrawImage = false;
					e.CellElement.DrawText = true;
				}
			}
		}

		private void gridFiles_CellDoubleClick(object sender, GridViewCellEventArgs e)
		{
			if (e.Row.DataBoundItem == null)
				return;

			if (e.Column.Name == "sourceFolder" || e.Column.Name == "destinationFolder")
			{
				var folderPath = (string) ((DataRowView) e.Row.DataBoundItem).Row[e.Column.Name];
				try
				{
					if (!string.IsNullOrWhiteSpace(folderPath) && Directory.Exists(folderPath))
						Process.Start(folderPath);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
		}

		private void UpdateDisplay(int key, GroupedFileTransferData data)
		{
			if (data == null) throw new ArgumentNullException("data");

			DataRow row;
			if (!_fileInfosIndex.TryGetValue(key, out row))
				row = _fileInfos.NewRow();

			if (row.RowState != DataRowState.Detached)
				row.BeginEdit();

			try
			{
				row["machine"] = data.MachineName;
				row["sourceFolder"] = data.MonitoredFolderPath;
				row["destinationFolder"] = data.DestinationFolderPath;
				row["progressBar"] = data.TotalBytes <= 0 ? 0D : (double) data.CopiedBytes / (double) data.TotalBytes;
				row["remainingFileCount"] = data.RemainingFileCount;
				row["hasErrors"] = data.FilesInError.Count > 0;

				// maj total avant copied pour éviter que copied > total, ce qui donne une progression > 100%
				row["totalBytes"] = data.TotalBytes;
				row["copiedBytes"] = data.CopiedBytes;
			}
			finally
			{
				row.EndEdit();
			}

			if (row.RowState == DataRowState.Detached)
			{
				_fileInfos.Rows.Add(row);
				_fileInfosIndex[key] = row;
			}
		}

		private IDisposable SubscribeToFileTransferDataSource()
		{
			return
				AgentBroker.Instance.ObserveOne<FileTransferData>(this.ParentAgent.Id, "FileTransferDataSource")
				// groupe par MachineName + MonitoredPath + DestinationFolder
					.GroupBy(data => Tuple.Create(data.MachineName, data.MonitoredFolderPath, data.DestinationFolderPath))
					.Select(g =>
						new {
							g.Key,
							Value =
								// groupe par nom de fichier
								g.GroupBy(data => data.FileName)
								// conserve les deux derniers événements
								.SelectMany(gg => gg.WithPrevious())
								// accumule en temps réel le total des CopiedBytes et TotalBytes pour le groupe principal
								.Scan(
									new GroupedFileTransferData(),
									(acc, cur) =>
									{
										// pour un nouveau fichier (1er événement qu'on reçoit), ajoute Total/CopiedBytes au total pour le groupe
										// pour les événements suivants pour ce même fichier, ajoute seulement le delta des Total/CopiedBytes
										// en théorie, TotalBytes ne devrait jamais changer, mais le fichier pourrait être remplacé, etc.

										if (cur.Item1 == null)
										{
											acc.MachineName = cur.Item2.MachineName;
											acc.MonitoredFolderPath = cur.Item2.MonitoredFolderPath;
											acc.DestinationFolderPath = cur.Item2.DestinationFolderPath;

											acc.RemainingFileCount++;
											acc.CopiedBytes += cur.Item2.CopiedBytes;
											acc.TotalBytes += cur.Item2.TotalBytes;
										}
										else
										{
											acc.CopiedBytes += cur.Item2.CopiedBytes - cur.Item1.CopiedBytes;
											acc.TotalBytes += cur.Item2.TotalBytes - cur.Item1.TotalBytes;
										}

										// la vérification doit être faite ici car le fichier peut être complété dès le premier événement
										if (cur.Item2.Exception == null && cur.Item2.CopiedBytes >= cur.Item2.TotalBytes)
											acc.RemainingFileCount--;

										if (cur.Item2.Exception == null)
											acc.FilesInError.Remove(cur.Item2.FileName);
										else
											acc.FilesInError.Add(cur.Item2.FileName);

										return acc;
									})
						})
				// applanit les groupes en tuples clé/valeur
					.SelectMany(g => g.Value.Select(data => new { g.Key, Value = data }))
				// accumule les événements pendant un certain temps pour pouvoir ensuite ne garder que le plus récent de chaque groupe
				// si on ne procède pas ainsi, la grille sera mise à jour trop souvent et l'interface va geler
					.Buffer(TimeSpan.FromSeconds(2))
					.ObserveOn(WindowsFormsSynchronizationContext.Current)
					.Subscribe(
						buffer =>
						{
							_fileInfos.BeginLoadData();
							try
							{
								// obtient l'événement le plus récent de chaque groupe et met à jour la grille avec celui-ci
								foreach (var acc in buffer.GroupBy(g => g.Key).Select(g => g.Last()))
									UpdateDisplay(acc.Key.GetHashCode(), acc.Value);
							}
							finally
							{
								_fileInfos.EndLoadData();
							}
						});
		}
	}
}