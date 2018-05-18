using DLC.Multiagent;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using DLC.Scientific.Acquisition.Core.Agents;
using DLC.Scientific.Acquisition.Core.Agents.Model;
using DLC.Scientific.Acquisition.Core.UI;
using DLC.Scientific.Core.Agents;
using DLC.Scientific.Core.Geocoding.Bgr;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls.UI;

namespace DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI
{
	public partial class AcquisitionManagerUI
		: AcquisitionStickyForm
	{
		private new IInternalAcquisitionManagerAgent ParentAgent { get { return (IInternalAcquisitionManagerAgent) base.ParentAgent; } }

		private IDisposable _bgrSubscription;
		private IEnumerable<IRtssc> _rtssFromRouteStart;
		private IEnumerable<IRtssc> _rtssFromRouteStop;

		private BindingList<string> _drivers;
		private BindingList<string> _operators;
		private BindingList<string> _sequenceTypes;
		private BindingList<Tuple<string, string>> _vehicles;

		private readonly Point _preparePanelLocation;
		private readonly Size _formSize;
		private readonly Size _panelSize;
		private DataView _dvAcquisitionTriggerModesStart;
		private DataView _dvAcquisitionTriggerModesStop;

		private bool _ignoreRtssSelectedIndexChanged;

		#region Constructor & Initialisation

		public AcquisitionManagerUI()
			: base()
		{
			InitializeComponent();

			_formSize = this.Size;
			_panelSize = pnlPrepareAcqui.Size;
			_preparePanelLocation = pnlPrepareAcqui.Location;

			tcStartAcqui.DrawItem += tabControl_DrawItem;
			tcStopAcqui.DrawItem += tabControl_DrawItem;

			SubscribeToUIEvents();

			ConvertStartStopTabControlToPanelsSwitcher();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			this.RegisterObserver(
				AgentBroker.Instance.ObserveOne<ProviderState>(this.ParentAgent.Id, "ProviderStateDataSource", ignoreAgentState: true)
					.ObserveOn(WindowsFormsSynchronizationContext.Current)
					.Subscribe(state => Task.WhenAll(HandleParentAgentProviderStateChanged(state))));

			DoAndIgnoreRtssIndexChanged(() => BindDDLs());

			SetControlsToInitialState();
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			base.OnFormClosing(e);

			if (e.CloseReason == CloseReason.UserClosing)
			{
				if (this.ParentAgent.ProviderState > ProviderState.Started)
				{
					e.Cancel = true;
					MessageBox.Show("Please complete the current acquisition before closing the Acquisition Manager window.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
		}

		private void SetControlsToInitialState()
		{
			ddlDrivers.DropDownListElement.AutoCompleteAppend.LimitToList = true;
			ddlOperators.DropDownListElement.AutoCompleteAppend.LimitToList = true;
			ddlVehicles.DropDownListElement.AutoCompleteAppend.LimitToList = true;
			ddlSequenceTypes.DropDownListElement.AutoCompleteAppend.LimitToList = true;

			ddlRouteStart.DropDownListElement.AutoCompleteAppend.LimitToList = true;
			ddlTronconStart.DropDownListElement.AutoCompleteAppend.LimitToList = true;
			ddlSectionStart.DropDownListElement.AutoCompleteAppend.LimitToList = true;
			ddlSousRouteStart.DropDownListElement.AutoCompleteAppend.LimitToList = true;

			ddlRouteStop.DropDownListElement.AutoCompleteAppend.LimitToList = true;
			ddlTronconStop.DropDownListElement.AutoCompleteAppend.LimitToList = true;
			ddlSectionStop.DropDownListElement.AutoCompleteAppend.LimitToList = true;
			ddlSousRouteStop.DropDownListElement.AutoCompleteAppend.LimitToList = true;

			imgPrepareAcquisition.BackgroundImage = ImageResources.PrepareButton;
			imgAddDriver.BackgroundImage = ImageResources.Add;
			imgAddOperator.BackgroundImage = ImageResources.Add;
			imgDeleteDriver.BackgroundImage = ImageResources.Delete;
			imgDeleteOperator.BackgroundImage = ImageResources.Delete;
			imgStartAcquisition.BackgroundImage = ImageResources.DisabledButton;
			imgStopAcquisition.BackgroundImage = ImageResources.DisabledButton;
			imgSaveValidation.BackgroundImage = ImageResources.DisabledSaveButton;

			pnlPrepareAcqui.Enabled = true;
			pnlStartAcqui.Enabled = false;
			pnlStopAcqui.Enabled = false;

			HideAllSpecificControls(true);
			HideAllSpecificControls(false);

			pnlValidation.Enabled = false;
			rdValide.Checked = false;
			rdInvalide.Checked = false;
		}
		private async Task HandleParentAgentProviderStateChanged(ProviderState state)
		{
			switch (state)
			{
				case ProviderState.Started:
				case ProviderState.InitializedRecord:
				case ProviderState.StartedRecord:
				case ProviderState.Failed:
					this.UseWaitCursor = false;
					Cursor.Current = Cursors.Default;
					break;
				case ProviderState.UninitializingRecord:
				case ProviderState.InitializingRecord:
					this.UseWaitCursor = true;
					Cursor.Current = Cursors.WaitCursor;
					break;
			}

			switch (state)
			{
				case ProviderState.Started:
					#region Prepare
					pnlPrepareAcqui.Enabled = true;

					ddlDrivers.Enabled = true;
					imgAddDriver.Enabled = true;
					imgAddDriver.BackgroundImage = ImageResources.Add;
					imgDeleteDriver.Enabled = true;
					imgDeleteDriver.BackgroundImage = ImageResources.Delete;

					ddlOperators.Enabled = true;
					imgAddOperator.Enabled = true;
					imgAddOperator.BackgroundImage = ImageResources.Add;
					imgDeleteOperator.Enabled = true;
					imgDeleteOperator.BackgroundImage = ImageResources.Delete;

					ddlSequenceTypes.Enabled = true;

					imgPrepareAcquisition.Enabled = true;
					imgPrepareAcquisition.BackgroundImage = ImageResources.PrepareButton;
					staticToolTips.SetToolTip(imgPrepareAcquisition, "Prepare acquisition");
					#endregion

					#region Start

					if (_bgrSubscription != null)
					{
						_bgrSubscription.Dispose();
						_bgrSubscription = null;
					}

					pnlStartAcqui.Enabled = false;
					imgStartAcquisition.BackgroundImage = ImageResources.DisabledButton;
					HideAllSpecificControls(true);
					#endregion

					#region Stop
					pnlStopAcqui.Enabled = false;
					imgStopAcquisition.BackgroundImage = ImageResources.DisabledButton;
					staticToolTips.SetToolTip(imgStopAcquisition, "Acquisition stopped");
					HideAllSpecificControls(false);
					#endregion

					#region Validate
					pnlValidation.Enabled = false;
					imgSaveValidation.BackgroundImage = ImageResources.DisabledSaveButton;
					imgSaveValidation.Enabled = false;
					txtCommentaires.Enabled = false;
					txtCommentaires.Text = "";
					rdValide.Checked = false;
					rdInvalide.Checked = false;
					#endregion
					break;
				case ProviderState.UninitializingRecord:
					#region Validate
					pnlValidation.Enabled = false;
					imgSaveValidation.BackgroundImage = ImageResources.DisabledSaveButton;
					imgSaveValidation.Enabled = false;
					txtCommentaires.Enabled = false;
					txtCommentaires.Text = "";
					rdValide.Checked = false;
					rdInvalide.Checked = false;
					#endregion
					break;
				case ProviderState.InitializingRecord:
					#region Prepare
					ddlDrivers.Enabled = false;
					imgAddDriver.Enabled = false;
					imgAddDriver.BackgroundImage = ImageResources.DisabledAdd;
					imgDeleteDriver.Enabled = false;
					imgDeleteDriver.BackgroundImage = ImageResources.DisabledDelete;

					ddlOperators.Enabled = false;
					imgAddOperator.Enabled = false;
					imgAddOperator.BackgroundImage = ImageResources.DisabledAdd;
					imgDeleteOperator.Enabled = false;
					imgDeleteOperator.BackgroundImage = ImageResources.DisabledDelete;

					ddlSequenceTypes.Enabled = false;

					imgPrepareAcquisition.BackgroundImage = ImageResources.CancelButton;
					staticToolTips.SetToolTip(imgPrepareAcquisition, "Cancel acquisition");
					#endregion
					break;
				case ProviderState.InitializedRecord:
					//HACK: pour détecter le cas où on est à l'étape de validation vs préparation
					if (pnlValidation.Enabled)
					{
						#region Stop
						pnlStopAcqui.Enabled = false;
						imgStopAcquisition.BackgroundImage = ImageResources.SuccessButton;
						#endregion

						#region Validate
						imgSaveValidation.Enabled = true;
						imgSaveValidation.BackgroundImage = ImageResources.SaveButton;
						#endregion
					}
					else
					{
						#region Start
						pnlStartAcqui.Enabled = true;

						imgStartAcquisition.Enabled = true;
						imgStartAcquisition.BackgroundImage = ImageResources.StartButton;
						staticToolTips.SetToolTip(imgStartAcquisition, "Engage start trigger");
						EnableStartStopAcquisitionControls(true, true);

						await DoAndIgnoreRtssIndexChanged(async () =>
						{
							await ShowCurrentSpecificControls(true);
							await TryShowEndSectionRtssc(true);
						});
						#endregion

						#region Validate
						txtCommentaires.Enabled = true;
						#endregion
					}
					break;
				case ProviderState.StartingRecord:
					#region Start
					imgStartAcquisition.BackgroundImage = ImageResources.BlueButton;
					staticToolTips.SetToolTip(imgStartAcquisition, "Cancel start trigger");

					pnlStartDirection.Enabled = false;
					ddlRouteStart.Enabled = false;
					ddlTronconStart.Enabled = false;
					ddlSectionStart.Enabled = false;
					ddlSousRouteStart.Enabled = false;
					mtxtChainageSelectionStart.Enabled = false;
					ddlProximityStart.Enabled = false;
					imgGetCurrentRTSSStart.Enabled = false;
					imgGetCurrentRTSSStart.BackgroundImage = ImageResources.DisabledPinpoint;
					#endregion
					break;
				case ProviderState.StartedRecord:
					#region Prepare
					pnlPrepareAcqui.Enabled = false;
					imgPrepareAcquisition.BackgroundImage = ImageResources.DisabledButton;
					#endregion

					#region Start
					pnlStartAcqui.Enabled = false;
					imgStartAcquisition.BackgroundImage = ImageResources.SuccessButton;
					staticToolTips.SetToolTip(imgStartAcquisition, "Acquisition started");
					#endregion

					#region Stop
					pnlStopAcqui.Enabled = true;

					imgStopAcquisition.Enabled = true;
					imgStopAcquisition.BackgroundImage = ImageResources.StopButton;
					staticToolTips.SetToolTip(imgStopAcquisition, "Engage stop trigger");
					EnableStartStopAcquisitionControls(false, true);

					await DoAndIgnoreRtssIndexChanged(async () =>
					{
						await ShowCurrentSpecificControls(false);
						await TryShowEndSectionRtssc(false);
					});
					#endregion
					break;
				case ProviderState.StoppingRecord:
					#region Stop
					imgStopAcquisition.BackgroundImage = ImageResources.BlueButton;
					staticToolTips.SetToolTip(imgStopAcquisition, "Cancel stop trigger");

					pnlStopDirection.Enabled = false;
					ddlRouteStop.Enabled = false;
					ddlTronconStop.Enabled = false;
					ddlSectionStop.Enabled = false;
					ddlSousRouteStop.Enabled = false;
					mtxtChainageSelectionStop.Enabled = false;
					ddlProximityStop.Enabled = false;
					imgGetCurrentRTSSStop.Enabled = false;
					imgGetCurrentRTSSStop.BackgroundImage = ImageResources.DisabledPinpoint;
					#endregion

					#region Validate
					pnlValidation.Enabled = true;
					imgSaveValidation.Enabled = false;
					#endregion
					break;
			}
		}


		private void BindDDLs()
		{
			_drivers = new BindingList<string>(this.ParentAgent.Drivers);
			ddlDrivers.DataSource = _drivers;
			ddlDrivers.SelectedValue = this.ParentAgent.SelectedDriver;

			_operators = new BindingList<string>(this.ParentAgent.Operators);
			ddlOperators.DataSource = _operators;
			ddlOperators.SelectedValue = this.ParentAgent.SelectedOperator;

			_vehicles = new BindingList<Tuple<string, string>>(this.ParentAgent.Vehicles);
			ddlVehicles.DataSource = _vehicles;
			ddlVehicles.ValueMember = "Item1";
			ddlVehicles.DisplayMember = "Item2";
			ddlVehicles.SelectedValue = this.ParentAgent.SelectedVehicle;

			_sequenceTypes = new BindingList<string>(this.ParentAgent.SequenceTypes);
			ddlSequenceTypes.DataSource = _sequenceTypes;
			ddlSequenceTypes.SelectedValue = this.ParentAgent.SelectedSequenceType;

			// the trigger mode list will refresh itself according to agents operational status

			var dt = new DataTable();
			dt.Columns.Add("Key", typeof(AcquisitionTriggerMode));
			dt.Columns.Add("Value", typeof(string));
			dt.Columns.Add("Type", typeof(string));
			dt.Columns.Add("Visible", typeof(bool));

			foreach (var triggerMode in this.ParentAgent.GetAcquisitionTriggerModes(true))
				dt.Rows.Add(triggerMode.Key, triggerMode.Value.Item2, triggerMode.Value.Item1, triggerMode.Value.Item1 == null || this.ParentAgent.IsDependencyOperational(triggerMode.Value.Item1));

			_dvAcquisitionTriggerModesStart = new DataView(dt);
			_dvAcquisitionTriggerModesStart.RowFilter = "Visible = true";

			ddlStartMode.ValueMember = "Key";
			ddlStartMode.DisplayMember = "Value";
			ddlStartMode.DataSource = _dvAcquisitionTriggerModesStart;
			ddlStartMode.SelectedValue = this.ParentAgent.LastSelectedStartTriggerMode;

			dt = dt.Clone();
			dt.Clear();

			foreach (var triggerMode in this.ParentAgent.GetAcquisitionTriggerModes(false))
				dt.Rows.Add(triggerMode.Key, triggerMode.Value.Item2, triggerMode.Value.Item1, triggerMode.Value.Item1 == null || this.ParentAgent.IsDependencyOperational(triggerMode.Value.Item1));

			_dvAcquisitionTriggerModesStop = new DataView(dt);
			_dvAcquisitionTriggerModesStop.RowFilter = "Visible = true";

			ddlStopMode.ValueMember = "Key";
			ddlStopMode.DisplayMember = "Value";
			ddlStopMode.DataSource = _dvAcquisitionTriggerModesStop;
			ddlStopMode.SelectedValue = this.ParentAgent.LastSelectedStopTriggerMode;

			this.RegisterObserver(
				AgentBroker.Instance.ObserveOne<Tuple<string, string, OperationalAgentStates>>(this.ParentAgent.Id, "DependenciesOperationalStateDataSource")
					.ObserveOn(WindowsFormsSynchronizationContext.Current)
					.Subscribe(
						data =>
						{
							// update sections related to start/stop mode selection
							foreach (var section in new[] { new { IsStartMode = true, View = _dvAcquisitionTriggerModesStart }, new { IsStartMode = false, View = _dvAcquisitionTriggerModesStop } })
							{
								var currentTriggerMode = GetSelectedTriggerMode(section.IsStartMode);
								bool needRefresh = false;
								foreach (DataRow row in section.View.Table.Rows)
								{
									if (!row.IsNull("Type") && row["Type"].ToString() == data.Item1)
									{
										row["Visible"] = (data.Item3 == OperationalAgentStates.None);
										if (data.Item3 != OperationalAgentStates.None && (AcquisitionTriggerMode) row["Key"] == currentTriggerMode)
											needRefresh = true;
									}
								}
								if (needRefresh)
									ShowCurrentSpecificControls(section.IsStartMode).Wait();
							}
						}));

			var sortedProximityRanges = this.ParentAgent.ProximityRanges.OrderBy(kv => kv.Key);

			ddlProximityStart.ValueMember = "Key";
			ddlProximityStart.DisplayMember = "Value";
			ddlProximityStart.DataSource = sortedProximityRanges.ToArray();

			ddlProximityStop.ValueMember = "Key";
			ddlProximityStop.DisplayMember = "Value";
			ddlProximityStop.DataSource = sortedProximityRanges.ToArray();
		}

		#endregion

		#region Drawing stuff

		/// <summary>
		/// Hide tabControl border (required because of the black window background).
		/// </summary>
		private void tabControl_DrawItem(object sender, DrawItemEventArgs e)
		{
			e.Graphics.FillRectangle(Brushes.Black, ((Control) sender).ClientRectangle);
		}

		private void ConvertStartStopTabControlToPanelsSwitcher()
		{
			tcStartAcqui.ItemSize = new Size(1, 1);
			tcStopAcqui.ItemSize = new Size(1, 1);

			// Hide tabControl glitch

			Panel pnlHideGlitchStartAcqui = new Panel();
			pnlHideGlitchStartAcqui.Location = tcStartAcqui.Location;
			pnlHideGlitchStartAcqui.Name = "pnlHideGlitchStartAcqui";
			pnlHideGlitchStartAcqui.Size = new Size(20, 5);
			pnlHideGlitchStartAcqui.BackColor = Color.Black;
			pnlStartAcqui.Controls.Add(pnlHideGlitchStartAcqui);
			pnlStartAcqui.Controls.SetChildIndex(pnlHideGlitchStartAcqui, 0);

			Panel pnlHideGlitchStopAcqui = new Panel();
			pnlHideGlitchStopAcqui.Location = tcStopAcqui.Location;
			pnlHideGlitchStopAcqui.Name = "pnlHideGlitchStopAcqui";
			pnlHideGlitchStopAcqui.Size = new Size(20, 5);
			pnlHideGlitchStopAcqui.BackColor = Color.Black;
			pnlStopAcqui.Controls.Add(pnlHideGlitchStopAcqui);
			pnlStopAcqui.Controls.SetChildIndex(pnlHideGlitchStopAcqui, 0);
		}

		#endregion

		#region Click Handlers

		private void SubscribeToUIEvents()
		{
			imgPrepareAcquisition.Click += (sender, e) => HandlePrepareAcquisition();
			imgStartAcquisition.Click += async (sender, e) => await HandleStartStopAcquisition(true);
			imgStopAcquisition.Click += async (sender, e) => await HandleStartStopAcquisition(false);
			imgSaveValidation.Click += (sender, e) => HandleSaveValidation();

			ddlStartMode.SelectedIndexChanged += async (sender, e) => { _ignoreRtssSelectedIndexChanged = true; HandleDisengage(true); await ShowCurrentSpecificControls(true); await TryShowEndSectionRtssc(true); await ShowEndSectionChainage(true, GetSelectedDirection(true)); _ignoreRtssSelectedIndexChanged = false; };
			ddlRouteStart.SelectedIndexChanged += async (sender, e) => { if (!_ignoreRtssSelectedIndexChanged) { _ignoreRtssSelectedIndexChanged = true; HandleDisengage(true); await HandleRouteSelectionChanged(true, (string) ddlRouteStart.SelectedValue); await ShowBeginSectionChainage(true, GetSelectedDirection(true)); _ignoreRtssSelectedIndexChanged = false; } };
			ddlTronconStart.SelectedIndexChanged += async (sender, e) => { if (!_ignoreRtssSelectedIndexChanged) { _ignoreRtssSelectedIndexChanged = true; HandleDisengage(true); await HandleTronconSelectionChanged(true); await ShowBeginSectionChainage(true, GetSelectedDirection(true)); _ignoreRtssSelectedIndexChanged = false; } };
			ddlSectionStart.SelectedIndexChanged += async (sender, e) => { if (!_ignoreRtssSelectedIndexChanged) { _ignoreRtssSelectedIndexChanged = true; HandleDisengage(true); await HandleSectionSelectionChanged(true); await ShowBeginSectionChainage(true, GetSelectedDirection(true)); _ignoreRtssSelectedIndexChanged = false; } };
			ddlSousRouteStart.SelectedIndexChanged += async (sender, e) => { if (!_ignoreRtssSelectedIndexChanged) { _ignoreRtssSelectedIndexChanged = true; HandleDisengage(true); await ShowBeginSectionChainage(true, GetSelectedDirection(true)); _ignoreRtssSelectedIndexChanged = false; } };
			rdStartDirection1.CheckedChanged += async (sender, e) => { if (!_ignoreRtssSelectedIndexChanged) { _ignoreRtssSelectedIndexChanged = true; HandleDisengage(true); await HandleDirectionCheckedChanged(true); _ignoreRtssSelectedIndexChanged = false; } };
			rdStartDirection2.CheckedChanged += async (sender, e) => { if (!_ignoreRtssSelectedIndexChanged) { _ignoreRtssSelectedIndexChanged = true; HandleDisengage(true); await HandleDirectionCheckedChanged(true); _ignoreRtssSelectedIndexChanged = false; } };
			imgGetCurrentRTSSStart.Click += async (sender, e) => { if (!_ignoreRtssSelectedIndexChanged) { _ignoreRtssSelectedIndexChanged = true; HandleDisengage(true); await HandleGetCurrentRtsscCommand(true); _ignoreRtssSelectedIndexChanged = false; } };

			ddlStopMode.SelectedIndexChanged += async (sender, e) => { _ignoreRtssSelectedIndexChanged = true; HandleDisengage(false); await ShowCurrentSpecificControls(false); await TryShowEndSectionRtssc(false); await ShowEndSectionChainage(false, GetSelectedDirection(false)); _ignoreRtssSelectedIndexChanged = false; };
			ddlRouteStop.SelectedIndexChanged += async (sender, e) => { if (!_ignoreRtssSelectedIndexChanged) { _ignoreRtssSelectedIndexChanged = true; HandleDisengage(false); await HandleRouteSelectionChanged(false, (string) ddlRouteStart.SelectedValue); await ShowEndSectionChainage(false, GetSelectedDirection(false)); _ignoreRtssSelectedIndexChanged = false; } };
			ddlTronconStop.SelectedIndexChanged += async (sender, e) => { if (!_ignoreRtssSelectedIndexChanged) { _ignoreRtssSelectedIndexChanged = true; HandleDisengage(false); await HandleTronconSelectionChanged(false); await ShowEndSectionChainage(false, GetSelectedDirection(false)); _ignoreRtssSelectedIndexChanged = false; } };
			ddlSectionStop.SelectedIndexChanged += async (sender, e) => { if (!_ignoreRtssSelectedIndexChanged) { _ignoreRtssSelectedIndexChanged = true; HandleDisengage(false); await HandleSectionSelectionChanged(false); await ShowEndSectionChainage(false, GetSelectedDirection(false)); _ignoreRtssSelectedIndexChanged = false; } };
			ddlSousRouteStop.SelectedIndexChanged += async (sender, e) => { if (!_ignoreRtssSelectedIndexChanged) { _ignoreRtssSelectedIndexChanged = true; HandleDisengage(false); await ShowEndSectionChainage(false, GetSelectedDirection(false)); _ignoreRtssSelectedIndexChanged = false; } };
			rdStopDirection1.CheckedChanged += async (sender, e) => { if (!_ignoreRtssSelectedIndexChanged) { _ignoreRtssSelectedIndexChanged = true; HandleDisengage(false); await HandleDirectionCheckedChanged(false); _ignoreRtssSelectedIndexChanged = false; _ignoreRtssSelectedIndexChanged = false; } };
			rdStopDirection2.CheckedChanged += async (sender, e) => { if (!_ignoreRtssSelectedIndexChanged) { _ignoreRtssSelectedIndexChanged = true; HandleDisengage(false); await HandleDirectionCheckedChanged(false); _ignoreRtssSelectedIndexChanged = false; _ignoreRtssSelectedIndexChanged = false; } };
			imgGetCurrentRTSSStop.Click += async (sender, e) => { if (!_ignoreRtssSelectedIndexChanged) { _ignoreRtssSelectedIndexChanged = true; HandleDisengage(false); await HandleGetCurrentRtsscCommand(false); _ignoreRtssSelectedIndexChanged = false; _ignoreRtssSelectedIndexChanged = false; } };
			txtDistanceStop.TextChanged += (sender, e) => HandleDisengage(false);

			AllMenuItem.Click += async (sender, e) => await CopyUIDataToClipboard(UIDataSections.All);
			StartInfoMenuItem.Click += async (sender, e) => await CopyUIDataToClipboard(UIDataSections.StartStop);
			CurrentRtssMenuItem.Click += async (sender, e) => await CopyUIDataToClipboard(UIDataSections.CurrentRtss);
			NextRtssMenuItem.Click += async (sender, e) => await CopyUIDataToClipboard(UIDataSections.NextRtss);

			rdValide.CheckedChanged += (sender, e) => txtCommentaires.Focus();
			rdInvalide.CheckedChanged += (sender, e) => txtCommentaires.Focus();
		}

		private void HandlePrepareAcquisition()
		{
			if (this.ParentAgent.ProviderState == ProviderState.InitializingRecord || this.ParentAgent.ProviderState == ProviderState.InitializedRecord || this.ParentAgent.ProviderState == ProviderState.StartingRecord)
			{
				// if registered to a starting mode
				if (this.ParentAgent.ProviderState == ProviderState.StartingRecord)
					HandleDisengage(true);

				this.ParentAgent.UninitializeRecord(new UninitializeRecordParameter { SequenceId = this.ParentAgent.SequenceId });
			}
			else
			{
				string driverFullName = (string) ddlDrivers.SelectedValue;
				string operatorFullName = (string) ddlOperators.SelectedValue;
				string vehicleFullName = (string) ddlVehicles.SelectedValue;
				string sequenceType = (string) ddlSequenceTypes.SelectedValue;

				var isValidSelection =
					ddlDrivers.SelectedIndex > -1 && ddlOperators.SelectedIndex > -1
					&& ddlVehicles.SelectedIndex > -1 && ddlSequenceTypes.SelectedIndex > -1;

				if (!isValidSelection || string.IsNullOrEmpty(driverFullName) || string.IsNullOrEmpty(operatorFullName) || string.IsNullOrEmpty(vehicleFullName) || string.IsNullOrEmpty(sequenceType))
					MessageBox.Show("Driver, operator, vehicle and acquisition type must not be empty.", "Invalid preparation data", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				else
				{
					this.ParentAgent.SaveDriversToConfig(_drivers, driverFullName);
					this.ParentAgent.SaveOperatorsToConfig(_operators, operatorFullName);
					this.ParentAgent.SaveSequenceTypeToConfig(sequenceType);

					this.ParentAgent.PrepareRecord(driverFullName, operatorFullName, vehicleFullName, sequenceType);
				}
			}
		}
		private async Task HandleStartStopAcquisition(bool isStartMode)
		{
			TextBoxBase txtDistance = txtDistanceStop;

			if (isStartMode && this.ParentAgent.ProviderState == ProviderState.StartingRecord)
			{
				await ShowCurrentSpecificControls(isStartMode);
				HandleDisengage(isStartMode);
			}
			else if (!isStartMode && this.ParentAgent.ProviderState == ProviderState.StoppingRecord)
			{
				await ShowCurrentSpecificControls(isStartMode);
				HandleDisengage(isStartMode);
			}
			else
			{
				AcquisitionTriggerMode triggerMode = GetSelectedTriggerMode(isStartMode);
				if (triggerMode == AcquisitionTriggerMode.Unknown)
				{
					if (isStartMode)
						MessageBox.Show("Please select a starting mode from the dropdown options.", "Invalid starting mode selection", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					else
						MessageBox.Show("Please select a stopping mode from the dropdown options.", "Invalid stopping mode selection", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

					return;
				}

				DirectionBgr? direction = null;
				IRtssc rtssc = null;
				int? proximityRange = null;
				double? distance = null;

				switch (triggerMode)
				{
					case AcquisitionTriggerMode.Rtssc:
					case AcquisitionTriggerMode.RtsscProximity:
					case AcquisitionTriggerMode.EndSection:

						if (triggerMode == AcquisitionTriggerMode.RtsscProximity)
							proximityRange = GetProximityRange(isStartMode);
						else if (triggerMode == AcquisitionTriggerMode.EndSection)
						{
							if (_bgrSubscription != null)
							{
								_bgrSubscription.Dispose();
								_bgrSubscription = null;
							}
						}

						rtssc = GetSelectedRtssc(isStartMode);
						if (rtssc == null)
						{
							MessageBox.Show(string.Format("Selected RTSSC must include the elements: route, tronçon, section and sous-route."), "Invalid RTSSC", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
							return;
						}
						direction = GetSelectedDirection(isStartMode);

						double rtssLength = await GetSectionLength(rtssc);
						if (rtssc.Chainage < 0 || rtssc.Chainage > rtssLength)
						{
							MessageBox.Show(string.Format("The RTSSC element 'chaînage' must be an integer between 0 and {0}.", rtssLength), "Invalid RTSSC", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
							return;
						}


						break;

					case AcquisitionTriggerMode.Distance:
						double tmpDistance;
						if (!double.TryParse(txtDistance.Text.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out tmpDistance) || tmpDistance < 0)
							MessageBox.Show(string.Format("Distance must be a decimal number greater than or equal to 0."), "Invalid distance", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						distance = tmpDistance;
						break;
				}

				if (isStartMode)
					await this.ParentAgent.EngageStartRecord(triggerMode, direction, rtssc, proximityRange);
				else
					await this.ParentAgent.EngageStopRecord(triggerMode, direction, rtssc, proximityRange, distance);

			}
		}
		private void HandleDisengage(bool isStartMode)
		{
			if ((isStartMode && this.ParentAgent.ProviderState < ProviderState.StartingRecord) || (!isStartMode && this.ParentAgent.ProviderState != ProviderState.StoppingRecord))
				return;

			if (this.ParentAgent.Disengage(isStartMode))
				ShowCanceledAcquisitionControls(isStartMode);
		}
		private void HandleSaveValidation()
		{
			if (!rdValide.Checked && !rdInvalide.Checked)
				MessageBox.Show("You must first indicate whether the acquisition was valid or not before saving.", "Missing validation check information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			else
			{
				this.ParentAgent.ValidateRecord(rdValide.Checked, txtCommentaires.Text)
					.ContinueWith(t => this.ParentAgent.UninitializeRecord(new UninitializeRecordParameter { SequenceId = this.ParentAgent.SequenceId }));
			}
		}

		private void imgAddDriver_Click(object sender, EventArgs e)
		{
			var dlg = new PersonNameInputBox("New driver", "Please enter driver's first and last name:");
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				var fullName = string.Format("{0}, {1}", dlg.LastName, dlg.FirstName);
				if (!_drivers.Contains(fullName))
				{
					_drivers.Add(fullName);
					this.ParentAgent.SaveDriversToConfig(_drivers, fullName);
					ddlDrivers.SelectedValue = fullName;
				}
			}
		}
		private void imgAddOperator_Click(object sender, EventArgs e)
		{
			var dlg = new PersonNameInputBox("New operator", "Please enter operator's first and last name:");
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				var fullName = string.Format("{0}, {1}", dlg.LastName, dlg.FirstName);
				if (!_operators.Contains(fullName))
				{
					_operators.Add(fullName);
					this.ParentAgent.SaveOperatorsToConfig(_operators, fullName);
					ddlOperators.SelectedValue = fullName;
				}
			}
		}
		private void imgDeleteDriver_Click(object sender, EventArgs e)
		{
			if (ddlDrivers.SelectedIndex < 0)
				return;

			if (DialogResult.Yes == MessageBox.Show(this, string.Format("Are you sure you want to delete the driver '{0}'?", ddlDrivers.SelectedText), "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
			{
				_drivers.Remove((string) ddlDrivers.SelectedValue);
				this.ParentAgent.SaveDriversToConfig(_drivers, (string) ddlDrivers.SelectedValue);
			}
		}
		private void imgDeleteOperator_Click(object sender, EventArgs e)
		{
			if (ddlOperators.SelectedIndex < 0)
				return;

			if (DialogResult.Yes == MessageBox.Show(this, string.Format("Are you sure you want to delete the operator '{0}'?", ddlOperators.SelectedText), "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
			{
				_operators.Remove((string) ddlOperators.SelectedValue);
				this.ParentAgent.SaveOperatorsToConfig(_operators, (string) ddlOperators.SelectedValue);
			}
		}

		#endregion

		private AcquisitionTriggerMode GetSelectedTriggerMode(bool isStartMode)
		{
			RadDropDownList ddlTriggerMode = isStartMode ? ddlStartMode : ddlStopMode;

			AcquisitionTriggerMode triggerMode;
			if (ddlTriggerMode.SelectedValue == null)
				triggerMode = AcquisitionTriggerMode.Unknown;
			else
				triggerMode = (AcquisitionTriggerMode) ddlTriggerMode.SelectedValue;

			return triggerMode;
		}
		private async Task ShowCurrentSpecificControls(bool isStartMode)
		{
			RadDropDownList ddlTriggerMode = isStartMode ? ddlStartMode : ddlStopMode;
			TabControl tcAcqui = isStartMode ? tcStartAcqui : tcStopAcqui;
			TabPage tpWaitingForBgr = isStartMode ? tpStartWaitingForBGR : tpStopWaitingForBGR;
			TabPage tpRtssDdls = isStartMode ? tpStartRtssDdls : tpStopRtssDdls;
			TabPage tpDistance = isStartMode ? null : tpStopDistance;
			Panel pnlDirection = isStartMode ? pnlStartDirection : pnlStopDirection;
			RadDropDownList ddlProximity = isStartMode ? ddlProximityStart : ddlProximityStop;
			PictureBox getCurrentRtss = isStartMode ? imgGetCurrentRTSSStart : imgGetCurrentRTSSStop;

			if (_bgrSubscription != null)
			{
				_bgrSubscription.Dispose();
				_bgrSubscription = null;
			}
			HideAllSpecificControls(isStartMode);

			AcquisitionTriggerMode triggerMode = GetSelectedTriggerMode(isStartMode);
			switch (triggerMode)
			{
				case AcquisitionTriggerMode.EndSection:
					{
						EnableRtsscControls(isStartMode, false);

						tcAcqui.SelectedTab = tpWaitingForBgr;
						tcAcqui.Refresh();
						var currentBgrInfo = await GetCurrentBgrData();
						ShowSpecificDirection(isStartMode, currentBgrInfo != null ? currentBgrInfo.Direction : DirectionBgr.Unknown);
						await TryUpdateRouteDdl(isStartMode);

						EnableStartStopAcquisitionControls(isStartMode, false);
						pnlDirection.Visible = true;
						getCurrentRtss.Visible = false;

						if (_bgrSubscription == null)
						{
							_bgrSubscription = AgentBroker.Instance.ObserveAny<IBgrDirectionalAgent, BgrData>("DataSource")
								.Where(data => data.Direction != DirectionBgr.Unknown)
								.Sample(TimeSpan.FromSeconds(3))
								.DistinctUntilChanged(bgrData => bgrData.Rtssc.Route + bgrData.Rtssc.Troncon + bgrData.Rtssc.Section + bgrData.Rtssc.SousRoute)
								.ObserveOn(WindowsFormsSynchronizationContext.Current)
								.Subscribe(async data => await ShowSpecificEndSectionRtssc(isStartMode, data));
						}

						tcAcqui.SelectedTab = tpRtssDdls;
						break;
					}
				case AcquisitionTriggerMode.Rtssc:
				case AcquisitionTriggerMode.RtsscProximity:
					{
						EnableRtsscControls(isStartMode, true);

						tcAcqui.SelectedTab = tpWaitingForBgr;
						tcAcqui.Refresh();
						var currentBgrInfo = await GetCurrentBgrData();
						ShowSpecificDirection(isStartMode, currentBgrInfo != null ? currentBgrInfo.Direction : DirectionBgr.Unknown);
						await TryUpdateRouteDdl(isStartMode);

						EnableStartStopAcquisitionControls(isStartMode, true);
						pnlDirection.Visible = true;
						getCurrentRtss.Visible = true;

						if (triggerMode == AcquisitionTriggerMode.RtsscProximity)
						{
							ddlProximity.Visible = true;
							ddlProximity.SelectedIndex = 0;
						}

						tcAcqui.SelectedTab = tpRtssDdls;

						break;
					}
				case AcquisitionTriggerMode.Distance:
					tcAcqui.SelectedTab = tpDistance;
					break;
			}
		}
		private void HideAllSpecificControls(bool isStartMode)
		{
			if (isStartMode)
			{
				tcStartAcqui.SelectedTab = tpStartInvisible;
				pnlStartDirection.Visible = false;
				ddlProximityStart.Visible = false;
			}
			else
			{
				tcStopAcqui.SelectedTab = tpStopInvisible;
				pnlStopDirection.Visible = false;
				ddlProximityStop.Visible = false;
			}
		}
		private void ShowCanceledAcquisitionControls(bool isStartMode)
		{
			PictureBox imgAcquisition = isStartMode ? imgStartAcquisition : imgStopAcquisition;
			Panel pnlDirection = isStartMode ? pnlStartDirection : pnlStopDirection;
			RadDropDownList ddlProximity = isStartMode ? ddlProximityStart : ddlProximityStop;
			TabControl tcAcqui = isStartMode ? tcStartAcqui : tcStopAcqui;
			TabPage tpRtssDdls = isStartMode ? tpStartRtssDdls : tpStopRtssDdls;

			imgAcquisition.BackgroundImage = ImageResources.StartButton;
			imgAcquisition.Enabled = true;

			AcquisitionTriggerMode triggerMode = GetSelectedTriggerMode(isStartMode);
			switch (triggerMode)
			{
				case AcquisitionTriggerMode.Rtssc:
				case AcquisitionTriggerMode.RtsscProximity:
					HideAllSpecificControls(isStartMode);
					tcAcqui.SelectedTab = tpRtssDdls;

					pnlDirection.Visible = true;
					pnlDirection.Enabled = true;
					ddlProximity.Visible = triggerMode == AcquisitionTriggerMode.RtsscProximity;
					ddlProximity.Enabled = triggerMode == AcquisitionTriggerMode.RtsscProximity;
					break;
				case AcquisitionTriggerMode.EndSection:
					HideAllSpecificControls(isStartMode);
					break;
			}
		}
		private void EnableStartStopAcquisitionControls(bool isStartMode, bool isEnable)
		{
			Panel direction = isStartMode ? pnlStartDirection : pnlStopDirection;
			RadDropDownList route = isStartMode ? ddlRouteStart : ddlRouteStop;
			RadDropDownList troncon = isStartMode ? ddlTronconStart : ddlTronconStop;
			RadDropDownList section = isStartMode ? ddlSectionStart : ddlSectionStop;
			RadDropDownList sousRoute = isStartMode ? ddlSousRouteStart : ddlSousRouteStop;
			MaskedTextBox chainage = isStartMode ? mtxtChainageSelectionStart : mtxtChainageSelectionStop;
			RadDropDownList proximity = isStartMode ? ddlProximityStart : ddlProximityStop;
			PictureBox getCurrentRtss = isStartMode ? imgGetCurrentRTSSStart : imgGetCurrentRTSSStop;

			direction.Enabled = isEnable;
			route.Enabled = isEnable;
			troncon.Enabled = isEnable;
			section.Enabled = isEnable;
			sousRoute.Enabled = isEnable;
			chainage.Enabled = isEnable;
			proximity.Enabled = isEnable;
			getCurrentRtss.Enabled = isEnable;
			getCurrentRtss.BackgroundImage = isEnable ? ImageResources.Pinpoint : ImageResources.DisabledPinpoint;
		}

		private async Task CopyUIDataToClipboard(UIDataSections section)
		{
			var sb = new StringBuilder();

			if (section.HasFlag(UIDataSections.All))
			{
				sb.AppendFormat("Driver            : {0}\r\n", ddlDrivers.SelectedValue);
				sb.AppendFormat("Operator          : {0}\r\n", ddlOperators.SelectedValue);
				sb.AppendFormat("Vehicle           : {0}\r\n", ddlVehicles.SelectedValue);
				sb.AppendFormat("Acquisition type  : {0}\r\n", ddlSequenceTypes.SelectedValue);
			}

			if (section.HasFlag(UIDataSections.StartStop))
			{
				sb.AppendFormat("Starting mode     : {0}\r\n", ddlStartMode.Text);

				if (ddlStartMode.SelectedValue != null)
				{
					var startMode = (AcquisitionTriggerMode) ddlStartMode.SelectedValue;
					switch (startMode)
					{
						case AcquisitionTriggerMode.Rtssc:
						case AcquisitionTriggerMode.RtsscProximity:
							sb.AppendFormat("Start RTSS        : {0}\r\n", GetSelectedRtssc(true));
							sb.AppendFormat("Direction         : {0}\r\n", GetSelectedDirection(true));
							if (startMode == AcquisitionTriggerMode.RtsscProximity)
								sb.AppendFormat("Proximity         : {0}\r\n", ddlProximityStart.SelectedValue);
							break;
					}
				}

				sb.AppendFormat("Stop mode         : {0}\r\n", ddlStopMode.Text);

				if (ddlStopMode.SelectedValue != null)
				{
					var stopMode = (AcquisitionTriggerMode) ddlStopMode.SelectedValue;
					switch (stopMode)
					{
						case AcquisitionTriggerMode.Distance:
							sb.AppendFormat("Stop distance     : {0}m\r\n", lblDistanceStop.Text);
							break;
						case AcquisitionTriggerMode.Rtssc:
						case AcquisitionTriggerMode.RtsscProximity:
							sb.AppendFormat("Stop RTSS         : {0}\r\n", GetSelectedRtssc(false));
							sb.AppendFormat("Direction         : {0}\r\n", GetSelectedDirection(false));
							if (stopMode == AcquisitionTriggerMode.RtsscProximity)
								sb.AppendFormat("Proximity         : {0}\r\n", ddlProximityStop.SelectedValue);
							break;
					}
				}
			}

			var bgrData = await GetCurrentBgrData() ?? new BgrData();

			if (section.HasFlag(UIDataSections.CurrentRtss))
				sb.AppendFormat("Current RTSSC      : {0}\r\n", bgrData.Rtssc ?? (object) "Unknown");

			if (section.HasFlag(UIDataSections.NextRtss))
				sb.AppendFormat("Next RTSSC         : {0}\r\n", bgrData.Rtssc == null ? (object) "Unknown" : GetNextRTSSCFromRTSS(bgrData.Rtssc));

			Clipboard.SetText(sb.ToString());
		}
	}
}