using DLC.Multiagent;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using DLC.Scientific.Acquisition.Core.Agents;
using DLC.Scientific.Acquisition.Core.UI;
using DLC.Scientific.Core.Geocoding.Bgr;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls.Themes;
using Telerik.WinControls.UI;

namespace DLC.Scientific.Acquisition.Agents.BgrDirectionalAgent.UI
{
	public partial class BgrUI
		: AcquisitionStickyForm
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

		private new IBgrDirectionalAgent ParentAgent { get { return (IBgrDirectionalAgent) base.ParentAgent; } }

		private readonly object _lock = new object();
		private readonly DataTable _rtsscTable = new DataTable();
		private readonly HashSet<string> _routes = new HashSet<string>();
		private int _routesGate;
		private long _userKeySequence = 0x1000000000;

		public BgrUI()
		{
			InitializeComponent();

			btnSplit.FlatStyle = FlatStyle.Flat;
			btnSplit.Image = ImageResources.Split;

			btnRowUp.FlatStyle = FlatStyle.Flat;
			btnRowUp.Image = ImageResources.Up;

			btnRowDown.FlatStyle = FlatStyle.Flat;
			btnRowDown.Image = ImageResources.Down;

			btnDelete.FlatStyle = FlatStyle.Flat;
			btnDelete.Image = ImageResources.Delete;

			btnRefresh.FlatStyle = FlatStyle.Flat;
			btnRefresh.Image = ImageResources.Refresh;

			_rtsscTable.Columns.Add(new DataColumn("key", typeof(long)) { Caption = "key", DefaultValue = 0 });
			_rtsscTable.Columns.Add(new DataColumn("direction", typeof(string)) { Caption = "Direction", MaxLength = 1, DefaultValue = "0" });
			_rtsscTable.Columns.Add(new DataColumn("voie", typeof(string)) { Caption = "Voie", MaxLength = 1, DefaultValue = "1" });
			_rtsscTable.Columns.Add(new DataColumn("route", typeof(string)) { Caption = "Route", MaxLength = 5 });
			_rtsscTable.Columns.Add(new DataColumn("troncon", typeof(string)) { Caption = "Tronçon", MaxLength = 2 });
			_rtsscTable.Columns.Add(new DataColumn("section", typeof(string)) { Caption = "Section", MaxLength = 3 });
			_rtsscTable.Columns.Add(new DataColumn("sousRoute", typeof(string)) { Caption = "Sous-route", MaxLength = 4 });
			_rtsscTable.Columns.Add(new DataColumn("chainageDebut", typeof(int)) { Caption = "Start", DefaultValue = 0 });
			_rtsscTable.Columns.Add(new DataColumn("chainageFin", typeof(int)) { Caption = "End", DefaultValue = 0 });
			_rtsscTable.Columns.Add(new DataColumn("longueur", typeof(double)) { Caption = "Length", DefaultValue = 0 });
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

			clstAllowedBGRDataTypes.SetItemChecked(0, this.ParentAgent.AllowedBgrDataTypes.HasFlag(BgrDataTypes.Routes));
			clstAllowedBGRDataTypes.SetItemChecked(1, this.ParentAgent.AllowedBgrDataTypes.HasFlag(BgrDataTypes.Bretelles));
			clstAllowedBGRDataTypes.SetItemChecked(2, this.ParentAgent.AllowedBgrDataTypes.HasFlag(BgrDataTypes.CarrefoursGiratoires));

			gridRtssc.AutoGenerateColumns = false;
			gridRtssc.Columns.AddRange(
				new GridViewComboBoxColumn("direction") { HeaderText = "Direction", TextAlignment = ContentAlignment.MiddleCenter, DataSource = new[] { "0", "1", "2" } },
				new GridViewDecimalColumn("voie") { HeaderText = "Voie", TextAlignment = ContentAlignment.MiddleCenter, DecimalPlaces = 0, Minimum = 0, NullValue = "1" },
				new GridViewComboBoxColumn("route") { HeaderText = "Route", TextAlignment = ContentAlignment.MiddleCenter },
				new GridViewComboBoxColumn("troncon") { HeaderText = "Tronçon", TextAlignment = ContentAlignment.MiddleCenter },
				new GridViewComboBoxColumn("section") { HeaderText = "Section", TextAlignment = ContentAlignment.MiddleCenter },
				new GridViewComboBoxColumn("sousRoute") { HeaderText = "Sous-route", TextAlignment = ContentAlignment.MiddleCenter },
				new GridViewDecimalColumn("chainageDebut") { HeaderText = "Start", TextAlignment = ContentAlignment.MiddleCenter, DecimalPlaces = 0, Minimum = 0, NullValue = "0" },
				new GridViewDecimalColumn("chainageFin") { HeaderText = "End", TextAlignment = ContentAlignment.MiddleCenter, DecimalPlaces = 0, Minimum = 0, NullValue = "0" },
				new GridViewDecimalColumn("longueur") { HeaderText = "Length", TextAlignment = ContentAlignment.MiddleCenter, DecimalPlaces = 0, ReadOnly = true });

			gridRtssc.DataSource = _rtsscTable.DefaultView;

			gridRtssc.MultiSelect = false;
			gridRtssc.EnableSorting = false;
			gridRtssc.ShowFilteringRow = false;
			gridRtssc.ShowGroupPanel = false;

			gridRtssc.AllowAutoSizeColumns = true;
			gridRtssc.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
			gridRtssc.EnableAlternatingRowColor = true;
			gridRtssc.MultiSelect = true;

			gridRtssc.ThemeName = new VisualStudio2012DarkTheme().ThemeName;

			this.RegisterObserver(
				AgentBroker.Instance.ObserveOne<BgrData>(this.ParentAgent.Id, "DataSource")
					.Where(_ => this.ParentAgent.ProviderState >= ProviderState.StartedRecord)
					.DistinctUntilChanged(data => (int?) data.Rtssc.Chainage)
					.Select(data => data.Rtssc)
					.GetItineraryFromBgrTrace()
					.AutoCorrectItinerary(this.ParentAgent.AutoCorrectDelta)
					.ObserveOn(WindowsFormsSynchronizationContext.Current)
					.Subscribe(
						t =>
						{
							var key = t.Item1;
							var rtssc = t.Item2;

							DataRow row = _rtsscTable.Rows.Count > 0 ? _rtsscTable.Rows[0] : null;
							if (row == null || (long) row["key"] != key)
							{
								row = _rtsscTable.NewRow();
								row["key"] = key;
								row["direction"] = (int) rtssc.Direction;
								row["voie"] = rtssc.Voie;
								row["route"] = rtssc.Route;
								row["troncon"] = rtssc.Troncon;
								row["section"] = rtssc.Section;
								row["sousRoute"] = rtssc.SousRoute;
								row["chainageDebut"] = t.Item3;
								row["longueur"] = rtssc.Longueur;

								_rtsscTable.Rows.InsertAt(row, 0);
							}

							if (t.Item4 != (int) row["chainageFin"])
								row["chainageFin"] = t.Item4;
						}));

			IDisposable timerSubscription = null;
			this.RegisterObserver(
				AgentBroker.Instance.ObserveOne<ProviderState>(this.ParentAgent.Id, "ProviderStateDataSource")
					.ObserveOn(WindowsFormsSynchronizationContext.Current)
					.Subscribe(
						state =>
						{
							bool canEdit = (state >= ProviderState.Started && state <= ProviderState.InitializedRecord);

							gridRtssc.AllowRowReorder = canEdit;
							gridRtssc.AllowAddNewRow = canEdit;
							gridRtssc.AllowDeleteRow = canEdit;

							btnSplit.Enabled = (state == ProviderState.StartedRecord);
							btnRowUp.Enabled = canEdit;
							btnRowDown.Enabled = canEdit;
							btnDelete.Enabled = canEdit;
							btnRefresh.Enabled = canEdit;

							foreach (var column in gridRtssc.Columns)
								column.ReadOnly = !canEdit;
							gridRtssc.Columns["voie"].ReadOnly = false;

							if (state == ProviderState.StartingRecord)
							{
								if (this.ParentAgent.AutoSearchIntervalInMs > 0)
								{
									// cache value to avoid calling repeatedly the agent
									var radius = this.ParentAgent.AutoSearchRadiusInMeters;

									timerSubscription = Observable.Timer(DateTimeOffset.Now, TimeSpan.FromMilliseconds(this.ParentAgent.AutoSearchIntervalInMs))
										.Subscribe(_ => RefreshRoutes(radius));
								}
							}
							else if (state < ProviderState.StartingRecord)
							{
								var sub = timerSubscription;
								if (sub != null)
									sub.Dispose();
							}

							if (state == ProviderState.UninitializingRecord)
							{
								// in case last gathered data has not been saved via RowDeleted/Changed
								// also ensure that the data file exists even if no RTSSC has been received
								SaveToFile(state, ensureExists: true);

								_rtsscTable.Rows.Clear();
							}
						}));

			_rtsscTable.RowDeleted += (ss, ee) => SaveToFile(this.ParentAgent.ProviderState);
			_rtsscTable.RowChanged += (ss, ee) => SaveToFile(this.ParentAgent.ProviderState);
		}

		private void clstAllowedBGRDataTypes_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			var filter = (BgrFiltersDisplay) clstAllowedBGRDataTypes.Items[e.Index];

			if (e.NewValue == CheckState.Checked)
				this.ParentAgent.AllowedBgrDataTypes |= filter.Value;
			else
				this.ParentAgent.AllowedBgrDataTypes &= ~filter.Value;
		}

		private void gridRtssc_CellValueChanged(object sender, GridViewCellEventArgs e)
		{
			if (e.Column.FieldName == "route") goto route;
			else if (e.Column.FieldName == "troncon") goto troncon;
			else if (e.Column.FieldName == "section") goto section;
			else if (e.Column.FieldName == "sousRoute") goto sousRoute;
			else return;

route:
			e.Row.Cells["troncon"].Value = null;
troncon:
			e.Row.Cells["section"].Value = null;
section:
			e.Row.Cells["sousRoute"].Value = null;
sousRoute:
			e.Row.Cells["chainageDebut"].Value = null;
			e.Row.Cells["chainageFin"].Value = null;
			e.Row.Cells["longueur"].Value = null;

			if (e.Column.FieldName == "sousRoute")
			{
				var route = Convert.ToString(e.Row.Cells["route"].Value);
				var troncon = Convert.ToString(e.Row.Cells["troncon"].Value);
				var section = Convert.ToString(e.Row.Cells["section"].Value);
				var sousRoute = Convert.ToString(e.Row.Cells["sousRoute"].Value);

				if (!string.IsNullOrEmpty(route) && !string.IsNullOrEmpty(troncon) && !string.IsNullOrEmpty(section) && !string.IsNullOrEmpty(sousRoute))
					e.Row.Cells["longueur"].Value = this.ParentAgent.GetSectionLength(new Rtssc(route, troncon, section, sousRoute));
			}
		}

		private void gridRtssc_CellBeginEdit(object sender, GridViewCellCancelEventArgs e)
		{
			var column = e.Column as GridViewComboBoxColumn;

			if (column == null)
				return;

			var route = Convert.ToString(e.Row.Cells["route"].Value);
			var troncon = Convert.ToString(e.Row.Cells["troncon"].Value);
			var section = Convert.ToString(e.Row.Cells["section"].Value);

			if (e.Column.FieldName == "route")
			{
				column.DataSource = _routes.OrderBy(_ => _);
			}
			else if (e.Column.FieldName == "troncon")
			{
				if (string.IsNullOrEmpty(route))
					column.DataSource = Enumerable.Empty<string>();
				else
				{
					column.DataSource =
						this.ParentAgent.GetRtssFromRoute(route)
							.Select(rtssc => rtssc.Troncon)
							.Distinct()
							.OrderBy(_ => _);
				}
			}
			else if (e.Column.FieldName == "section")
			{
				if (string.IsNullOrEmpty(route) || string.IsNullOrEmpty(troncon))
					column.DataSource = Enumerable.Empty<string>();
				else
				{
					column.DataSource =
						this.ParentAgent.GetRtssFromRoute(route)
							.Where(rtssc => rtssc.Troncon == troncon)
							.Select(rtssc => rtssc.Section)
							.Distinct()
							.OrderBy(_ => _);
				}
			}
			else if (e.Column.FieldName == "sousRoute")
			{
				if (string.IsNullOrEmpty(route) || string.IsNullOrEmpty(troncon) || string.IsNullOrEmpty("section"))
					column.DataSource = Enumerable.Empty<string>();
				else
				{
					column.DataSource =
						this.ParentAgent.GetRtssFromRoute(route)
							.Where(rtssc => rtssc.Troncon == troncon)
							.Where(rtssc => rtssc.Section == section)
							.Select(rtssc => rtssc.SousRoute)
							.Distinct()
							.OrderBy(_ => _);
				}
			}
		}

		private void btnSplit_Click(object sender, EventArgs e)
		{
			var row = _rtsscTable.Rows.Cast<DataRow>().FirstOrDefault();
			if (row != null)
			{
				var newRow = _rtsscTable.NewRow();
				newRow.ItemArray = row.ItemArray;
				newRow.SetField<int>("chainageDebut", newRow.Field<int>("chainageFin"));

				// update the key with a unique value outside of calculated key values
				_userKeySequence++;
				row.SetField<long>("key", row.Field<long>("key") + _userKeySequence);

				_rtsscTable.Rows.InsertAt(newRow, 0);
			}
		}

		private void btnRowUp_Click(object sender, EventArgs e)
		{
			MoveSelectedRow(i => i - 1);
		}

		private void btnRowDown_Click(object sender, EventArgs e)
		{
			MoveSelectedRow(i => i + 1);
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			foreach (var row in gridRtssc.SelectedRows.ToArray())
				row.Delete();
		}

		private void btnRefresh_Click(object sender, EventArgs e)
		{
			RefreshRoutes(this.ParentAgent.ManualSearchRadiusInMeters);
		}

		private Task RefreshRoutes(double searchRadiusInMeters)
		{
			var gate = Interlocked.CompareExchange(ref _routesGate, 0, 1);

			if (gate != 0)
				return Task.FromResult(0);
			else
				return AgentBroker.Instance.TryExecuteOnFirst<ILocalisationAgent, LocalisationData>(a => a.CurrentData)
					.ContinueWith(
						t =>
						{
							if (t.IsCompleted && t.Result.IsSuccessful && t.Result.Result != null)
							{
								var routes = this.ParentAgent.SelectRoutes(t.Result.Result.CorrectedData.PositionData, searchRadiusInMeters, null);

								foreach (var route in routes)
									_routes.Add(route);
							}

							_routesGate = 0;
						});
		}

		private void MoveSelectedRow(Func<int, int> getNewIndex)
		{
			if (getNewIndex == null) throw new ArgumentNullException("getNewIndex");

			var gridRow = gridRtssc.SelectedRows.FirstOrDefault();
			if (gridRow != null)
			{
				var oldIndex = gridRow.Index;
				var newIndex = getNewIndex(oldIndex);

				if (newIndex < 0 || newIndex > _rtsscTable.Rows.Count)
					return;

				var row = ((DataRowView) gridRow.DataBoundItem).Row;
				var newRow = _rtsscTable.NewRow();
				newRow.ItemArray = row.ItemArray;
				_rtsscTable.Rows.RemoveAt(oldIndex);
				_rtsscTable.Rows.InsertAt(newRow, newIndex);
			}
		}

		private void SaveToFile(ProviderState currentProviderState, bool ensureExists = false)
		{
			if (currentProviderState >= ProviderState.UninitializingRecord)
			{
				var filename = Path.Combine(this.ParentAgent.JournalAbsoluteSavePath, this.ParentAgent.ItineraryLogRelativeFilePath);

				if (ensureExists)
				{
					lock (_lock)
					{
						using (var fs = new FileStream(filename, FileMode.OpenOrCreate)) { }
					}
				}

				var iti = _rtsscTable.Rows.Cast<DataRow>()
					.Reverse()
					.Select(r => Tuple.Create(
						r.Field<long>("key"),
						(IRtssc) new Rtssc {
							Route = r.Field<string>("route"),
							Troncon = r.Field<string>("troncon"),
							Section = r.Field<string>("section"),
							SousRoute = r.Field<string>("sousRoute"),
							Longueur = (long) r.Field<double>("longueur"),
							Voie = Convert.ToInt32(r.Field<string>("voie") ?? r.Table.Columns["voie"].DefaultValue),
							Direction = (DirectionBgr) Convert.ToInt32(r.Field<string>("direction") ?? r.Table.Columns["direction"].DefaultValue)
						},
						(double) r.Field<int>("chainageDebut"),
						(double) r.Field<int>("chainageFin")))
					.ToObservable();

				File.WriteAllLines(filename, RtsscHelper.ConvertToItiFormat(iti).ToEnumerable());
			}
		}
	}
}