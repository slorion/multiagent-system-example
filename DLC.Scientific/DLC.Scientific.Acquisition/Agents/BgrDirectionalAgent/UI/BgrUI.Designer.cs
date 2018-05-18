namespace DLC.Scientific.Acquisition.Agents.BgrDirectionalAgent.UI
{
	partial class BgrUI
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
			this.lblFiltres = new System.Windows.Forms.Label();
			this.gridRtssc = new Telerik.WinControls.UI.RadGridView();
			this.panel1 = new System.Windows.Forms.Panel();
			this.btnSplit = new System.Windows.Forms.Button();
			this.btnRowUp = new System.Windows.Forms.Button();
			this.btnRowDown = new System.Windows.Forms.Button();
			this.btnRefresh = new System.Windows.Forms.Button();
			this.btnDelete = new System.Windows.Forms.Button();
			this.clstAllowedBGRDataTypes = new DLC.Scientific.Acquisition.Agents.BgrDirectionalAgent.UI.HiddenSelectionCheckedListBox();
			this.gridPanelFill = new System.Windows.Forms.Panel();
			this.ttMain = new System.Windows.Forms.ToolTip(this.components);
			this.pnlGeneralInfos.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridRtssc)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridRtssc.MasterTemplate)).BeginInit();
			this.panel1.SuspendLayout();
			this.gridPanelFill.SuspendLayout();
			this.SuspendLayout();
			//
			// pnlGeneralInfos
			//
			this.pnlGeneralInfos.Controls.Add(this.gridPanelFill);
			this.pnlGeneralInfos.Controls.Add(this.panel1);
			this.pnlGeneralInfos.Size = new System.Drawing.Size(582, 502);
			this.pnlGeneralInfos.Controls.SetChildIndex(this.panel1, 0);
			this.pnlGeneralInfos.Controls.SetChildIndex(this.gridPanelFill, 0);
			//
			// lblFiltres
			//
			this.lblFiltres.AutoSize = true;
			this.lblFiltres.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblFiltres.ForeColor = System.Drawing.Color.Yellow;
			this.lblFiltres.Location = new System.Drawing.Point(3, 12);
			this.lblFiltres.Name = "lblFiltres";
			this.lblFiltres.Size = new System.Drawing.Size(50, 13);
			this.lblFiltres.TabIndex = 57;
			this.lblFiltres.Text = "Include:";
			//
			// gridRtssc
			//
			this.gridRtssc.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.gridRtssc.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridRtssc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.gridRtssc.Location = new System.Drawing.Point(0, 0);
			this.gridRtssc.Name = "gridRtssc";
			//
			//
			//
			this.gridRtssc.RootElement.ControlBounds = new System.Drawing.Rectangle(0, 0, 240, 150);
			this.gridRtssc.Size = new System.Drawing.Size(572, 255);
			this.gridRtssc.TabIndex = 0;
			this.gridRtssc.Text = "gridRtssc";
			this.gridRtssc.CellBeginEdit += new Telerik.WinControls.UI.GridViewCellCancelEventHandler(this.gridRtssc_CellBeginEdit);
			this.gridRtssc.CellValueChanged += new Telerik.WinControls.UI.GridViewCellEventHandler(this.gridRtssc_CellValueChanged);
			//
			// panel1
			//
			this.panel1.Controls.Add(this.btnSplit);
			this.panel1.Controls.Add(this.btnRowUp);
			this.panel1.Controls.Add(this.btnRowDown);
			this.panel1.Controls.Add(this.btnRefresh);
			this.panel1.Controls.Add(this.btnDelete);
			this.panel1.Controls.Add(this.clstAllowedBGRDataTypes);
			this.panel1.Controls.Add(this.lblFiltres);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(5, 5);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(572, 35);
			this.panel1.TabIndex = 62;
			//
			// btnSplit
			//
			this.btnSplit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSplit.Location = new System.Drawing.Point(438, 6);
			this.btnSplit.Name = "btnSplit";
			this.btnSplit.Size = new System.Drawing.Size(24, 24);
			this.btnSplit.TabIndex = 1;
			this.ttMain.SetToolTip(this.btnSplit, "Change current \'voie\'");
			this.btnSplit.UseVisualStyleBackColor = true;
			this.btnSplit.Click += new System.EventHandler(this.btnSplit_Click);
			//
			// btnRowUp
			//
			this.btnRowUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRowUp.Location = new System.Drawing.Point(464, 6);
			this.btnRowUp.Name = "btnRowUp";
			this.btnRowUp.Size = new System.Drawing.Size(24, 24);
			this.btnRowUp.TabIndex = 2;
			this.ttMain.SetToolTip(this.btnRowUp, "Move up RTSSC");
			this.btnRowUp.UseVisualStyleBackColor = true;
			this.btnRowUp.Click += new System.EventHandler(this.btnRowUp_Click);
			//
			// btnRowDown
			//
			this.btnRowDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRowDown.Location = new System.Drawing.Point(490, 6);
			this.btnRowDown.Name = "btnRowDown";
			this.btnRowDown.Size = new System.Drawing.Size(24, 24);
			this.btnRowDown.TabIndex = 3;
			this.ttMain.SetToolTip(this.btnRowDown, "Move down RTSSC");
			this.btnRowDown.UseVisualStyleBackColor = true;
			this.btnRowDown.Click += new System.EventHandler(this.btnRowDown_Click);
			//
			// btnRefresh
			//
			this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRefresh.Location = new System.Drawing.Point(542, 6);
			this.btnRefresh.Name = "btnRefresh";
			this.btnRefresh.Size = new System.Drawing.Size(24, 24);
			this.btnRefresh.TabIndex = 5;
			this.ttMain.SetToolTip(this.btnRefresh, "Refresh RTSSC dropdown lists with roads within 10 km");
			this.btnRefresh.UseVisualStyleBackColor = true;
			this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
			//
			// btnDelete
			//
			this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDelete.Location = new System.Drawing.Point(516, 6);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(24, 24);
			this.btnDelete.TabIndex = 4;
			this.ttMain.SetToolTip(this.btnDelete, "Delete RTSSC");
			this.btnDelete.UseVisualStyleBackColor = true;
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			//
			// clstAllowedBGRDataTypes
			//
			this.clstAllowedBGRDataTypes.BackColor = System.Drawing.Color.Black;
			this.clstAllowedBGRDataTypes.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.clstAllowedBGRDataTypes.CheckOnClick = true;
			this.clstAllowedBGRDataTypes.ForeColor = System.Drawing.Color.Yellow;
			this.clstAllowedBGRDataTypes.FormattingEnabled = true;
			this.clstAllowedBGRDataTypes.Location = new System.Drawing.Point(59, 12);
			this.clstAllowedBGRDataTypes.MultiColumn = true;
			this.clstAllowedBGRDataTypes.Name = "clstAllowedBGRDataTypes";
			this.clstAllowedBGRDataTypes.Size = new System.Drawing.Size(373, 15);
			this.clstAllowedBGRDataTypes.TabIndex = 0;
			this.clstAllowedBGRDataTypes.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clstAllowedBGRDataTypes_ItemCheck);
			//
			// gridPanelFill
			//
			this.gridPanelFill.Controls.Add(this.gridRtssc);
			this.gridPanelFill.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridPanelFill.Location = new System.Drawing.Point(5, 40);
			this.gridPanelFill.Name = "gridPanelFill";
			this.gridPanelFill.Size = new System.Drawing.Size(572, 255);
			this.gridPanelFill.TabIndex = 63;
			//
			// BgrUI
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(582, 502);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
			this.Location = new System.Drawing.Point(0, 0);
			this.MinimumSize = new System.Drawing.Size(590, 300);
			this.Name = "BgrUI";
			this.Text = "BgrUI";
			this.pnlGeneralInfos.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gridRtssc.MasterTemplate)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridRtssc)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.gridPanelFill.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label lblFiltres;
		private Telerik.WinControls.UI.RadGridView gridRtssc;
		private HiddenSelectionCheckedListBox clstAllowedBGRDataTypes;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel gridPanelFill;
		private System.Windows.Forms.Button btnRefresh;
		private System.Windows.Forms.Button btnDelete;
		private System.Windows.Forms.Button btnRowUp;
		private System.Windows.Forms.Button btnRowDown;
		private System.Windows.Forms.ToolTip ttMain;
		private System.Windows.Forms.Button btnSplit;
	}
}