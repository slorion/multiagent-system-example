namespace DLC.Scientific.Acquisition.Agents.EventPanelAgent.UI
{
	partial class EventPanelUI
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EventPanelUI));
			this.pnlEvents = new System.Windows.Forms.FlowLayoutPanel();
			this.pnlLog = new System.Windows.Forms.Panel();
			this.gridLog = new Telerik.WinControls.UI.RadGridView();
			this.btnEditLog = new System.Windows.Forms.Button();
			this.pnlEventAndLog = new System.Windows.Forms.SplitContainer();
			this.chkOrientation = new System.Windows.Forms.CheckBox();
			this.lblHotkeyMode = new System.Windows.Forms.Label();
			this.picHotkeyMode = new System.Windows.Forms.PictureBox();
			this.panel2 = new System.Windows.Forms.Panel();
			this.panel3 = new System.Windows.Forms.Panel();
			this.pnlGeneralInfos.SuspendLayout();
			this.pnlLog.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridLog)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridLog.MasterTemplate)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlEventAndLog)).BeginInit();
			this.pnlEventAndLog.Panel1.SuspendLayout();
			this.pnlEventAndLog.Panel2.SuspendLayout();
			this.pnlEventAndLog.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picHotkeyMode)).BeginInit();
			this.panel2.SuspendLayout();
			this.panel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlGeneralInfos
			// 
			this.pnlGeneralInfos.Controls.Add(this.panel3);
			this.pnlGeneralInfos.Controls.Add(this.panel2);
			this.pnlGeneralInfos.Size = new System.Drawing.Size(717, 673);
			this.pnlGeneralInfos.Controls.SetChildIndex(this.panel2, 0);
			this.pnlGeneralInfos.Controls.SetChildIndex(this.panel3, 0);
			// 
			// pnlEvents
			// 
			this.pnlEvents.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlEvents.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.pnlEvents.Location = new System.Drawing.Point(0, 0);
			this.pnlEvents.Name = "pnlEvents";
			this.pnlEvents.Size = new System.Drawing.Size(196, 415);
			this.pnlEvents.TabIndex = 0;
			// 
			// pnlLog
			// 
			this.pnlLog.Controls.Add(this.gridLog);
			this.pnlLog.Controls.Add(this.btnEditLog);
			this.pnlLog.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlLog.Location = new System.Drawing.Point(0, 0);
			this.pnlLog.Name = "pnlLog";
			this.pnlLog.Size = new System.Drawing.Size(507, 415);
			this.pnlLog.TabIndex = 1;
			// 
			// gridLog
			// 
			this.gridLog.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridLog.Location = new System.Drawing.Point(0, 0);
			this.gridLog.Name = "gridLog";
			this.gridLog.Size = new System.Drawing.Size(507, 415);
			this.gridLog.TabIndex = 2;
			this.gridLog.Text = "radGridView1";
			this.gridLog.CellDoubleClick += new Telerik.WinControls.UI.GridViewCellEventHandler(this.gridLog_CellDoubleClick);
			// 
			// btnEditLog
			// 
			this.btnEditLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnEditLog.Location = new System.Drawing.Point(543, -289);
			this.btnEditLog.Name = "btnEditLog";
			this.btnEditLog.Size = new System.Drawing.Size(75, 23);
			this.btnEditLog.TabIndex = 1;
			this.btnEditLog.Text = "Éditer";
			this.btnEditLog.UseVisualStyleBackColor = true;
			this.btnEditLog.Click += new System.EventHandler(this.btnEditLog_Click);
			// 
			// pnlEventAndLog
			// 
			this.pnlEventAndLog.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlEventAndLog.Location = new System.Drawing.Point(0, 0);
			this.pnlEventAndLog.Name = "pnlEventAndLog";
			// 
			// pnlEventAndLog.Panel1
			// 
			this.pnlEventAndLog.Panel1.Controls.Add(this.pnlEvents);
			this.pnlEventAndLog.Panel1MinSize = 65;
			// 
			// pnlEventAndLog.Panel2
			// 
			this.pnlEventAndLog.Panel2.Controls.Add(this.pnlLog);
			this.pnlEventAndLog.Panel2MinSize = 0;
			this.pnlEventAndLog.Size = new System.Drawing.Size(707, 415);
			this.pnlEventAndLog.SplitterDistance = 196;
			this.pnlEventAndLog.TabIndex = 53;
			// 
			// chkOrientation
			// 
			this.chkOrientation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.chkOrientation.Appearance = System.Windows.Forms.Appearance.Button;
			this.chkOrientation.AutoSize = true;
			this.chkOrientation.Location = new System.Drawing.Point(612, 7);
			this.chkOrientation.Name = "chkOrientation";
			this.chkOrientation.Size = new System.Drawing.Size(92, 23);
			this.chkOrientation.TabIndex = 54;
			this.chkOrientation.Text = "Mode horizontal";
			this.chkOrientation.UseVisualStyleBackColor = true;
			this.chkOrientation.Click += new System.EventHandler(this.chkOrientation_Click);
			// 
			// lblHotkeyMode
			// 
			this.lblHotkeyMode.AutoSize = true;
			this.lblHotkeyMode.Cursor = System.Windows.Forms.Cursors.Hand;
			this.lblHotkeyMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblHotkeyMode.ForeColor = System.Drawing.Color.LightGray;
			this.lblHotkeyMode.Location = new System.Drawing.Point(51, 12);
			this.lblHotkeyMode.Name = "lblHotkeyMode";
			this.lblHotkeyMode.Size = new System.Drawing.Size(154, 13);
			this.lblHotkeyMode.TabIndex = 55;
			this.lblHotkeyMode.Text = "Mode raccourci désactivé";
			this.lblHotkeyMode.Click += new System.EventHandler(this.lblHotkeyMode_Click);
			// 
			// picHotkeyMode
			// 
			this.picHotkeyMode.BackColor = System.Drawing.Color.LightGray;
			this.picHotkeyMode.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("picHotkeyMode.BackgroundImage")));
			this.picHotkeyMode.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.picHotkeyMode.Cursor = System.Windows.Forms.Cursors.Hand;
			this.picHotkeyMode.Location = new System.Drawing.Point(13, 3);
			this.picHotkeyMode.Name = "picHotkeyMode";
			this.picHotkeyMode.Size = new System.Drawing.Size(32, 32);
			this.picHotkeyMode.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.picHotkeyMode.TabIndex = 56;
			this.picHotkeyMode.TabStop = false;
			this.picHotkeyMode.Click += new System.EventHandler(this.picHotkeyMode_Click);
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.picHotkeyMode);
			this.panel2.Controls.Add(this.lblHotkeyMode);
			this.panel2.Controls.Add(this.chkOrientation);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Location = new System.Drawing.Point(5, 5);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(707, 46);
			this.panel2.TabIndex = 57;
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.pnlEventAndLog);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(5, 51);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(707, 415);
			this.panel3.TabIndex = 54;
			// 
			// EventPanelUI
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(717, 673);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
			this.Location = new System.Drawing.Point(0, 0);
			this.Name = "EventPanelUI";
			this.Text = "EventPanelUI";
			this.pnlGeneralInfos.ResumeLayout(false);
			this.pnlLog.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gridLog.MasterTemplate)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridLog)).EndInit();
			this.pnlEventAndLog.Panel1.ResumeLayout(false);
			this.pnlEventAndLog.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlEventAndLog)).EndInit();
			this.pnlEventAndLog.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.picHotkeyMode)).EndInit();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.panel3.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.FlowLayoutPanel pnlEvents;
		private System.Windows.Forms.Panel pnlLog;
		private System.Windows.Forms.Button btnEditLog;
		private System.Windows.Forms.SplitContainer pnlEventAndLog;
		private System.Windows.Forms.CheckBox chkOrientation;
		private System.Windows.Forms.Label lblHotkeyMode;
		private System.Windows.Forms.PictureBox picHotkeyMode;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel panel3;
		private Telerik.WinControls.UI.RadGridView gridLog;
	}
}