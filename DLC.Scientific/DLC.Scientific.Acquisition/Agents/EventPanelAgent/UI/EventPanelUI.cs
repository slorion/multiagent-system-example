using DLC.Framework.Win32;
using DLC.Multiagent;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using DLC.Scientific.Acquisition.Core.Agents;
using DLC.Scientific.Acquisition.Core.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls.Themes;
using Telerik.WinControls.UI;

namespace DLC.Scientific.Acquisition.Agents.EventPanelAgent.UI
{
	public partial class EventPanelUI
		: AcquisitionStickyForm
	{
		private readonly LowLevelKeyboardHook _keyboardHook = new LowLevelKeyboardHook();
		private bool _hotkeyModeEnabled;

		private BindingList<RoadEventData> _log = new BindingList<RoadEventData>();

		private new IInternalEventPanelAgent ParentAgent { get { return (IInternalEventPanelAgent) base.ParentAgent; } }

		public EventPanelUI()
			: base()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			// hotkey mode

			lblHotkeyMode.Text = string.Format("Shortcut mode {0} ({1})", _hotkeyModeEnabled ? "activated" : "deactivated", this.ParentAgent.ToggleHotkeyModeKey);
			picHotkeyMode.Image = _hotkeyModeEnabled ? ImageResources.HotkeyModeEnabled : ImageResources.HotkeyModeDisabled;

			_keyboardHook.KeyDown += (ss, ee) => HandleHotkey((Keys) ee.KeyCode);
			_keyboardHook.KeyDownCancelOpportunity += (ss, ee) => ee.Cancel = _hotkeyModeEnabled;
			_keyboardHook.Hook();

			// setup Log grid

			using (var theme = new VisualStudio2012DarkTheme())
				gridLog.ThemeName = theme.ThemeName;

			gridLog.AllowAddNewRow = false;
			gridLog.AllowDeleteRow = false;
			gridLog.AllowEditRow = false;
			gridLog.AllowAutoSizeColumns = true;
			gridLog.AllowColumnResize = true;
			gridLog.ShowFilteringRow = false;
			gridLog.ShowGroupPanel = false;
			gridLog.AutoGenerateColumns = true;
			gridLog.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;

			gridLog.DataSource = _log;

			foreach (var column in gridLog.Columns)
				column.IsVisible = false;

			gridLog.Columns.Move(gridLog.Columns["Id"].Index, 0);
			gridLog.Columns["Id"].TextAlignment = ContentAlignment.MiddleCenter;
			gridLog.Columns["Id"].HeaderText = "ID";
			gridLog.Columns["Id"].IsVisible = true;
			gridLog.Columns["Id"].Width = 20;

			gridLog.Columns.Move(gridLog.Columns["Timestamp"].Index, 1);
			gridLog.Columns["Timestamp"].TextAlignment = ContentAlignment.MiddleCenter;
			gridLog.Columns["Timestamp"].HeaderText = "Time";
			gridLog.Columns["Timestamp"].IsVisible = true;
			gridLog.Columns["Timestamp"].Width = 48;
			gridLog.Columns["Timestamp"].FormatString = "{0:HH:mm:ss}";

			gridLog.Columns.Move(gridLog.Columns["Distance"].Index, 2);
			gridLog.Columns["Distance"].TextAlignment = ContentAlignment.MiddleCenter;
			gridLog.Columns["Distance"].HeaderText = "Distance";
			gridLog.Columns["Distance"].IsVisible = true;
			gridLog.Columns["Distance"].Width = 51;

			gridLog.Columns.Move(gridLog.Columns["Description"].Index, 3);
			gridLog.Columns["Description"].TextAlignment = ContentAlignment.MiddleCenter;
			gridLog.Columns["Description"].HeaderText = "Description";
			gridLog.Columns["Description"].IsVisible = true;
			gridLog.Columns["Description"].MinWidth = 82;
			gridLog.Columns["Description"].AutoSizeMode = BestFitColumnMode.DisplayedCells;

			gridLog.Columns.Move(gridLog.Columns["Severity"].Index, 4);
			gridLog.Columns["Severity"].TextAlignment = ContentAlignment.MiddleCenter;
			gridLog.Columns["Severity"].HeaderText = "Severity";
			gridLog.Columns["Severity"].IsVisible = true;
			gridLog.Columns["Severity"].MinWidth = 48;
			gridLog.Columns["Severity"].AutoSizeMode = BestFitColumnMode.DisplayedCells;

			gridLog.Columns.Move(gridLog.Columns["DistanceManualCorrection"].Index, 5);
			gridLog.Columns["DistanceManualCorrection"].TextAlignment = ContentAlignment.MiddleCenter;
			gridLog.Columns["DistanceManualCorrection"].HeaderText = "Correction";
			gridLog.Columns["DistanceManualCorrection"].IsVisible = true;
			gridLog.Columns["DistanceManualCorrection"].MinWidth = 54;
			gridLog.Columns["DistanceManualCorrection"].AutoSizeMode = BestFitColumnMode.DisplayedCells;

			gridLog.Columns.Move(gridLog.Columns["Comment"].Index, 6);
			gridLog.Columns["Comment"].TextAlignment = ContentAlignment.MiddleCenter;
			gridLog.Columns["Comment"].HeaderText = "Comment";
			gridLog.Columns["Comment"].IsVisible = true;
			gridLog.Columns["Comment"].AutoSizeMode = BestFitColumnMode.DisplayedCells;
			gridLog.Columns["Comment"].WrapText = true;

			gridLog.AutoSizeRows = true;
			gridLog.BestFitColumns();

			this.RegisterObserver(
				AgentBroker.Instance.ObserveOne<RoadEventData>(this.ParentAgent.Id, "DataSource")
					.ObserveOn(WindowsFormsSynchronizationContext.Current)
					.Subscribe(
						data =>
						{
							_log.Insert(0, data);
							gridLog.Rows[0].IsSelected = true;
							gridLog.BestFitColumns();
						}));

			// load RoadEvent groups
			foreach (var roadEvent in this.ParentAgent.ReadRoadEventConfiguration())
			{
				var groupControl = new RoadEventGroupControl(roadEvent.RoadEventDataDisplayInfos);
				groupControl.RoadEventClicked +=
					(ss, ee) =>
					{
						if (this.ParentAgent.ProviderState >= ProviderState.StartedRecord)
						{
							var data = this.ParentAgent.CloneRoadEvent(ee.Value.RoadEventDataTemplate);

							if (!ee.Value.ShowEditDialog || (ee.Value.ShowEditDialog && ShowEditLogDataDialog(data) == DialogResult.OK))
							{
#pragma warning disable 4014
								this.ParentAgent.OnRoadEvent(data, isNew: true);
#pragma warning restore 4014
							}
						}
					};

				pnlEvents.Controls.Add(groupControl);
			}

			// set enabled/disabled
			SetEnabledState(this.ParentAgent.ProviderState >= ProviderState.InitializedRecord);

			this.RegisterObserver(
				AgentBroker.Instance.ObserveOne<ProviderState>(this.ParentAgent.Id, "ProviderStateDataSource", ignoreAgentState: true)
					.ObserveOn(WindowsFormsSynchronizationContext.Current)
					.Subscribe(
						async state =>
						{
							SetEnabledState(this.ParentAgent.ProviderState >= ProviderState.InitializedRecord);

							if (this.ParentAgent.ProviderState < ProviderState.InitializedRecord)
								SetHotkeyMode(false);

							if (state <= ProviderState.InitializingRecord)
							{
								_log.Clear();

								var roadEventControlsToReset = pnlEvents.Controls.OfType<RoadEventGroupControl>()
									.Where(currentControl => !currentControl.RoadEventGroup.Any(item => item.RoadEventDataTemplate.IsSnapshot))
									.Where(control => control.ActiveRoadEvent != control.RoadEventGroup.First());

								foreach (var roadEventInfo in roadEventControlsToReset)
									roadEventInfo.SetOrToggleActiveRoadEvent(roadEventInfo.RoadEventGroup.First());
							}
							else if (state == ProviderState.StartedRecord)
							{
								// write initial state of each road event groups, except those that are snapshots
								var allRoadEventControls = pnlEvents.Controls.OfType<RoadEventGroupControl>();
								var noSnapshotRoadEventControls = allRoadEventControls.Where(currentControl => !currentControl.RoadEventGroup.Any(item => item.RoadEventDataTemplate.IsSnapshot));

								foreach (var roadEventInfo in noSnapshotRoadEventControls)
								{
									var data = this.ParentAgent.CloneRoadEvent(roadEventInfo.ActiveRoadEvent.RoadEventDataTemplate);
									await this.ParentAgent.OnRoadEvent(data, isNew: true, progress: 0);
								}
							}
						}));
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			if (gridLog != null)
				gridLog.BestFitColumns();
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);

			pnlEventAndLog.Orientation = this.ParentAgent.UIOrientation;
			chkOrientation.Checked = pnlEventAndLog.Orientation == Orientation.Horizontal;

			if (pnlEventAndLog.Orientation == Orientation.Horizontal)
				pnlEventAndLog.SplitterDistance = this.ParentAgent.SplitterDistanceHorizontalMode;
			else
				pnlEventAndLog.SplitterDistance = this.ParentAgent.SplitterDistanceVerticalMode;
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			base.OnFormClosing(e);

			if (pnlEventAndLog.Orientation == Orientation.Horizontal)
				this.ParentAgent.SplitterDistanceHorizontalMode = pnlEventAndLog.SplitterDistance;
			else
				this.ParentAgent.SplitterDistanceVerticalMode = pnlEventAndLog.SplitterDistance;
		}

		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			base.OnFormClosed(e);

			if (_keyboardHook != null)
				_keyboardHook.UnHook();
		}

		private void gridLog_CellDoubleClick(object sender, GridViewCellEventArgs e)
		{
			if (e.RowIndex > -1 && e.Row.DataBoundItem != null)
				EditLogData((RoadEventData) gridLog.Rows[e.RowIndex].DataBoundItem);
		}

		private void btnEditLog_Click(object sender, EventArgs e)
		{
			if (gridLog.CurrentRow != null)
				EditLogData((RoadEventData) gridLog.CurrentRow.DataBoundItem);
		}

		private void chkOrientation_Click(object sender, EventArgs e)
		{
			if (chkOrientation.Checked)
			{
				this.ParentAgent.SplitterDistanceVerticalMode = pnlEventAndLog.SplitterDistance;
				chkOrientation.Text = "Horizontal Mode";
				pnlEventAndLog.Orientation = Orientation.Horizontal;
				pnlEventAndLog.SplitterDistance = this.ParentAgent.SplitterDistanceHorizontalMode;
			}
			else
			{
				this.ParentAgent.SplitterDistanceHorizontalMode = pnlEventAndLog.SplitterDistance;
				chkOrientation.Text = "Vertical Mode";
				pnlEventAndLog.Orientation = Orientation.Vertical;
				pnlEventAndLog.SplitterDistance = this.ParentAgent.SplitterDistanceVerticalMode;
			}

			this.ParentAgent.UIOrientation = pnlEventAndLog.Orientation;
		}

		private void picHotkeyMode_Click(object sender, EventArgs e)
		{
			var enabled = (this.ParentAgent.ProviderState >= ProviderState.InitializedRecord) && !_hotkeyModeEnabled;
			SetHotkeyMode(enabled);
		}

		private void lblHotkeyMode_Click(object sender, EventArgs e)
		{
			var enabled = (this.ParentAgent.ProviderState >= ProviderState.InitializedRecord) && !_hotkeyModeEnabled;
			SetHotkeyMode(enabled);
		}

		private void HandleHotkey(Keys key)
		{
			if (key == this.ParentAgent.ToggleHotkeyModeKey)
			{
				var enabled = (this.ParentAgent.ProviderState >= ProviderState.InitializedRecord) && !_hotkeyModeEnabled;
				SetHotkeyMode(enabled);
			}
			else
			{
				if (_hotkeyModeEnabled)
				{
					var roadEventToActivate = (
							from groupControl in pnlEvents.Controls.OfType<RoadEventGroupControl>()
							from roadEvent in groupControl.RoadEventGroup
							where roadEvent.Hotkey == key
							select new { GroupControl = groupControl, RoadEvent = roadEvent }
						).FirstOrDefault();

					if (roadEventToActivate != null)
						roadEventToActivate.GroupControl.SetOrToggleActiveRoadEvent(roadEventToActivate.RoadEvent);
				}
			}
		}

		private void SetHotkeyMode(bool enabled)
		{
			_hotkeyModeEnabled = enabled;

			lblHotkeyMode.Text = string.Format("Shortcut mode {0} ({1})", enabled ? "activated" : "deactivated", this.ParentAgent.ToggleHotkeyModeKey);
			lblHotkeyMode.ForeColor = enabled ? Color.Lime : Color.LightGray;
			picHotkeyMode.Image = enabled ? ImageResources.HotkeyModeEnabled : ImageResources.HotkeyModeDisabled;
			this.ParentAgent.OnHotkeyModeChanged(enabled);
		}

		private void EditLogData(RoadEventData data)
		{
			if (data == null) throw new ArgumentNullException("data");

			data = this.ParentAgent.CloneRoadEvent(data);

			if (ShowEditLogDataDialog(data) == DialogResult.OK)
			{
#pragma warning disable 4014
				this.ParentAgent.OnRoadEvent(data, isNew: false);
#pragma warning restore 4014
			}
		}

		private DialogResult ShowEditLogDataDialog(RoadEventData data)
		{
			var hotkeyModeEnabled = _hotkeyModeEnabled;
			_hotkeyModeEnabled = false;
			try
			{
				using (var dialog = new EditLogDialog(data))
				{
					return dialog.ShowDialog();
				}
			}
			finally
			{
				_hotkeyModeEnabled = hotkeyModeEnabled;
			}
		}

		private void SetEnabledState(bool enabled)
		{
			foreach (var roadEventControl in pnlEvents.Controls.OfType<RoadEventGroupControl>())
				roadEventControl.Enabled = enabled;
		}
	}
}