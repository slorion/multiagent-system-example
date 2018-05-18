using DLC.Framework.UI;
using DLC.Multiagent;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using DLC.Scientific.Acquisition.Core.Agents;
using DLC.Scientific.Acquisition.Core.Agents.Model;
using DLC.Scientific.Acquisition.Core.UI;
using DLC.Scientific.Core.Agents;
using DLC.Scientific.Core.Geocoding.Bgr;
using DLC.Scientific.Core.Geocoding.Gps;
using System;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Forms;

namespace DLC.Scientific.Acquisition.Agents.DashboardAgent.UI
{
	public partial class DashboardUI
		: AcquisitionStickyForm
	{
		private static readonly TimeSpan ReceiveDataTimeout = TimeSpan.FromSeconds(3);
		private static readonly TimeSpan UIRefreshInterval = TimeSpan.FromMilliseconds(333);

		IDisposable _lastDistanceBeforeTriggerSubscription;
		IDisposable _absoluteDistanceSubscription;

		private new IDashboardAgent ParentAgent { get { return (IDashboardAgent) base.ParentAgent; } }

		public DashboardUI()
			: base()
		{
			InitializeComponent();
			this.Icon = Icon.FromHandle(ImageResources.Dashboard.GetHicon());
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			this.RegisterObserver(
				AgentBroker.Instance.ObserveOne<Tuple<string, string, OperationalAgentStates>>(this.ParentAgent.Id, "DependenciesOperationalStateDataSource")
					.ObserveOn(WindowsFormsSynchronizationContext.Current)
					.Subscribe(
						t =>
						{
							var color = (t.Item3 == OperationalAgentStates.None) ? Color.Lime : Color.Red;

							var agentType = Type.GetType(t.Item1);
							if (agentType == typeof(ILocalisationAgent))
								lblLatLon.ForeColor = color;
							else if (agentType == typeof(IDistanceAgent))
								lblDistance.ForeColor = color;
							else if (agentType == typeof(IBgrDirectionalAgent))
							{
								lblRoute.ForeColor = color;
								lblTroncon.ForeColor = color;
								lblSection.ForeColor = color;
								lblSousRoute.ForeColor = color;
								lblChainage.ForeColor = color;
								lbDirection.ForeColor = color;
								lbLongueurSection.ForeColor = color;
							}
						}));

			var emptyBgrData = new BgrData { Rtssc = new Rtssc(), Direction = DirectionBgr.Unknown };
			this.RegisterObserver(
				AgentBroker.Instance.ObserveAny<IBgrDirectionalAgent, BgrData>("DataSource")
					.Buffer(ReceiveDataTimeout, 1)
					.Sample(UIRefreshInterval)
					.Select(buffer => buffer.LastOrDefault() ?? emptyBgrData)
					.ObserveOn(WindowsFormsSynchronizationContext.Current)
					.Subscribe(
						data =>
						{
							lblRoute.Text = string.IsNullOrEmpty(data.Rtssc.Route) ? "-----" : data.Rtssc.Route;
							lblTroncon.Text = string.IsNullOrEmpty(data.Rtssc.Troncon) ? "---" : data.Rtssc.Troncon;
							lblSection.Text = string.IsNullOrEmpty(data.Rtssc.Section) ? "--" : data.Rtssc.Section;
							lblSousRoute.Text = string.IsNullOrEmpty(data.Rtssc.SousRoute) ? "----" : data.Rtssc.SousRoute;
							lblChainage.Text = data.Rtssc.Chainage == null ? "--+---" : string.Format("{0:##0+000}", data.Rtssc.Chainage.Value);
							lbDirection.Text = Convert.ToString((int) data.Direction);

							this.lbCs.Text = string.IsNullOrEmpty(data.Rtssc.CentreDeServiceName) ? "-----" : data.Rtssc.CentreDeServiceName;
							toolTipManager.SetToolTip(lbCs, lbCs.Text);

							this.lbDt.Text = string.IsNullOrEmpty(data.Rtssc.DirectionTerritorialeName) ? "-----" : data.Rtssc.DirectionTerritorialeName;
							toolTipManager.SetToolTip(lbDt, lbDt.Text);

							this.lbLongueurSection.Text = data.Rtssc.Longueur <= 0 ? "-" : data.Rtssc.Longueur.ToString("## ##0") + " m";
						}));

			var emptyDisplayGpsData = new { GpsStatus = GpsStatus.SignalLost, Latitude = new string('-', 4), Longitude = new string('-', 4) };
			this.RegisterObserver(
				AgentBroker.Instance.ObserveAny<ILocalisationAgent, LocalisationData>("DataSource")
					.Buffer(ReceiveDataTimeout, 1)
					.Sample(UIRefreshInterval)
					.Select(
						buffer =>
						{
							var last = buffer.LastOrDefault();

							if (last == null)
								return emptyDisplayGpsData;
							else
								return new { GpsStatus = last.GpsStatus, Latitude = last.CorrectedData.PositionData.Latitude.ToString("##0.000000").Replace(',', '.'), Longitude = last.CorrectedData.PositionData.Longitude.ToString("##0.000000").Replace(',', '.') };
						})
					.ObserveOn(WindowsFormsSynchronizationContext.Current)
					.Subscribe(
						data =>
						{
							lblLatLon.Text = string.Format("{0}, {1}", data.Latitude, data.Longitude);

							switch (data.GpsStatus)
							{
								case GpsStatus.Initializing:
									lblLatLon.ForeColor = Color.Orange;
									break;
								case GpsStatus.Reliable:
									lblLatLon.ForeColor = Color.Lime;
									break;
								case GpsStatus.SignalLost:
									lblLatLon.ForeColor = Color.Red;
									break;
								case GpsStatus.MultiPathDetected:
									lblLatLon.ForeColor = Color.Yellow;
									break;
							}
						}));

			this.RegisterObserver(
				AgentBroker.Instance.ObserveAny<IAcquisitionManagerAgent, AcquisitionManagerStateChangedResult>("AcquisitionManagerStateDataSource")
					.Select(data => new { ProviderState = data.ProviderState, Parameters = data.Parameters })
					.ObserveOn(WindowsFormsSynchronizationContext.Current)
					.Subscribe(
						data =>
						{
							switch (data.ProviderState)
							{
								case ProviderState.Started:
									lblSequenceur.Text = "";
									lblDistanceBeforeStartRecord.Text = "-";
									lblDistanceBeforeStopRecord.Text = "-";
									lblDistance.Text = "-";
									break;
								case ProviderState.InitializedRecord:
									lblSequenceur.Text = data.Parameters.SequenceId;
									lblDistanceBeforeStartRecord.Text = "-";

									// if triggered start/stop
									if (_lastDistanceBeforeTriggerSubscription != null)
									{
										_lastDistanceBeforeTriggerSubscription.Dispose();
										_lastDistanceBeforeTriggerSubscription = null;
									}

									// validation case
									if (_absoluteDistanceSubscription != null)
									{
										_absoluteDistanceSubscription.Dispose();
										_absoluteDistanceSubscription = null;
										lblDistanceBeforeStopRecord.Text = "-";
									}
									break;
								case ProviderState.StartingRecord:
									{
										_lastDistanceBeforeTriggerSubscription = UpdateDistanceBeforeTrigger(data.ProviderState, (TriggeredAcquisitionParameter) data.Parameters);
									}
									break;
								case ProviderState.StartedRecord:
									{
										if (_lastDistanceBeforeTriggerSubscription != null)
											_lastDistanceBeforeTriggerSubscription.Dispose();

										lblDistanceBeforeStartRecord.Text = "-";
										lblDistanceBeforeStopRecord.Text = "-";

										if (_absoluteDistanceSubscription == null)
											_absoluteDistanceSubscription = AgentBroker.Instance.ObserveAny<IDistanceAgent, DistanceData>("DataSource")
												.ObserveOn(WindowsFormsSynchronizationContext.Current)
												.Subscribe(distanceData => lblDistance.Text = string.Format("{0:#,0}" + " m.", distanceData.AbsoluteDistance));
									}
									break;
								case ProviderState.StoppingRecord:
									{
										_lastDistanceBeforeTriggerSubscription = UpdateDistanceBeforeTrigger(data.ProviderState, (TriggeredAcquisitionParameter) data.Parameters);
									}
									break;
							}
						}));

			this.RegisterObserver(
				AgentBroker.Instance.ObserveAny<IEventPanelAgent, bool>("HotkeyModeEnabledDataSource")
					.ObserveOn(WindowsFormsSynchronizationContext.Current)
					.Subscribe(data => UpdateKeyboardHook(data)));

			var emptySpeedData = new SpeedData { CurrentSpeed = -1, SpeedSource = SpeedActiveMode.None };
			this.RegisterObserver(
				AgentBroker.Instance.ObserveAny<ISpeedAgent, SpeedData>("DataSource")
					.Buffer(ReceiveDataTimeout, 1)
					.Sample(UIRefreshInterval)
					.Select(buffer => buffer.LastOrDefault())
					.ObserveOn(WindowsFormsSynchronizationContext.Current)
					.Subscribe(
						data =>
						{
							data = data ?? emptySpeedData;

							SetSpeedSourceImage(data.SpeedSource);
							SetSpeedImages(data.CurrentSpeed, data.IsInRange);
						}));
		}

		/// <summary>
		/// Update distance label related to current acquisition step (and triggered start/stop mode).
		/// </summary>
		private IDisposable UpdateDistanceBeforeTrigger(ProviderState state, TriggeredAcquisitionParameter param)
		{
			IDisposable subscription = null;
			switch (state)
			{
				case ProviderState.StartingRecord:
				case ProviderState.StoppingRecord:
					Label currentLblDistance = state == ProviderState.StartingRecord ? lblDistanceBeforeStartRecord : lblDistanceBeforeStopRecord;
					switch (param.TriggerMode)
					{
						case AcquisitionTriggerMode.Rtssc:
						case AcquisitionTriggerMode.RtsscProximity:
						case AcquisitionTriggerMode.EndSection:
							{
								subscription = AgentBroker.Instance.ObserveAny<ILocalisationAgent, LocalisationData>("DataSource")
									.Sample(UIRefreshInterval)
									.ObserveOn(WindowsFormsSynchronizationContext.Current)
									.Subscribe(data =>
									{
										if (param.GeoCoordinate == null || data == null)
										{
											currentLblDistance.Text = "Error!";
											return;
										}
										double distance = GpsHelper.OrthodromicDistance(param.GeoCoordinate, data.CorrectedData.PositionData) - (param.ProximityRange.HasValue ? param.ProximityRange.Value : 0);
										currentLblDistance.Text = distance >= 1000 ? Math.Round(distance / 1000, 2).ToString("0.00") + " km" : distance.ToString("0") + " m";
									});
								break;
							}
						case AcquisitionTriggerMode.Distance:
							{
								subscription = AgentBroker.Instance.ObserveAny<IDistanceAgent, DistanceData>("DataSource")
									.Sample(UIRefreshInterval)
									.ObserveOn(WindowsFormsSynchronizationContext.Current)
									.Subscribe(
										data =>
										{
											double distance = param.Distance.Value - data.AbsoluteDistance;
											currentLblDistance.Text = distance >= 1000 ? Math.Round(distance / 1000, 2).ToString("0.00") + " km" : distance.ToString("0") + " m";
										});
								break;
							}
					}
					break;
			}
			return subscription;
		}

		private void SetSpeedSourceImage(SpeedActiveMode mode)
		{
			switch (mode)
			{
				case SpeedActiveMode.None:
					this.PicSrcGps.Image = ImageResources.text_gpsInactive;
					this.PicSrcWheel.Image = ImageResources.text_wheelInactive;
					break;
				case SpeedActiveMode.Gps:
					this.PicSrcGps.Image = ImageResources.text_GPSActive;
					this.PicSrcWheel.Image = ImageResources.text_wheelInactive;
					break;
				case SpeedActiveMode.Distance:
					this.PicSrcGps.Image = ImageResources.text_gpsInactive;
					this.PicSrcWheel.Image = ImageResources.text_wheelActive;
					break;
			}
		}
		private void SetSpeedImages(double speedKmh, bool isInRange)
		{
			int errorSpeed = isInRange ? 0 : 10;

			if (speedKmh < 0 || speedKmh >= 1000)
			{
				this.picSpeed100.Image = null;
				this.picSpeed10.Image = null;
				this.picSpeed1.Image = null;
				return;
			}

			if (speedKmh < 100)
			{
				this.picSpeed100.Image = this.NumberList.Images[0 + errorSpeed];
				this.picSpeed10.Image = this.NumberList.Images[(int) speedKmh / 10 + errorSpeed];
				this.picSpeed1.Image = this.NumberList.Images[(int) speedKmh % 10 + errorSpeed];
			}
			else
			{
				int reste = (int) speedKmh % 100;
				this.picSpeed100.Image = this.NumberList.Images[(int) speedKmh / 100 + errorSpeed];
				this.picSpeed10.Image = this.NumberList.Images[(int) reste / 10 + errorSpeed];
				this.picSpeed1.Image = this.NumberList.Images[(int) reste % 10 + errorSpeed];
			}
		}

		private void UpdateKeyboardHook(bool keyboardHookState)
		{
			if ((bool?) picEventHotkeyMode.Image.Tag == keyboardHookState)
				return;

			if (keyboardHookState)
			{
				lblEventHotkeyMode.Text = "Shortcut mode activated";
				lblEventHotkeyMode.ForeColor = Color.Lime;
				picEventHotkeyMode.Image = ImageResources.EventHotkeyModeActive;
				picEventHotkeyMode.Image.Tag = true;
			}
			else
			{
				lblEventHotkeyMode.Text = "Shortcut mode deactivated";
				lblEventHotkeyMode.ForeColor = Color.LightGray;
				picEventHotkeyMode.Image = ImageResources.EventHotkeyModInactive;
				picEventHotkeyMode.Image.Tag = false;
			}
		}

		#region ClipBoard

		private void latitudeLongitudeToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			STASetClipboardText(CopyValues(CopyValuesElements.LatLong));
		}
		private void longitudeToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			STASetClipboardText(CopyValues(CopyValuesElements.Longitude));
		}
		private void latitudeToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			STASetClipboardText(CopyValues(CopyValuesElements.Latitude));
		}
		private void rtsscToolStripMenuItem_Click(object sender, EventArgs e)
		{
			STASetClipboardText(CopyValues(CopyValuesElements.Rtssc));
		}
		private void sequenceurToolStripMenuItem_Click(object sender, EventArgs e)
		{
			STASetClipboardText(CopyValues(CopyValuesElements.Sequenceur));
		}
		private void toutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			STASetClipboardText(CopyValues(CopyValuesElements.All));
		}

		[Flags]
		private enum CopyValuesElements
		{
			Sequenceur = 1,
			Latitude = 1 << 2,
			Longitude = 1 << 3,
			Rtssc = 1 << 4,

			LatLong = Latitude | Longitude,
			All = 0xFF
		}

		private string CopyValues(CopyValuesElements elements)
		{
			var sb = new StringBuilder();

			if (elements.HasFlag(CopyValuesElements.Sequenceur))
				sb.AppendLine(string.Format("{0}", lblSequenceur.Text));

			string lat = lblLatLon.Text.Split(',')[0];
			string lon = lblLatLon.Text.Split(',')[1];

			if (elements.HasFlag(CopyValuesElements.Latitude) && elements.HasFlag(CopyValuesElements.Longitude))
				sb.AppendLine(string.Format("Latitude, Longitude : {0}, {1}", lat, lon));
			else if (elements.HasFlag(CopyValuesElements.Latitude))
				sb.AppendLine(string.Format("Latitude            : {0}", lat));
			else if (elements.HasFlag(CopyValuesElements.Longitude))
				sb.AppendLine(string.Format("Longitude           : {0}", lon));

			if (elements.HasFlag(CopyValuesElements.Rtssc))
			{
				string route = string.IsNullOrEmpty(lblRoute.Text) ? "-----" : lblRoute.Text;
				string troncon = string.IsNullOrEmpty(lblTroncon.Text) ? "---" : lblTroncon.Text;
				string section = string.IsNullOrEmpty(lblSection.Text) ? "--" : lblSection.Text;
				string sRoute = string.IsNullOrEmpty(lblSousRoute.Text) ? "----" : lblSousRoute.Text;
				string chainage = string.IsNullOrEmpty(lblChainage.Text) ? "--+---" : lblChainage.Text;
				sb.AppendLine(string.Format("Rtssc               : {0} {1} {2} {3} {4}", route, troncon, section, sRoute, chainage));
			}

			if (elements == CopyValuesElements.All)
			{
				sb.AppendLine(string.Format("Section length        : {0}", lbLongueurSection.Text));
				sb.AppendLine(string.Format("Distance traveled     : {0}", lblDistance.Text));
				sb.AppendLine(string.Format("Distance before start : {0}", lblDistanceBeforeStartRecord.Text));
				sb.AppendLine(string.Format("Distance before stop  : {0}", lblDistanceBeforeStopRecord.Text));
				sb.AppendLine(string.Format("CS : {0}   DT : {1}", lbCs.Text, lbDt.Text));
			}

			return sb.ToString();
		}

		private static void STASetClipboardText(string text)
		{
			UIThreadingHelper.DispatchUI(() => Clipboard.SetText(text));
		}

		#endregion
	}
}