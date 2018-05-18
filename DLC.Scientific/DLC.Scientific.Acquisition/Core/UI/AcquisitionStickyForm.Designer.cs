namespace DLC.Scientific.Acquisition.Core.UI
{
	partial class AcquisitionStickyForm
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
			this.ttManager = new System.Windows.Forms.ToolTip(this.components);
			this.pnlGeneralInfos = new System.Windows.Forms.Panel();
			this.pnlBaseBottom = new System.Windows.Forms.Panel();
			this.radCollapsiblePanel1 = new Telerik.WinControls.UI.RadCollapsiblePanel();
			this.pnlGeneralInfos.SuspendLayout();
			this.pnlBaseBottom.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.radCollapsiblePanel1)).BeginInit();
			this.SuspendLayout();
			//
			// pnlGeneralInfos
			//
			this.pnlGeneralInfos.Controls.Add(this.pnlBaseBottom);
			this.pnlGeneralInfos.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlGeneralInfos.Location = new System.Drawing.Point(0, 0);
			this.pnlGeneralInfos.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.pnlGeneralInfos.Name = "pnlGeneralInfos";
			this.pnlGeneralInfos.Padding = new System.Windows.Forms.Padding(5);
			this.pnlGeneralInfos.Size = new System.Drawing.Size(659, 205);
			this.pnlGeneralInfos.TabIndex = 52;
			//
			// pnlBaseBottom
			//
			this.pnlBaseBottom.Controls.Add(this.radCollapsiblePanel1);
			this.pnlBaseBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlBaseBottom.Location = new System.Drawing.Point(5, -2);
			this.pnlBaseBottom.Margin = new System.Windows.Forms.Padding(0);
			this.pnlBaseBottom.Name = "pnlBaseBottom";
			this.pnlBaseBottom.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
			this.pnlBaseBottom.Size = new System.Drawing.Size(649, 202);
			this.pnlBaseBottom.TabIndex = 53;
			//
			// radCollapsiblePanel1
			//
			this.radCollapsiblePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.radCollapsiblePanel1.EnableAnimation = false;
			this.radCollapsiblePanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.radCollapsiblePanel1.ForeColor = System.Drawing.Color.Red;
			this.radCollapsiblePanel1.Location = new System.Drawing.Point(0, 5);
			this.radCollapsiblePanel1.Name = "radCollapsiblePanel1";
			this.radCollapsiblePanel1.OwnerBoundsCache = new System.Drawing.Rectangle(0, 0, 649, 221);
			//
			// radCollapsiblePanel1.PanelContainer
			//
			this.radCollapsiblePanel1.PanelContainer.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.radCollapsiblePanel1.PanelContainer.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
			this.radCollapsiblePanel1.PanelContainer.Size = new System.Drawing.Size(647, 169);
			this.radCollapsiblePanel1.Size = new System.Drawing.Size(649, 197);
			this.radCollapsiblePanel1.TabIndex = 52;
			this.radCollapsiblePanel1.Text = "radCollapsiblePanel1";
			this.radCollapsiblePanel1.Expanded += new System.EventHandler(this.radCollapsiblePanel1_Expanded);
			this.radCollapsiblePanel1.Collapsed += new System.EventHandler(this.radCollapsiblePanel1_Collapsed);
			//
			// AcquisitionStickyForm
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(659, 205);
			this.Controls.Add(this.pnlGeneralInfos);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "AcquisitionStickyForm";
			this.Text = "AcquisitionStickyForm";
			this.pnlGeneralInfos.ResumeLayout(false);
			this.pnlBaseBottom.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.radCollapsiblePanel1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ToolTip ttManager;
		protected System.Windows.Forms.Panel pnlGeneralInfos;
		private Telerik.WinControls.UI.RadCollapsiblePanel radCollapsiblePanel1;
		private System.Windows.Forms.Panel pnlBaseBottom;
	}
}