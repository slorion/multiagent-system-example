using DLC.Multiagent;
using DLC.Scientific.Acquisition.Core.Agents;
using DLC.Scientific.Acquisition.Core.UI;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DLC.Scientific.Acquisition.Agents.ShutdownAgent.UI
{
	public partial class ShutdownUI
		: AcquisitionStickyForm
	{
		public ShutdownUI()
			: base()
		{
			InitializeComponent();
		}

		private new IShutdownAgent ParentAgent { get { return (IShutdownAgent) base.ParentAgent; } }

		private async void btnReboot_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(this, "Are you sure you want to restart all connected machines?", "Restart Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				await Task.WhenAll(AgentBroker.Instance.TryExecuteOnAll<IShutdownAgent>(
					a =>
					{
						if (a.Id != this.ParentAgent.Id)
							a.RebootMachine();
					}).ThrowOnError())
					.ContinueWith(t => this.ParentAgent.RebootMachine(), TaskContinuationOptions.OnlyOnRanToCompletion);
			}
		}

		private async void btnShutdown_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(this, "Are you sure you want to shutdown all connected machines?", "Shutdown Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				await Task.WhenAll(AgentBroker.Instance.TryExecuteOnAll<IShutdownAgent>(
					a =>
					{
						if (a.Id != this.ParentAgent.Id)
							a.ShutdownMachine();
					}).ThrowOnError())
					.ContinueWith(t => this.ParentAgent.ShutdownMachine(), TaskContinuationOptions.OnlyOnRanToCompletion);
			}
		}
	}
}