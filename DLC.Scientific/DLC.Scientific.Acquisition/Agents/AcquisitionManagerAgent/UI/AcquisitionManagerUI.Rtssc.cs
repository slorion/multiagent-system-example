using DLC.Multiagent;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using DLC.Scientific.Acquisition.Core.Agents;
using DLC.Scientific.Acquisition.Core.Agents.Model;
using DLC.Scientific.Core.Geocoding;
using DLC.Scientific.Core.Geocoding.Bgr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls.UI;

namespace DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI
{
	partial class AcquisitionManagerUI
	{
		private void EnableRtsscControls(bool isStartMode, bool enable)
		{
			RadDropDownList ddlRoute = isStartMode ? ddlRouteStart : ddlRouteStop;
			RadDropDownList ddlTroncon = isStartMode ? ddlTronconStart : ddlTronconStop;
			RadDropDownList ddlSection = isStartMode ? ddlSectionStart : ddlSectionStop;
			RadDropDownList ddlSousRoute = isStartMode ? ddlSousRouteStart : ddlSousRouteStop;
			MaskedTextBox mtxtChainage = isStartMode ? mtxtChainageSelectionStart : mtxtChainageSelectionStop;

			ddlRoute.Enabled = enable;
			ddlTroncon.Enabled = enable;
			ddlSection.Enabled = enable;
			ddlSousRoute.Enabled = enable;
			mtxtChainage.Enabled = enable;
		}

		private async Task HandleRouteSelectionChanged(bool isStartMode, string route)
		{
			RadDropDownList ddlRoute = isStartMode ? ddlRouteStart : ddlRouteStop;
			RadDropDownList ddlTroncon = isStartMode ? ddlTronconStart : ddlTronconStop;

			if (ddlRoute.SelectedItem != null)
			{
				IEnumerable<IRtssc> rtssFromRoute;
				if (isStartMode)
				{
					_rtssFromRouteStart = await GetRtssFromRoute(route);
					rtssFromRoute = _rtssFromRouteStart;
				}
				else
				{
					_rtssFromRouteStop = await GetRtssFromRoute(route);
					rtssFromRoute = _rtssFromRouteStop;
				}

				ddlTroncon.DataSource = (
					from rtss in rtssFromRoute
					//where rtss.Route == (string) ddlRoute.SelectedItem
					orderby rtss.Troncon ascending
					select rtss.Troncon)
					.Distinct().ToList();
			}
			else
			{
				ddlTroncon.DataSource = null;
				//ddlRoute.Enabled = false;
			}

			await HandleTronconSelectionChanged(isStartMode);
		}
		private async Task HandleTronconSelectionChanged(bool isStartMode)
		{
			RadDropDownList ddlTroncon = isStartMode ? ddlTronconStart : ddlTronconStop;
			RadDropDownList ddlSection = isStartMode ? ddlSectionStart : ddlSectionStop;

			if (ddlTroncon.SelectedItem != null)
			{
				var rtssFromRoute = isStartMode ? _rtssFromRouteStart : _rtssFromRouteStop;

				ddlSection.DataSource = (
					from rtss in rtssFromRoute
					where rtss.Troncon == (string) ddlTroncon.SelectedValue
					orderby rtss.Section ascending
					select rtss.Section)
					.Distinct().ToList();
			}
			else
			{
				ddlSection.DataSource = null;
				//ddlTroncon.Enabled = false;
			}

			await HandleSectionSelectionChanged(isStartMode);
		}
		private async Task HandleSectionSelectionChanged(bool isStartMode)
		{
			DirectionBgr direction = GetSelectedDirection(isStartMode);

			BindStartStopSousRouteDDLs(isStartMode, direction);
			await ShowEndSectionChainage(isStartMode, direction);
		}
		private void BindStartStopSousRouteDDLs(bool isStartMode, DirectionBgr direction)
		{
			RadDropDownList ddlTroncon = isStartMode ? ddlTronconStart : ddlTronconStop;
			RadDropDownList ddlSection = isStartMode ? ddlSectionStart : ddlSectionStop;
			RadDropDownList ddlSousRoute = isStartMode ? ddlSousRouteStart : ddlSousRouteStop;

			if (ddlSection.SelectedItem != null)
			{
				if (direction != DirectionBgr.Unknown)
				{
					var rtssFromRoute = isStartMode ? _rtssFromRouteStart : _rtssFromRouteStop;

					if (direction == DirectionBgr.ForwardChaining)
					{
						ddlSousRoute.DataSource = (
							from currentRTSS in rtssFromRoute
							where currentRTSS.Section == (string) ddlSection.SelectedValue
								&& currentRTSS.Troncon == (string) ddlTroncon.SelectedValue
								&& (
									currentRTSS.CodeSousRoute == "3"
									|| currentRTSS.CodeCoteChaussee == "D"
									|| currentRTSS.CodeCoteChaussee == "C")
							orderby currentRTSS.SousRoute ascending
							select currentRTSS.SousRoute)
							.Distinct().ToList();
					}
					else if (direction == DirectionBgr.BackwardChaining)
					{
						ddlSousRoute.DataSource = (
							from currentRTSS in rtssFromRoute
							where currentRTSS.Section == (string) ddlSection.SelectedValue
								&& currentRTSS.Troncon == (string) ddlTroncon.SelectedValue
								&& (
									currentRTSS.CodeCoteChaussee == "G"
									|| currentRTSS.CodeCoteChaussee == "C")
							orderby currentRTSS.SousRoute ascending
							select currentRTSS.SousRoute)
							.Distinct().ToList();
					}
				}
				else
					ddlSousRoute.DataSource = null;
			}
			else
			{
				ddlSousRoute.DataSource = null;
				//ddlSection.Enabled = false;
			}
		}
		private async Task HandleGetCurrentRtsscCommand(bool isStartMode)
		{
			RadDropDownList ddlRoute = isStartMode ? ddlRouteStart : ddlRouteStop;
			RadDropDownList ddlTroncon = isStartMode ? ddlTronconStart : ddlTronconStop;
			RadDropDownList ddlSection = isStartMode ? ddlSectionStart : ddlSectionStop;
			RadDropDownList ddlSousRoute = isStartMode ? ddlSousRouteStart : ddlSousRouteStop;
			TextBoxBase mtxtChainageSelection = isStartMode ? mtxtChainageSelectionStart : mtxtChainageSelectionStop;

			HandleDisengage(isStartMode);

			var bgrData = await GetCurrentBgrData();
			if (bgrData == null || bgrData.Rtssc == null)
			{
				bgrData = new BgrData { Direction = DirectionBgr.Unknown, Rtssc = null };
				MessageBox.Show("Cannot get current RTSSC. Your current location may not be a registered road or GPS is currently not working.");
			}
			ShowSpecificDirection(isStartMode, bgrData.Direction);
			await ShowSpecificRtssc(isStartMode, bgrData);
			mtxtChainageSelection.Text = bgrData.Rtssc.Chainage.GetValueOrDefault().ToString("F0");
		}
		private async Task HandleDirectionCheckedChanged(bool isStartMode)
		{
			await DoAndIgnoreRtssIndexChanged(async () =>
			{
				AcquisitionTriggerMode triggerMode = GetSelectedTriggerMode(isStartMode);
				switch (triggerMode)
				{
					case AcquisitionTriggerMode.Rtssc:
					case AcquisitionTriggerMode.RtsscProximity:
						DirectionBgr direction = GetSelectedDirection(isStartMode);
						BindStartStopSousRouteDDLs(isStartMode, direction);
						await ShowEndSectionChainage(isStartMode, direction);
						break;
				}
			});
		}

		private IRtssc GetSelectedRtssc(bool isStartMode)
		{
			RadDropDownList ddlRoute = isStartMode ? ddlRouteStart : ddlRouteStop;
			RadDropDownList ddlTroncon = isStartMode ? ddlTronconStart : ddlTronconStop;
			RadDropDownList ddlSection = isStartMode ? ddlSectionStart : ddlSectionStop;
			RadDropDownList ddlSousRoute = isStartMode ? ddlSousRouteStart : ddlSousRouteStop;
			TextBoxBase mtxtChainageSelection = isStartMode ? mtxtChainageSelectionStart : mtxtChainageSelectionStop;

			IRtssc rtssc = null;
			AcquisitionTriggerMode triggerMode = GetSelectedTriggerMode(isStartMode);

			if (triggerMode == AcquisitionTriggerMode.Rtssc || triggerMode == AcquisitionTriggerMode.RtsscProximity || triggerMode == AcquisitionTriggerMode.EndSection)
			{
				var ValidateControl = new Func<RadDropDownList, bool>(ddlControl => ddlControl.SelectedIndex != -1 && ddlControl.SelectedValue.ToString() == ddlControl.Text);

				if (ValidateControl(ddlRoute) && ValidateControl(ddlTroncon) && ValidateControl(ddlSection) && ValidateControl(ddlSousRoute))
				{
					double chainage;
					if (!double.TryParse(mtxtChainageSelection.Text, out chainage))
						chainage = 0;

					rtssc = new Rtssc(
						(string) ddlRoute.SelectedValue,
						(string) ddlTroncon.SelectedValue,
						(string) ddlSection.SelectedValue,
						(string) ddlSousRoute.SelectedValue,
						chainage: chainage);
				}
			}

			return rtssc;
		}
		private DirectionBgr GetSelectedDirection(bool isStartMode)
		{
			RadioButton rdForwardDirection = isStartMode ? rdStartDirection1 : rdStopDirection1;
			RadioButton rdBackwardDirection = isStartMode ? rdStartDirection2 : rdStopDirection2;

			if (rdForwardDirection.Checked)
				return DirectionBgr.ForwardChaining;
			else if (rdBackwardDirection.Checked)
				return DirectionBgr.BackwardChaining;
			else
				return DirectionBgr.Unknown;
		}
		private int? GetProximityRange(bool isStartMode)
		{
			RadDropDownList ddlProximity = isStartMode ? ddlProximityStart : ddlProximityStop;
			return (int?) ddlProximity.SelectedValue;
		}

		private async Task TryUpdateRouteDdl(bool isStartMode)
		{
			RadDropDownList ddlRoute = isStartMode ? ddlRouteStart : ddlRouteStop;

			if (ddlRoute.Tag == null || DateTimeOffset.Now - (DateTimeOffset) ddlRoute.Tag > TimeSpan.FromMinutes(2))
			{
				var loc = await AgentBroker.Instance.TryExecuteOnFirst<ILocalisationAgent, LocalisationData>(agent => agent.CurrentData).GetValueOrDefault(null);
				if (loc == null)
					MessageBox.Show("Aucune coordonnée GPS disponible.");
				else
				{
					ddlRoute.DataSource = (await SelectRoutes(loc.CorrectedData.PositionData, isStartMode ? this.ParentAgent.StartRtsscSearchRadiusInMeters : this.ParentAgent.StopRtsscSearchRadiusInMeters, null)).ToList();
					ddlRoute.Tag = DateTimeOffset.Now;
					await HandleRouteSelectionChanged(isStartMode, (string) ddlRoute.SelectedValue);
				}
			}
		}
		private async Task ShowSpecificRtssc(bool isStartMode, BgrData bgrData)
		{
			if (bgrData == null) throw new ArgumentNullException("bgrData");

			TabControl tcAcqui = isStartMode ? tcStartAcqui : tcStopAcqui;
			TabPage tpRtssDdls = isStartMode ? tpStartRtssDdls : tpStopRtssDdls;

			RadDropDownList ddlRoute = isStartMode ? ddlRouteStart : ddlRouteStop;
			RadDropDownList ddlTroncon = isStartMode ? ddlTronconStart : ddlTronconStop;
			RadDropDownList ddlSection = isStartMode ? ddlSectionStart : ddlSectionStop;
			RadDropDownList ddlSousRoute = isStartMode ? ddlSousRouteStart : ddlSousRouteStop;

			if (bgrData.Rtssc != null)
			{
				tcAcqui.SelectedTab = tpRtssDdls;

				if (GetSelectedDirection(isStartMode) != bgrData.Direction)
				{
					ShowSpecificDirection(isStartMode, bgrData.Direction);
					await HandleRouteSelectionChanged(isStartMode, (string) ddlRoute.SelectedValue);
				}

				if ((string) ddlRoute.SelectedValue != bgrData.Rtssc.Route)
				{
					ddlRoute.SelectedValue = bgrData.Rtssc.Route;
					await HandleRouteSelectionChanged(isStartMode, bgrData.Rtssc.Route);
				}
				if ((string) ddlTroncon.SelectedValue != bgrData.Rtssc.Troncon)
				{
					ddlTroncon.SelectedValue = bgrData.Rtssc.Troncon;
					await HandleTronconSelectionChanged(isStartMode);
				}
				if ((string) ddlSection.SelectedValue != bgrData.Rtssc.Section)
				{
					ddlSection.SelectedValue = bgrData.Rtssc.Section;
					await HandleSectionSelectionChanged(isStartMode);
				}
				if ((string) ddlSousRoute.SelectedValue != bgrData.Rtssc.SousRoute)
				{
					ddlSousRoute.SelectedValue = bgrData.Rtssc.SousRoute;
				}
			}
			else
			{
				HideAllSpecificControls(isStartMode);
				MessageBox.Show("No RTSS to show", "Cannot show RTSS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}
		private async Task ShowSpecificEndSectionRtssc(bool isStartMode, BgrData bgrData)
		{
			if (bgrData == null || bgrData.Rtssc == null)
			{
				//Stop bgrCallback
				MessageBox.Show("Cannot get RTSSC for the next section start.");
				return;
			}

			if (bgrData.Direction == DirectionBgr.Unknown)
			{
				//Stop bgrCallback
				MessageBox.Show("Cannot get the current direction which is required to get the RTSSC of the next section start. Possible solution: Vehicle must be moving to determine the current direction when the current 'sous-route' type is [000C].");
			}

			_ignoreRtssSelectedIndexChanged = true;
			try
			{
				await ShowSpecificRtssc(isStartMode, bgrData);
				await ShowEndSectionChainage(isStartMode, bgrData.Direction);
			}
			finally { _ignoreRtssSelectedIndexChanged = false; }
		}
		private async Task TryShowEndSectionRtssc(bool isStartMode)
		{
			AcquisitionTriggerMode stopTriggerMode = GetSelectedTriggerMode(isStartMode);
			if (stopTriggerMode == AcquisitionTriggerMode.Rtssc || stopTriggerMode == AcquisitionTriggerMode.RtsscProximity)
			{
				var bgrData = await AgentBroker.Instance.TryExecuteOnFirst<IBgrDirectionalAgent, BgrData>(agent => agent.CurrentData).GetValueOrDefault(null); ;
				await ShowSpecificEndSectionRtssc(isStartMode, bgrData);
				await ShowEndSectionChainage(isStartMode, GetSelectedDirection(isStartMode));
			}
		}
		private Task ShowBeginSectionChainage(bool isStartMode, DirectionBgr direction)
		{
			return ShowEndSectionChainage(isStartMode, direction == DirectionBgr.BackwardChaining ? DirectionBgr.ForwardChaining : direction == DirectionBgr.ForwardChaining ? DirectionBgr.BackwardChaining : DirectionBgr.Unknown);
		}
		private async Task ShowEndSectionChainage(bool isStartMode, DirectionBgr direction)
		{
			var rtssc = GetSelectedRtssc(isStartMode);
			if (rtssc == null)
				return;

			MaskedTextBox mtxtChainageSelection = isStartMode ? mtxtChainageSelectionStart : mtxtChainageSelectionStop;
			switch (direction)
			{
				case DirectionBgr.ForwardChaining:
					mtxtChainageSelection.Text = (await GetSectionLength(rtssc)).ToString("F0");
					break;
				case DirectionBgr.BackwardChaining:
					mtxtChainageSelection.Text = 0.ToString("F0");
					break;
				default:
					mtxtChainageSelection.Text = "";
					break;
			}
		}
		private void ShowSpecificDirection(bool isStartMode, DirectionBgr direction)
		{
			Panel pnlDirection = isStartMode ? pnlStartDirection : pnlStopDirection;
			RadioButton rdDirection1 = isStartMode ? rdStartDirection1 : rdStopDirection1;
			RadioButton rdDirection2 = isStartMode ? rdStartDirection2 : rdStopDirection2;

			rdDirection1.Checked = (direction == DirectionBgr.ForwardChaining);
			rdDirection2.Checked = (direction == DirectionBgr.BackwardChaining);
		}

		private static Task<IEnumerable<string>> SelectRoutes(GeoCoordinate coord, double searchRadiusInMeters, int? maxRouteNumber)
		{
			return AgentBroker.Instance.TryExecuteOnFirst<IBgrDirectionalAgent, IEnumerable<string>>(agent => agent.SelectRoutes(coord, searchRadiusInMeters, maxRouteNumber)).GetValueOrDefault(Enumerable.Empty<string>());
		}
		private static Task<IEnumerable<IRtssc>> GetRtssFromRoute(string route)
		{
			return AgentBroker.Instance.TryExecuteOnFirst<IBgrDirectionalAgent, IEnumerable<IRtssc>>(agent => agent.GetRtssFromRoute(route)).GetValueOrDefault(Enumerable.Empty<IRtssc>());
		}
		private static Task<BgrData> GetCurrentBgrData()
		{
			return AgentBroker.Instance.TryExecuteOnFirst<IBgrDirectionalAgent, BgrData>(agent => agent.CurrentData).GetValueOrDefault(null);
		}
		private static Task<double> GetSectionLength(IRtssc rtssc)
		{
			return AgentBroker.Instance.TryExecuteOnFirst<IBgrDirectionalAgent, double>(agent => agent.GetSectionLength(rtssc)).GetValueOrDefault(-1);
		}

		private async Task DoAndIgnoreRtssIndexChanged(Func<Task> action)
		{
			_ignoreRtssSelectedIndexChanged = true;
			await action();
			_ignoreRtssSelectedIndexChanged = false;
		}
		private void DoAndIgnoreRtssIndexChanged(Action action)
		{
			_ignoreRtssSelectedIndexChanged = true;
			action();
			_ignoreRtssSelectedIndexChanged = false;
		}

		private async Task<IRtssc> GetNextRTSSCFromRTSS(IRtssc rtssc)
		{
			if (rtssc == null) throw new ArgumentNullException("rtssc");

			// By default, start at a 'chaînage' value equal to 5 to increase the probability
			// that the geoCoordinate returned when querying BGR is the one we are looking for.
			// Doing the inverse operation, i.e. the RTSS obtained from the geoCoordinate,
			// should be on the same 'tronçon/section' with approximately the same 'chaînage' value
			Rtssc rtsscTemp = new Rtssc(rtssc, 5);

			GeoCoordinate coord = await AgentBroker.Instance.TryExecuteOnFirst<IBgrDirectionalAgent, GeoCoordinate>(agent => agent.GeoCodage(rtsscTemp)).GetValueOrDefault(null);
			return await AgentBroker.Instance.TryExecuteOnFirst<IBgrDirectionalAgent, IRtssc>(agent => agent.GetNextRtsscSameDirection(coord)).GetValueOrDefault(null);
		}
	}
}
