namespace DLC.Scientific.Acquisition.Agents.FileTransferAgent.UI
{
	partial class FtsUI
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FtsUI));
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.gridFiles = new Telerik.WinControls.UI.RadGridView();
			this.radWizard1 = new Telerik.WinControls.UI.RadWizard();
			this.panel1 = new System.Windows.Forms.Panel();
			this.pnlGeneralInfos.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridFiles)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridFiles.MasterTemplate)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.radWizard1)).BeginInit();
			this.SuspendLayout();
			//
			// pnlGeneralInfos
			//
			this.pnlGeneralInfos.Controls.Add(this.gridFiles);
			this.pnlGeneralInfos.Size = new System.Drawing.Size(649, 607);
			this.pnlGeneralInfos.Controls.SetChildIndex(this.gridFiles, 0);
			//
			// toolTip
			//
			this.toolTip.AutoPopDelay = 5000;
			this.toolTip.InitialDelay = 200;
			this.toolTip.ReshowDelay = 100;
			this.toolTip.ShowAlways = true;
			//
			// gridFiles
			//
			this.gridFiles.AutoScroll = true;
			this.gridFiles.AutoSizeRows = true;
			this.gridFiles.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.gridFiles.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridFiles.Location = new System.Drawing.Point(5, 5);
			//
			// gridFiles
			//
			this.gridFiles.MasterTemplate.AllowAddNewRow = false;
			this.gridFiles.MasterTemplate.AllowDeleteRow = false;
			this.gridFiles.MasterTemplate.AllowEditRow = false;
			this.gridFiles.MasterTemplate.AutoExpandGroups = true;
			this.gridFiles.MasterTemplate.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill;
			this.gridFiles.MasterTemplate.EnableAlternatingRowColor = true;
			this.gridFiles.MasterTemplate.EnableFiltering = true;
			this.gridFiles.MasterTemplate.MultiSelect = true;
			this.gridFiles.Name = "gridFiles";
			this.gridFiles.ReadOnly = true;
			//
			//
			//
			this.gridFiles.RootElement.ControlBounds = new System.Drawing.Rectangle(5, 5, 240, 150);
			this.gridFiles.Size = new System.Drawing.Size(639, 395);
			this.gridFiles.TabIndex = 0;
			this.gridFiles.CreateCell += new Telerik.WinControls.UI.GridViewCreateCellEventHandler(this.gridFiles_CreateCell);
			this.gridFiles.CellFormatting += new Telerik.WinControls.UI.CellFormattingEventHandler(this.gridFiles_CellFormatting);
			this.gridFiles.CellDoubleClick += new Telerik.WinControls.UI.GridViewCellEventHandler(this.gridFiles_CellDoubleClick);
			//
			// radWizard1
			//
			this.radWizard1.CompletionPage = null;
			this.radWizard1.Location = new System.Drawing.Point(0, 0);
			this.radWizard1.Name = "radWizard1";
			this.radWizard1.PageHeaderIcon = ((System.Drawing.Image)(resources.GetObject("radWizard1.PageHeaderIcon")));
			this.radWizard1.Size = new System.Drawing.Size(600, 400);
			this.radWizard1.TabIndex = 0;
			this.radWizard1.WelcomePage = null;
			//
			// panel1
			//
			this.panel1.Location = new System.Drawing.Point(36, 32);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(546, 348);
			this.panel1.TabIndex = 53;
			//
			// FtsUI
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(649, 607);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
			this.Icon = global::DLC.Scientific.Acquisition.Agents.FileTransferAgent.Properties.Resources.Acquisition;
			this.Location = new System.Drawing.Point(0, 0);
			this.Name = "FtsUI";
			this.Text = "File transfer";
			this.pnlGeneralInfos.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gridFiles.MasterTemplate)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridFiles)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.radWizard1)).EndInit();
			this.ResumeLayout(false);

		}



		#endregion

		private System.Windows.Forms.ToolTip toolTip;
		private Telerik.WinControls.UI.RadGridView gridFiles;
		private Telerik.WinControls.UI.RadWizard radWizard1;
		private System.Windows.Forms.Panel panel1;
	}
}