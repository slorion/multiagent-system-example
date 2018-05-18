namespace DLC.Multiagent.DesktopApp.UI
{
	partial class MultiagentUI
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
			Telerik.WinControls.UI.TableViewDefinition tableViewDefinition1 = new Telerik.WinControls.UI.TableViewDefinition();
			this.panel1 = new System.Windows.Forms.Panel();
			this.btnDisplayMode = new System.Windows.Forms.Button();
			this.btnServiceShowLog = new System.Windows.Forms.Button();
			this.btnServiceConfigure = new System.Windows.Forms.Button();
			this.btnServiceStartStop = new System.Windows.Forms.Button();
			this.txtInformation = new System.Windows.Forms.TextBox();
			this.panel2 = new System.Windows.Forms.Panel();
			this.btnAgentCloseAllGui = new System.Windows.Forms.Button();
			this.btnAgentActivate = new System.Windows.Forms.Button();
			this.btnAgentDeactivate = new System.Windows.Forms.Button();
			this.gridAgents = new Telerik.WinControls.UI.RadGridView();
			this.btnAgentRecycle = new System.Windows.Forms.Button();
			this.btnAgentShowMainGui = new System.Windows.Forms.Button();
			this.ttMain = new System.Windows.Forms.ToolTip(this.components);
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridAgents)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridAgents.MasterTemplate)).BeginInit();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.btnDisplayMode);
			this.panel1.Controls.Add(this.btnServiceShowLog);
			this.panel1.Controls.Add(this.btnServiceConfigure);
			this.panel1.Controls.Add(this.btnServiceStartStop);
			this.panel1.Controls.Add(this.txtInformation);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1018, 79);
			this.panel1.TabIndex = 0;
			// 
			// btnDisplayMode
			// 
			this.btnDisplayMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnDisplayMode.Location = new System.Drawing.Point(220, 6);
			this.btnDisplayMode.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.btnDisplayMode.Name = "btnDisplayMode";
			this.btnDisplayMode.Size = new System.Drawing.Size(61, 61);
			this.btnDisplayMode.TabIndex = 7;
			this.ttMain.SetToolTip(this.btnDisplayMode, "Change display mode");
			this.btnDisplayMode.UseVisualStyleBackColor = true;
			this.btnDisplayMode.Click += new System.EventHandler(this.btnDisplayMode_Click);
			// 
			// btnServiceShowLog
			// 
			this.btnServiceShowLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnServiceShowLog.Location = new System.Drawing.Point(149, 6);
			this.btnServiceShowLog.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.btnServiceShowLog.Name = "btnServiceShowLog";
			this.btnServiceShowLog.Size = new System.Drawing.Size(61, 61);
			this.btnServiceShowLog.TabIndex = 2;
			this.ttMain.SetToolTip(this.btnServiceShowLog, "Open log file");
			this.btnServiceShowLog.UseVisualStyleBackColor = true;
			this.btnServiceShowLog.Click += new System.EventHandler(this.btnServiceShowLog_Click);
			// 
			// btnServiceConfigure
			// 
			this.btnServiceConfigure.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnServiceConfigure.Location = new System.Drawing.Point(77, 6);
			this.btnServiceConfigure.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.btnServiceConfigure.Name = "btnServiceConfigure";
			this.btnServiceConfigure.Size = new System.Drawing.Size(61, 61);
			this.btnServiceConfigure.TabIndex = 1;
			this.ttMain.SetToolTip(this.btnServiceConfigure, "Open configuration file");
			this.btnServiceConfigure.UseVisualStyleBackColor = true;
			this.btnServiceConfigure.Click += new System.EventHandler(this.btnServiceConfigure_Click);
			// 
			// btnServiceStartStop
			// 
			this.btnServiceStartStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnServiceStartStop.Location = new System.Drawing.Point(6, 6);
			this.btnServiceStartStop.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.btnServiceStartStop.Name = "btnServiceStartStop";
			this.btnServiceStartStop.Size = new System.Drawing.Size(61, 61);
			this.btnServiceStartStop.TabIndex = 0;
			this.btnServiceStartStop.UseVisualStyleBackColor = true;
			this.btnServiceStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
			// 
			// txtInformation
			// 
			this.txtInformation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtInformation.BackColor = System.Drawing.SystemColors.Control;
			this.txtInformation.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtInformation.Enabled = false;
			this.txtInformation.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtInformation.Location = new System.Drawing.Point(292, 20);
			this.txtInformation.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.txtInformation.Name = "txtInformation";
			this.txtInformation.ReadOnly = true;
			this.txtInformation.Size = new System.Drawing.Size(699, 32);
			this.txtInformation.TabIndex = 6;
			this.txtInformation.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtInformation.WordWrap = false;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.btnAgentCloseAllGui);
			this.panel2.Controls.Add(this.btnAgentActivate);
			this.panel2.Controls.Add(this.btnAgentDeactivate);
			this.panel2.Controls.Add(this.gridAgents);
			this.panel2.Controls.Add(this.btnAgentRecycle);
			this.panel2.Controls.Add(this.btnAgentShowMainGui);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(0, 79);
			this.panel2.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(1018, 844);
			this.panel2.TabIndex = 1;
			// 
			// btnAgentCloseAllGui
			// 
			this.btnAgentCloseAllGui.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAgentCloseAllGui.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnAgentCloseAllGui.Location = new System.Drawing.Point(952, 11);
			this.btnAgentCloseAllGui.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.btnAgentCloseAllGui.Name = "btnAgentCloseAllGui";
			this.btnAgentCloseAllGui.Size = new System.Drawing.Size(61, 61);
			this.btnAgentCloseAllGui.TabIndex = 5;
			this.ttMain.SetToolTip(this.btnAgentCloseAllGui, "Close the main UI of the selected agents");
			this.btnAgentCloseAllGui.UseVisualStyleBackColor = true;
			this.btnAgentCloseAllGui.Click += new System.EventHandler(this.btnAgentCloseAllGui_Click);
			// 
			// btnAgentActivate
			// 
			this.btnAgentActivate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAgentActivate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnAgentActivate.Location = new System.Drawing.Point(666, 11);
			this.btnAgentActivate.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.btnAgentActivate.Name = "btnAgentActivate";
			this.btnAgentActivate.Size = new System.Drawing.Size(61, 61);
			this.btnAgentActivate.TabIndex = 0;
			this.ttMain.SetToolTip(this.btnAgentActivate, "Activate selected agents");
			this.btnAgentActivate.UseVisualStyleBackColor = true;
			this.btnAgentActivate.Click += new System.EventHandler(this.btnAgentActivate_Click);
			// 
			// btnAgentDeactivate
			// 
			this.btnAgentDeactivate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAgentDeactivate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnAgentDeactivate.Location = new System.Drawing.Point(737, 11);
			this.btnAgentDeactivate.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.btnAgentDeactivate.Name = "btnAgentDeactivate";
			this.btnAgentDeactivate.Size = new System.Drawing.Size(61, 61);
			this.btnAgentDeactivate.TabIndex = 1;
			this.ttMain.SetToolTip(this.btnAgentDeactivate, "Deactivate selected agents");
			this.btnAgentDeactivate.UseVisualStyleBackColor = true;
			this.btnAgentDeactivate.Click += new System.EventHandler(this.btnAgentDeactivate_Click);
			// 
			// gridAgents
			// 
			this.gridAgents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridAgents.AutoScroll = true;
			this.gridAgents.AutoSizeRows = true;
			this.gridAgents.Location = new System.Drawing.Point(6, 83);
			this.gridAgents.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			// 
			// 
			// 
			this.gridAgents.MasterTemplate.AllowAddNewRow = false;
			this.gridAgents.MasterTemplate.AllowDeleteRow = false;
			this.gridAgents.MasterTemplate.AllowEditRow = false;
			this.gridAgents.MasterTemplate.AutoExpandGroups = true;
			this.gridAgents.MasterTemplate.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill;
			this.gridAgents.MasterTemplate.EnableAlternatingRowColor = true;
			this.gridAgents.MasterTemplate.EnableFiltering = true;
			this.gridAgents.MasterTemplate.MultiSelect = true;
			this.gridAgents.MasterTemplate.ShowRowHeaderColumn = false;
			this.gridAgents.MasterTemplate.ViewDefinition = tableViewDefinition1;
			this.gridAgents.Name = "gridAgents";
			this.gridAgents.ReadOnly = true;
			this.gridAgents.Size = new System.Drawing.Size(1007, 755);
			this.gridAgents.TabIndex = 4;
			this.gridAgents.ThemeName = "ControlDefault";
			this.gridAgents.CellFormatting += new Telerik.WinControls.UI.CellFormattingEventHandler(this.gridAgents_CellFormatting);
			this.gridAgents.CellDoubleClick += new Telerik.WinControls.UI.GridViewCellEventHandler(this.gridAgents_CellDoubleClick);
			// 
			// btnAgentRecycle
			// 
			this.btnAgentRecycle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAgentRecycle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnAgentRecycle.Location = new System.Drawing.Point(809, 11);
			this.btnAgentRecycle.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.btnAgentRecycle.Name = "btnAgentRecycle";
			this.btnAgentRecycle.Size = new System.Drawing.Size(61, 61);
			this.btnAgentRecycle.TabIndex = 2;
			this.ttMain.SetToolTip(this.btnAgentRecycle, "Recycle selected agents");
			this.btnAgentRecycle.UseVisualStyleBackColor = true;
			this.btnAgentRecycle.Click += new System.EventHandler(this.btnAgentRecycle_Click);
			// 
			// btnAgentShowMainGui
			// 
			this.btnAgentShowMainGui.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAgentShowMainGui.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnAgentShowMainGui.Location = new System.Drawing.Point(880, 11);
			this.btnAgentShowMainGui.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.btnAgentShowMainGui.Name = "btnAgentShowMainGui";
			this.btnAgentShowMainGui.Size = new System.Drawing.Size(61, 61);
			this.btnAgentShowMainGui.TabIndex = 3;
			this.ttMain.SetToolTip(this.btnAgentShowMainGui, "Display main UI of the selected agents");
			this.btnAgentShowMainGui.UseVisualStyleBackColor = true;
			this.btnAgentShowMainGui.Click += new System.EventHandler(this.btnAgentShowMainGui_Click);
			// 
			// MultiagentUI
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1018, 923);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.KeyPreview = true;
			this.Location = new System.Drawing.Point(0, 0);
			this.Margin = new System.Windows.Forms.Padding(11, 11, 11, 11);
			this.Name = "MultiagentUI";
			this.Text = "MultiagentUI";
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MultiagentUI_KeyUp);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gridAgents.MasterTemplate)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridAgents)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.ToolTip ttMain;
		private System.Windows.Forms.TextBox txtInformation;
		private System.Windows.Forms.Button btnServiceStartStop;
		private System.Windows.Forms.Button btnServiceShowLog;
		private System.Windows.Forms.Button btnServiceConfigure;
		private System.Windows.Forms.Button btnAgentRecycle;
		private System.Windows.Forms.Button btnAgentShowMainGui;
		private Telerik.WinControls.UI.RadGridView gridAgents;
		private System.Windows.Forms.Button btnAgentActivate;
		private System.Windows.Forms.Button btnAgentDeactivate;
		private System.Windows.Forms.Button btnAgentCloseAllGui;
		private System.Windows.Forms.Button btnDisplayMode;

	}
}