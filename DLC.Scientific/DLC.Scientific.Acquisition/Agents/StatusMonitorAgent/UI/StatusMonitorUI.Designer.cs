namespace DLC.Scientific.Acquisition.Agents.StatusMonitorAgent.UI
{
	partial class StatusMonitorUI
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
			System.Windows.Forms.ColumnHeader providerStateColumnHeader;
			this.imlStateIcons = new System.Windows.Forms.ImageList(this.components);
			this.lvwProviderAgents = new System.Windows.Forms.ListView();
			this.ttGlobal = new System.Windows.Forms.ToolTip(this.components);
			providerStateColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.pnlGeneralInfos.SuspendLayout();
			this.SuspendLayout();
			//
			// pnlGeneralInfos
			//
			this.pnlGeneralInfos.Controls.Add(this.lvwProviderAgents);
			this.pnlGeneralInfos.Size = new System.Drawing.Size(465, 522);
			this.pnlGeneralInfos.Controls.SetChildIndex(this.lvwProviderAgents, 0);
			//
			// providerStateColumnHeader
			//
			providerStateColumnHeader.Text = "State";
			providerStateColumnHeader.Width = -1;
			//
			// imlStateIcons
			//
			this.imlStateIcons.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.imlStateIcons.ImageSize = new System.Drawing.Size(46, 46);
			this.imlStateIcons.TransparentColor = System.Drawing.Color.Transparent;
			//
			// lvwProviderAgents
			//
			this.lvwProviderAgents.Alignment = System.Windows.Forms.ListViewAlignment.SnapToGrid;
			this.lvwProviderAgents.BackColor = System.Drawing.Color.Black;
			this.lvwProviderAgents.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            providerStateColumnHeader});
			this.lvwProviderAgents.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvwProviderAgents.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lvwProviderAgents.FullRowSelect = true;
			this.lvwProviderAgents.GridLines = true;
			this.lvwProviderAgents.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.lvwProviderAgents.HideSelection = false;
			this.lvwProviderAgents.LargeImageList = this.imlStateIcons;
			this.lvwProviderAgents.Location = new System.Drawing.Point(5, 5);
			this.lvwProviderAgents.Name = "lvwProviderAgents";
			this.lvwProviderAgents.ShowItemToolTips = true;
			this.lvwProviderAgents.Size = new System.Drawing.Size(455, 310);
			this.lvwProviderAgents.SmallImageList = this.imlStateIcons;
			this.lvwProviderAgents.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.lvwProviderAgents.TabIndex = 1;
			this.lvwProviderAgents.UseCompatibleStateImageBehavior = false;
			//
			// StatusMonitorUI
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(465, 522);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
			this.Location = new System.Drawing.Point(0, 0);
			this.Name = "StatusMonitorUI";
			this.Text = "Status Monitor";
			this.pnlGeneralInfos.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ImageList imlStateIcons;
		private System.Windows.Forms.ListView lvwProviderAgents;
		private System.Windows.Forms.ToolTip ttGlobal;
	}
}