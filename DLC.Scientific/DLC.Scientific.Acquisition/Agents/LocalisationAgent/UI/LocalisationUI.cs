using DLC.Framework.UI.Forms;
using DLC.Multiagent;
using DLC.Multiagent.Logging;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using DLC.Scientific.Acquisition.Core.Agents;
using DLC.Scientific.Acquisition.Core.UI;
using DLC.Scientific.Core.Geocoding.Gps;
using NLog.Fluent;
using System;
using System.Reactive.Linq;
using System.Windows.Forms;

namespace DLC.Scientific.Acquisition.Agents.LocalisationAgent.UI
{
	public partial class LocalisationUI
		: AcquisitionStickyForm
	{
		private new ILocalisationAgent ParentAgent { get { return (ILocalisationAgent) base.ParentAgent; } }

		public LocalisationUI()
			: base()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (!this.InVSDesigner())
			{
				this.RegisterObserver(
					AgentBroker.Instance.ObserveOne<LocalisationData>(ParentAgent.Id, "DataSource")
						.Sample(TimeSpan.FromMilliseconds(200))
						.ObserveOn(WindowsFormsSynchronizationContext.Current)
						.Subscribe(UpdateData));
			}
		}

		private void UpdateData(LocalisationData data)
		{
			try
			{
				txtLatitude.Text = data.CorrectedData.PositionData.Latitude.ToString("F7");
				txtLongitude.Text = data.CorrectedData.PositionData.Longitude.ToString("F7");
				txtAltitude.Text = data.CorrectedData.PositionData.Altitude.ToString("F2");

				txtSpeedKmh.Text = data.CorrectedData.VelocityData.SpeedKmh.ToString("F2");
				txtSpeedMs.Text = data.CorrectedData.VelocityData.SpeedMs.ToString("F2");
				txtTime.Text = data.CorrectedData.PositionData.Utc.ToString("yyyy-MM-dd HH:mm:ss.fff");

				switch (data.GpsStatus)
				{
					case GpsStatus.Initializing:
						txtStatut.Text = "Initializing...";
						break;
					case GpsStatus.Reliable:
						txtStatut.Text = "Reliable signal";
						break;
					case GpsStatus.SignalLost:
						txtStatut.Text = "Lost signal";
						break;
					case GpsStatus.MultiPathDetected:
						txtStatut.Text = "Multiple paths detected";
						break;
					default:
						txtStatut.Text = "Unknown";
						break;
				}

				txtNbSatellite.Text = data.CorrectedData.PositionData.NbSatellites.ToString();
				txtPdop.Text = data.CorrectedData.PrecisionData.Pdop.ToString();
			}
			catch (Exception ex)
			{
				Log.Error().Message(string.Format("Error while showing GPS data: '{0}'", ex.Message)).Exception(ex).WithAgent(this.ParentAgent.Id).Write();
			}
		}
	}
}