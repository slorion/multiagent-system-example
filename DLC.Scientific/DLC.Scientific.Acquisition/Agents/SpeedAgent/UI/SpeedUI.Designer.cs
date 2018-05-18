namespace DLC.Scientific.Acquisition.Agents.SpeedAgent.UI
{
	partial class SpeedUI
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpeedUI));
			this.txtGpsSpeed = new System.Windows.Forms.TextBox();
			this.txtDistanceSpeed = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.panelSpeedInfo = new System.Windows.Forms.Panel();
			this.pnlGeneralInfos.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.panelSpeedInfo.SuspendLayout();
			this.SuspendLayout();
			//
			// pnlGeneralInfos
			//
			this.pnlGeneralInfos.Controls.Add(this.panelSpeedInfo);
			this.pnlGeneralInfos.Size = new System.Drawing.Size(314, 271);
			this.pnlGeneralInfos.Controls.SetChildIndex(this.panelSpeedInfo, 0);
			//
			// txtGpsSpeed
			//
			this.txtGpsSpeed.Location = new System.Drawing.Point(63, 27);
			this.txtGpsSpeed.Name = "txtGpsSpeed";
			this.txtGpsSpeed.Size = new System.Drawing.Size(67, 20);
			this.txtGpsSpeed.TabIndex = 0;
			//
			// txtDistanceSpeed
			//
			this.txtDistanceSpeed.Location = new System.Drawing.Point(224, 27);
			this.txtDistanceSpeed.Name = "txtDistanceSpeed";
			this.txtDistanceSpeed.Size = new System.Drawing.Size(67, 20);
			this.txtDistanceSpeed.TabIndex = 1;
			//
			// label1
			//
			this.label1.AutoSize = true;
			this.label1.ForeColor = System.Drawing.Color.Yellow;
			this.label1.Location = new System.Drawing.Point(17, 30);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "GPS:";
			//
			// label2
			//
			this.label2.AutoSize = true;
			this.label2.ForeColor = System.Drawing.Color.Yellow;
			this.label2.Location = new System.Drawing.Point(153, 30);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(65, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Distance:";
			//
			// groupBox1
			//
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.txtGpsSpeed);
			this.groupBox1.Controls.Add(this.txtDistanceSpeed);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
			this.groupBox1.ForeColor = System.Drawing.Color.Yellow;
			this.groupBox1.Location = new System.Drawing.Point(0, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(304, 59);
			this.groupBox1.TabIndex = 52;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Vitesse (km/h)";
			//
			// panelSpeedInfo
			//
			this.panelSpeedInfo.Controls.Add(this.groupBox1);
			this.panelSpeedInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelSpeedInfo.Location = new System.Drawing.Point(5, 5);
			this.panelSpeedInfo.Name = "panelSpeedInfo";
			this.panelSpeedInfo.Size = new System.Drawing.Size(304, 59);
			this.panelSpeedInfo.TabIndex = 54;
			//
			// SpeedUI
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(314, 271);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "SpeedUI";
			this.Text = "Speed Monitor";
			this.pnlGeneralInfos.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.panelSpeedInfo.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox txtGpsSpeed;
		private System.Windows.Forms.TextBox txtDistanceSpeed;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Panel panelSpeedInfo;
	}
}