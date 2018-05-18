namespace DLC.Scientific.Acquisition.Agents.ShutdownAgent.UI
{
	partial class ShutdownUI
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.labelReboot = new System.Windows.Forms.Label();
			this.btnShutdown = new System.Windows.Forms.PictureBox();
			this.btnReboot = new System.Windows.Forms.PictureBox();
			this.pnlGeneralInfos.SuspendLayout();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.btnShutdown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.btnReboot)).BeginInit();
			this.SuspendLayout();
			//
			// pnlGeneralInfos
			//
			this.pnlGeneralInfos.Controls.Add(this.panel1);
			this.pnlGeneralInfos.Size = new System.Drawing.Size(439, 397);
			this.pnlGeneralInfos.Controls.SetChildIndex(this.panel1, 0);
			//
			// panel1
			//
			this.panel1.BackColor = System.Drawing.Color.CornflowerBlue;
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.labelReboot);
			this.panel1.Controls.Add(this.btnShutdown);
			this.panel1.Controls.Add(this.btnReboot);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(5, 5);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(429, 185);
			this.panel1.TabIndex = 0;
			//
			// label1
			//
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
			this.label1.Location = new System.Drawing.Point(57, 148);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(97, 32);
			this.label1.TabIndex = 6;
			this.label1.Text = "Shutdown";
			//
			// labelReboot
			//
			this.labelReboot.AutoSize = true;
			this.labelReboot.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
			this.labelReboot.Location = new System.Drawing.Point(246, 148);
			this.labelReboot.Name = "labelReboot";
			this.labelReboot.Size = new System.Drawing.Size(150, 32);
			this.labelReboot.TabIndex = 5;
			this.labelReboot.Text = "Restart";
			//
			// btnShutdown
			//
			this.btnShutdown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btnShutdown.Image = global::DLC.Scientific.Acquisition.Agents.ShutdownAgent.Properties.Resources.Shutdown;
			this.btnShutdown.Location = new System.Drawing.Point(41, 17);
			this.btnShutdown.Name = "btnShutdown";
			this.btnShutdown.Size = new System.Drawing.Size(128, 128);
			this.btnShutdown.TabIndex = 4;
			this.btnShutdown.TabStop = false;
			this.btnShutdown.Click += new System.EventHandler(this.btnShutdown_Click);
			//
			// btnReboot
			//
			this.btnReboot.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btnReboot.Image = global::DLC.Scientific.Acquisition.Agents.ShutdownAgent.Properties.Resources.Reboot;
			this.btnReboot.Location = new System.Drawing.Point(257, 17);
			this.btnReboot.Name = "btnReboot";
			this.btnReboot.Size = new System.Drawing.Size(128, 128);
			this.btnReboot.TabIndex = 3;
			this.btnReboot.TabStop = false;
			this.btnReboot.Click += new System.EventHandler(this.btnReboot_Click);
			//
			// ShutdownUI
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.MediumBlue;
			this.ClientSize = new System.Drawing.Size(439, 397);
			this.Location = new System.Drawing.Point(0, 0);
			this.MaximizeBox = false;
			this.Name = "ShutdownUI";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Shutdown and restart";
			this.pnlGeneralInfos.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.btnShutdown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.btnReboot)).EndInit();
			this.ResumeLayout(false);

		}


		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.PictureBox btnReboot;
		private System.Windows.Forms.PictureBox btnShutdown;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelReboot;
	}
}