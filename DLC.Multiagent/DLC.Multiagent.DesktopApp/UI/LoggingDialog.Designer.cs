namespace DLC.Multiagent.DesktopApp.UI
{
	partial class LoggingDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			Telerik.WinControls.UI.TableViewDefinition tableViewDefinition3 = new Telerik.WinControls.UI.TableViewDefinition();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.btnConfigure = new System.Windows.Forms.Button();
			this.cboLogLevel = new System.Windows.Forms.ComboBox();
			this.gridLog = new Telerik.WinControls.UI.RadGridView();
			this.trackOpacity = new System.Windows.Forms.TrackBar();
			this.btnClear = new System.Windows.Forms.Button();
			this.btnStartStop = new System.Windows.Forms.Button();
			this.ttMain = new System.Windows.Forms.ToolTip(this.components);
			this.btnShow = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.gridLog)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridLog.MasterTemplate)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackOpacity)).BeginInit();
			this.SuspendLayout();
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(1040, 24);
			this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(79, 25);
			this.label2.TabIndex = 10;
			this.label2.Text = "Opacity";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(737, 21);
			this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(115, 25);
			this.label1.TabIndex = 9;
			this.label1.Text = "Trace Level";
			// 
			// btnConfigure
			// 
			this.btnConfigure.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnConfigure.Location = new System.Drawing.Point(6, 6);
			this.btnConfigure.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.btnConfigure.Name = "btnConfigure";
			this.btnConfigure.Size = new System.Drawing.Size(61, 61);
			this.btnConfigure.TabIndex = 8;
			this.ttMain.SetToolTip(this.btnConfigure, "Configure Journalisation");
			this.btnConfigure.UseVisualStyleBackColor = true;
			this.btnConfigure.Click += new System.EventHandler(this.btnConfigure_Click);
			// 
			// cboLogLevel
			// 
			this.cboLogLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cboLogLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboLogLevel.FormattingEnabled = true;
			this.cboLogLevel.Location = new System.Drawing.Point(864, 18);
			this.cboLogLevel.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.cboLogLevel.Name = "cboLogLevel";
			this.cboLogLevel.Size = new System.Drawing.Size(145, 32);
			this.cboLogLevel.TabIndex = 2;
			// 
			// gridLog
			// 
			this.gridLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridLog.EnableFastScrolling = true;
			this.gridLog.Location = new System.Drawing.Point(6, 78);
			this.gridLog.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			// 
			// 
			// 
			this.gridLog.MasterTemplate.AllowAddNewRow = false;
			this.gridLog.MasterTemplate.AllowDeleteRow = false;
			this.gridLog.MasterTemplate.AllowEditRow = false;
			this.gridLog.MasterTemplate.EnableFiltering = true;
			this.gridLog.MasterTemplate.MultiSelect = true;
			this.gridLog.MasterTemplate.PageSize = 200;
			this.gridLog.MasterTemplate.ShowRowHeaderColumn = false;
			this.gridLog.MasterTemplate.ViewDefinition = tableViewDefinition3;
			this.gridLog.Name = "gridLog";
			this.gridLog.ReadOnly = true;
			this.gridLog.Size = new System.Drawing.Size(1294, 642);
			this.gridLog.TabIndex = 0;
			this.gridLog.RowFormatting += new Telerik.WinControls.UI.RowFormattingEventHandler(this.gridLog_RowFormatting);
			this.gridLog.CellDoubleClick += new Telerik.WinControls.UI.GridViewCellEventHandler(this.gridLog_CellDoubleClick);
			// 
			// trackOpacity
			// 
			this.trackOpacity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.trackOpacity.Location = new System.Drawing.Point(1131, 18);
			this.trackOpacity.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.trackOpacity.Maximum = 100;
			this.trackOpacity.Minimum = 50;
			this.trackOpacity.Name = "trackOpacity";
			this.trackOpacity.Size = new System.Drawing.Size(167, 80);
			this.trackOpacity.TabIndex = 3;
			this.trackOpacity.TickStyle = System.Windows.Forms.TickStyle.None;
			this.trackOpacity.Value = 100;
			this.trackOpacity.Scroll += new System.EventHandler(this.trackOpacity_Scroll);
			// 
			// btnClear
			// 
			this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnClear.Location = new System.Drawing.Point(77, 6);
			this.btnClear.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(61, 61);
			this.btnClear.TabIndex = 11;
			this.ttMain.SetToolTip(this.btnClear, "Empty Grid");
			this.btnClear.UseVisualStyleBackColor = true;
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			// 
			// btnStartStop
			// 
			this.btnStartStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnStartStop.Location = new System.Drawing.Point(149, 6);
			this.btnStartStop.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.btnStartStop.Name = "btnStartStop";
			this.btnStartStop.Size = new System.Drawing.Size(61, 61);
			this.btnStartStop.TabIndex = 12;
			this.ttMain.SetToolTip(this.btnStartStop, "Pause Listening");
			this.btnStartStop.UseVisualStyleBackColor = true;
			this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
			// 
			// btnShow
			// 
			this.btnShow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnShow.Location = new System.Drawing.Point(220, 6);
			this.btnShow.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.btnShow.Name = "btnShow";
			this.btnShow.Size = new System.Drawing.Size(61, 61);
			this.btnShow.TabIndex = 13;
			this.ttMain.SetToolTip(this.btnShow, "Show File");
			this.btnShow.UseVisualStyleBackColor = true;
			this.btnShow.Click += new System.EventHandler(this.btnShow_Click);
			// 
			// LoggingDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1305, 726);
			this.Controls.Add(this.btnShow);
			this.Controls.Add(this.btnStartStop);
			this.Controls.Add(this.btnClear);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnConfigure);
			this.Controls.Add(this.cboLogLevel);
			this.Controls.Add(this.gridLog);
			this.Controls.Add(this.trackOpacity);
			this.Location = new System.Drawing.Point(0, 0);
			this.Margin = new System.Windows.Forms.Padding(11, 11, 11, 11);
			this.Name = "LoggingDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Execution Journal";
			((System.ComponentModel.ISupportInitialize)(this.gridLog.MasterTemplate)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridLog)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackOpacity)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Telerik.WinControls.UI.RadGridView gridLog;
		private System.Windows.Forms.ComboBox cboLogLevel;
		private System.Windows.Forms.TrackBar trackOpacity;
		private System.Windows.Forms.Button btnConfigure;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnClear;
		private System.Windows.Forms.Button btnStartStop;
		private System.Windows.Forms.ToolTip ttMain;
		private System.Windows.Forms.Button btnShow;
	}
}