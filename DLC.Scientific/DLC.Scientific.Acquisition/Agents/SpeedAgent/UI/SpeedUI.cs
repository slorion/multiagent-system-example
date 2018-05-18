using DLC.Multiagent;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using DLC.Scientific.Acquisition.Core.Agents;
using DLC.Scientific.Acquisition.Core.UI;
using System;
using System.Drawing;
using System.Reactive.Linq;
using System.Windows.Forms;

namespace DLC.Scientific.Acquisition.Agents.SpeedAgent.UI
{
	public partial class SpeedUI
		: AcquisitionStickyForm
	{
		private new ISpeedAgent ParentAgent { get { return (ISpeedAgent) base.ParentAgent; } }

		public SpeedUI()
			: base()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			this.RegisterObserver(
				AgentBroker.Instance.ObserveOne<SpeedData>(ParentAgent.Id, "DataSource")
					.Sample(TimeSpan.FromMilliseconds(333))
					.Buffer(TimeSpan.FromSeconds(3), 1)
					.ObserveOn(WindowsFormsSynchronizationContext.Current)
					.Subscribe(
						buffer =>
						{
							if (buffer.Count == 0)
							{
								txtDistanceSpeed.BackColor = Color.White;
								txtDistanceSpeed.Text = "0.00";
								txtGpsSpeed.BackColor = Color.White;
								txtGpsSpeed.Text = "0.00";
							}
							else
							{
								var data = buffer[0];

								txtGpsSpeed.Text = data.GpsSpeed.GetValueOrDefault(0).ToString("#0.00");
								txtDistanceSpeed.Text = data.DistanceSpeed.GetValueOrDefault(0).ToString("#0.00");

								switch (data.SpeedSource)
								{
									case SpeedActiveMode.Gps:
										txtDistanceSpeed.BackColor = Color.White;
										txtGpsSpeed.BackColor = Color.GreenYellow;
										break;
									case SpeedActiveMode.Distance:
										txtDistanceSpeed.BackColor = Color.GreenYellow;
										txtGpsSpeed.BackColor = Color.White;
										break;
									default:
										txtDistanceSpeed.BackColor = Color.White;
										txtGpsSpeed.BackColor = Color.White;
										break;
								}
							}
						}));
		}
	}
}