using DLC.Multiagent.Logging;
using NLog;
using NLog.Targets;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace DLC.Multiagent.DesktopApp.UI
{
	internal partial class LoggingDialog
		: ReactiveForm
	{
		private bool _isPaused;
		private readonly Icon _defaultIcon;
		private readonly Icon _errorIcon;

		public LoggingDialog()
		{
			InitializeComponent();

			_defaultIcon = Icon.FromHandle(ImageResources.ServiceShowLog.GetHicon());
			_errorIcon = Icon.FromHandle(ImageResources.AgentStateError.GetHicon());

			this.Icon = _defaultIcon;

			btnConfigure.FlatAppearance.BorderSize = 0;
			btnConfigure.Image = ImageResources.ServiceConfigure;
			btnClear.FlatAppearance.BorderSize = 0;
			btnClear.Image = ImageResources.LogClear;
			btnStartStop.FlatAppearance.BorderSize = 0;
			btnStartStop.Image = ImageResources.LogPause;
			btnShow.FlatAppearance.BorderSize = 0;
			btnShow.Image = ImageResources.LogShow;

			gridLog.Columns.Add("timestamp", "Date");
			gridLog.Columns.Add("level", "Level");
			gridLog.Columns.Add("source", "Source");
			gridLog.Columns.Add("agentId", "Agent ID");
			gridLog.Columns.Add("message", "Message");
			gridLog.Columns.Add("exception", "Exception");

			gridLog.Columns["timestamp"].AutoSizeMode = BestFitColumnMode.DisplayedCells;
			gridLog.Columns["level"].AutoSizeMode = BestFitColumnMode.DisplayedCells;
			gridLog.Columns["source"].AutoSizeMode = BestFitColumnMode.DisplayedCells;
			gridLog.Columns["agentId"].AutoSizeMode = BestFitColumnMode.DisplayedCells;
			gridLog.Columns["message"].AutoSizeMode = BestFitColumnMode.DisplayedCells;
			gridLog.Columns["exception"].AutoSizeMode = BestFitColumnMode.DisplayedCells;

			gridLog.Columns["message"].MaxWidth = gridLog.Width;
			gridLog.Columns["exception"].MaxWidth = gridLog.Width;
			gridLog.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
			gridLog.AllowAutoSizeColumns = true;
			gridLog.AllowColumnResize = true;

			gridLog.BestFitColumns();

			TelerikHelper.RegisterTooltipForRadControl(this, ttMain, gridLog);
		}

		public LoggingDialog(IObservable<MultiagentUI.UITheme> uiThemeDataSource)
			: this()
		{
			if (uiThemeDataSource == null) throw new ArgumentNullException("uiThemeDataSource");

			this.RegisterObserver(
				uiThemeDataSource
					.ObserveOn(WindowsFormsSynchronizationContext.Current)
					.Subscribe(
						uiTheme =>
						{
							gridLog.ThemeName = uiTheme.Theme.ThemeName;
							this.BackColor = uiTheme.BackColor;
						}));
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			cboLogLevel.DataSource = new[] { BrokerLogLevel.Off, BrokerLogLevel.Fatal, BrokerLogLevel.Error, BrokerLogLevel.Warn, BrokerLogLevel.Info, BrokerLogLevel.Debug, BrokerLogLevel.Trace };
			cboLogLevel.SelectedItem = BrokerLogLevel.Warn;

			this.RegisterObserver(
				AgentBroker.Instance.LogDataSource
					.ObserveOn(WindowsFormsSynchronizationContext.Current)
					.Where(entry => !_isPaused)
					.Subscribe(
						entry =>
						{
							// entry.level filter is done in Subscribe to be sure that it is executed on the UI thread
							if (entry.Level >= (BrokerLogLevel) cboLogLevel.SelectedValue)
							{
								var row = gridLog.Rows.AddNew();

								row.Cells["timestamp"].Value = entry.Timestamp;
								row.Cells["level"].Value = entry.Level;
								row.Cells["source"].Value = entry.Source;
								row.Cells["agentId"].Value = entry.AgentId;
								row.Cells["message"].Value = entry.Message;
								row.Cells["exception"].Value = entry.Exception;

								if (gridLog.Rows.Count < 50 || gridLog.Rows.Count % 50 == 0)
									gridLog.BestFitColumns();
							}

							if (entry.Level >= BrokerLogLevel.Error)
								this.Icon = _errorIcon;
							else
								this.Icon = _defaultIcon;
						}));
		}

		protected override void OnResizeEnd(EventArgs e)
		{
			base.OnResizeEnd(e);

			gridLog.Columns["message"].MaxWidth = gridLog.Width;
			gridLog.Columns["exception"].MaxWidth = gridLog.Width;
			gridLog.BestFitColumns();
		}

		private void btnConfigure_Click(object sender, EventArgs e)
		{
			Process.Start(new ProcessStartInfo(AgentBroker.Instance.LogConfigurationFilePath) { UseShellExecute = true });
		}

		private void btnClear_Click(object sender, EventArgs e)
		{
			gridLog.Rows.Clear();
		}

		private void btnStartStop_Click(object sender, EventArgs e)
		{
			_isPaused = !_isPaused;

			if (_isPaused)
			{
				btnStartStop.Image = ImageResources.LogStart;
				ttMain.SetToolTip(btnStartStop, "Start listening");
			}
			else
			{
				btnStartStop.Image = ImageResources.LogPause;
				ttMain.SetToolTip(btnStartStop, "Pause listening");
			}
		}

		private void btnShow_Click(object sender, EventArgs e)
		{
			var fileTarget = LogManager.Configuration.AllTargets.OfType<FileTarget>().FirstOrDefault();

			if (fileTarget == null)
				MessageBox.Show("No trace file has been configured.");
			else
			{
				string filePath = fileTarget.FileName.Render(new LogEventInfo { TimeStamp = DateTime.Now });

				if (File.Exists(filePath))
					Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
				else
					MessageBox.Show(string.Format("Trace file '{0}' has not yet been created.", filePath));
			}
		}

		private void trackOpacity_Scroll(object sender, EventArgs e)
		{
			this.Opacity = (double) trackOpacity.Value / 100;
		}

		private void gridLog_RowFormatting(object sender, RowFormattingEventArgs e)
		{
			if (e.RowElement is GridDataRowElement)
			{
				foreach (var cell in e.RowElement.VisualCells)
					cell.ToolTipText = cell.Text;

				string logLevel = (string) e.RowElement.Data.Cells["level"].Value;

				e.RowElement.DrawBorder = true;
				e.RowElement.BorderBoxStyle = BorderBoxStyle.SingleBorder;
				e.RowElement.BorderGradientStyle = GradientStyles.Solid;
				e.RowElement.BorderWidth = 3;

				if (string.Equals(logLevel, LogLevel.Info.Name, StringComparison.Ordinal))
					e.RowElement.BorderColor = Color.DarkBlue;
				else if (string.Equals(logLevel, LogLevel.Warn.Name, StringComparison.Ordinal))
					e.RowElement.BorderColor = Color.Yellow;
				else if (string.Equals(logLevel, LogLevel.Error.Name, StringComparison.Ordinal))
					e.RowElement.BorderColor = Color.Red;
				else if (string.Equals(logLevel, LogLevel.Fatal.Name, StringComparison.Ordinal))
					e.RowElement.BorderColor = Color.Red;
				else
				{
					e.RowElement.ResetValue(LightVisualElement.BorderBoxStyleProperty);
					e.RowElement.ResetValue(LightVisualElement.BorderColorProperty);
					e.RowElement.ResetValue(LightVisualElement.BorderGradientStyleProperty);
					e.RowElement.ResetValue(LightVisualElement.BorderWidthProperty);
				}
			}
		}

		private void gridLog_CellDoubleClick(object sender, GridViewCellEventArgs e)
		{
			if (e.Row is GridViewDataRowInfo)
			{
				string text = Convert.ToString(e.Value);
				if (!string.IsNullOrEmpty(text))
					MessageBox.Show(text);
			}
		}
	}
}