using DLC.Multiagent;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using DLC.Scientific.Acquisition.Core.Agents;
using DLC.Scientific.Acquisition.Core.UI;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DLC.Scientific.Acquisition.Agents.DistanceAgent.UI
{
	public partial class CalibrationUI
		: AcquisitionStickyForm
	{
		private new IDistanceAgent ParentAgent { get { return (IDistanceAgent) base.ParentAgent; } }

		private IDisposable _calibrationSubscription;

		public CalibrationUI()
			: base()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			var refEncodeurNumber = this.ParentAgent.ReferenceEncoderNumber;
			if (refEncodeurNumber != 1 && refEncodeurNumber != 2)
				throw new InvalidOperationException(string.Format("Invalid EncoderNumber '{0}'.", refEncodeurNumber));

			SetInitialValues();
			SetInitialState();

			txtOldCalibrationLeft.Text = this.ParentAgent.PPKMLeft.ToString();
			txtOldCalibrationRight.Text = this.ParentAgent.PPKMRight.ToString();

			this.RegisterObserver(
				AgentBroker.Instance.ObserveOne<DistanceData>(this.ParentAgent.Id, "DataSource")
					.ObserveOn(WindowsFormsSynchronizationContext.Current)
					.Subscribe(
						data =>
						{
							if (data.AbsoluteDistance % 10 == 0 || data.AbsoluteDistance < 1000)
								txtDistance.Text = data.AbsoluteDistance.ToString();
						}));
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			base.OnFormClosing(e);

			if (_calibrationSubscription != null)
				_calibrationSubscription.Dispose();

			if (this.ParentAgent.ProviderState == ProviderState.Calibrating)
			{
				this.ParentAgent.StopCalibration()
					.ContinueWith(t => this.ParentAgent.ProcessCalibrationData(false, new DistanceCalibrationData()));
			}
		}

		private async void btnStart_Click(object sender, EventArgs e)
		{
			await TryAction(
				async () =>
				{
					SetInitialValues();
					SetCalibratingState();

					await this.ParentAgent.StartCalibration();

					_calibrationSubscription = AgentBroker.Instance.ObserveOne<DistanceData>(this.ParentAgent.Id, "DataSource")
					   .ObserveOn(WindowsFormsSynchronizationContext.Current)
					   .Subscribe(
						   data =>
						   {
							   txtNewCalibrationRight.Text = string.Format("{0: 0}", data.AbsoluteRightPulseCount);
							   txtNewCalibrationLeft.Text = string.Format("{0: 0}", data.AbsoluteLeftPulseCount);
						   });
				});
		}

		private async void btnStop_Click(object sender, EventArgs e)
		{
			await TryAction(
				async () =>
				{
					try
					{
						this.UseWaitCursor = true;
						Cursor.Current = Cursors.WaitCursor;
						btnStop.Enabled = false;

						var currentCalibration = (DistanceCalibrationData) await this.ParentAgent.StopCalibration();

						_calibrationSubscription.Dispose();

						txtNewCalibrationLeft.Text = currentCalibration.PpkmLeft.ToString();
						txtNewCalibrationRight.Text = currentCalibration.PpkmRight.ToString();
						SetValidationState();
					}
					finally { this.UseWaitCursor = false; Cursor.Current = Cursors.Default; }
				});
		}

		private async void btnValidate_Click(object sender, EventArgs e)
		{
			await ValidateCalibration(true);
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void SetInitialValues()
		{
			txtDistance.Text = "0";
		}
		private void SetInitialState()
		{
			btnStart.Enabled = true;
			btnStop.Enabled = false;
			btnValidate.Enabled = false;
			btnCancel.Enabled = true;
		}
		private void SetCalibratingState()
		{
			btnStart.Enabled = false;
			btnStop.Enabled = true;
			btnValidate.Enabled = false;
			btnCancel.Enabled = false;
		}
		private void SetValidationState()
		{
			btnStart.Enabled = false;
			btnStop.Enabled = false;
			btnValidate.Enabled = true;
			btnCancel.Enabled = true;
		}

		private async Task ValidateCalibration(bool isValid)
		{
			await TryAction(async () =>
				{
					int leftPpkm = 0;
					int rightPpkm = 0;
					int distanceValue = 0;

					if (Int32.TryParse(txtNewCalibrationLeft.Text, out leftPpkm)
						&& Int32.TryParse(txtNewCalibrationRight.Text, out rightPpkm)
						&& Int32.TryParse(txtDistance.Text, out distanceValue))
					{
						var ex = await this.ParentAgent.ProcessCalibrationData(true, new DistanceCalibrationData { ReferenceEncoderNumber = this.ParentAgent.ReferenceEncoderNumber, PpkmLeft = leftPpkm, PpkmRight = rightPpkm, IntervalLength = 1 });
						if (ex != null)
							MessageBox.Show(ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						else
						{
							txtOldCalibrationLeft.Text = leftPpkm.ToString();
							txtOldCalibrationRight.Text = rightPpkm.ToString();
							txtNewCalibrationLeft.Text = "";
							txtNewCalibrationRight.Text = "";
						}

						SetInitialState();
					}
				});
		}

		private async Task TryAction(Func<Task> action)
		{
			if (action == null) throw new ArgumentNullException("action");

			try
			{
				await action();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				SetInitialState();
			}
		}
	}
}