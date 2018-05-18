namespace DLC.Scientific.Acquisition.Agents.LocalisationAgent.UI
{
    partial class LocalisationUI
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.txtAltitude = new System.Windows.Forms.TextBox();
			this.txtLongitude = new System.Windows.Forms.TextBox();
			this.txtLatitude = new System.Windows.Forms.TextBox();
			this.lblAffAlt = new System.Windows.Forms.Label();
			this.lblAffLong = new System.Windows.Forms.Label();
			this.lblAffLat = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.txtSpeedMs = new System.Windows.Forms.TextBox();
			this.txtSpeedKmh = new System.Windows.Forms.TextBox();
			this.txtTime = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.txtStatut = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.txtPdop = new System.Windows.Forms.TextBox();
			this.txtNbSatellite = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.pnlGpsInfo = new System.Windows.Forms.Panel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.pnlGeneralInfos.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.pnlGpsInfo.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			//
			// pnlGeneralInfos
			//
			this.pnlGeneralInfos.Controls.Add(this.pnlGpsInfo);
			this.pnlGeneralInfos.Size = new System.Drawing.Size(630, 453);
			this.pnlGeneralInfos.Controls.SetChildIndex(this.pnlGpsInfo, 0);
			//
			// groupBox1
			//
			this.groupBox1.Controls.Add(this.splitContainer2);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
			this.groupBox1.ForeColor = System.Drawing.Color.Yellow;
			this.groupBox1.Location = new System.Drawing.Point(0, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(620, 241);
			this.groupBox1.TabIndex = 52;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Data";
			//
			// splitContainer2
			//
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(3, 16);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			//
			// splitContainer2.Panel1
			//
			this.splitContainer2.Panel1.Controls.Add(this.splitContainer1);
			//
			// splitContainer2.Panel2
			//
			this.splitContainer2.Panel2.Controls.Add(this.groupBox4);
			this.splitContainer2.Size = new System.Drawing.Size(614, 222);
			this.splitContainer2.SplitterDistance = 133;
			this.splitContainer2.TabIndex = 57;
			//
			// splitContainer1
			//
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			//
			// splitContainer1.Panel1
			//
			this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
			//
			// splitContainer1.Panel2
			//
			this.splitContainer1.Panel2.Controls.Add(this.groupBox3);
			this.splitContainer1.Size = new System.Drawing.Size(614, 133);
			this.splitContainer1.SplitterDistance = 302;
			this.splitContainer1.TabIndex = 56;
			//
			// groupBox2
			//
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.txtAltitude);
			this.groupBox2.Controls.Add(this.txtLongitude);
			this.groupBox2.Controls.Add(this.txtLatitude);
			this.groupBox2.Controls.Add(this.lblAffAlt);
			this.groupBox2.Controls.Add(this.lblAffLong);
			this.groupBox2.Controls.Add(this.lblAffLat);
			this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
			this.groupBox2.ForeColor = System.Drawing.Color.Yellow;
			this.groupBox2.Location = new System.Drawing.Point(7, 3);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(288, 108);
			this.groupBox2.TabIndex = 53;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Coordinates";
			//
			// txtAltitude
			//
			this.txtAltitude.Location = new System.Drawing.Point(103, 74);
			this.txtAltitude.Name = "txtAltitude";
			this.txtAltitude.ReadOnly = true;
			this.txtAltitude.Size = new System.Drawing.Size(179, 20);
			this.txtAltitude.TabIndex = 5;
			//
			// txtLongitude
			//
			this.txtLongitude.Location = new System.Drawing.Point(103, 49);
			this.txtLongitude.Name = "txtLongitude";
			this.txtLongitude.ReadOnly = true;
			this.txtLongitude.Size = new System.Drawing.Size(179, 20);
			this.txtLongitude.TabIndex = 4;
			//
			// txtLatitude
			//
			this.txtLatitude.Location = new System.Drawing.Point(103, 24);
			this.txtLatitude.Name = "txtLatitude";
			this.txtLatitude.ReadOnly = true;
			this.txtLatitude.Size = new System.Drawing.Size(179, 20);
			this.txtLatitude.TabIndex = 3;
			//
			// lblAffAlt
			//
			this.lblAffAlt.AutoSize = true;
			this.lblAffAlt.Location = new System.Drawing.Point(13, 77);
			this.lblAffAlt.Name = "lblAffAlt";
			this.lblAffAlt.Size = new System.Drawing.Size(58, 13);
			this.lblAffAlt.TabIndex = 2;
			this.lblAffAlt.Text = "Altitude:";
			//
			// lblAffLong
			//
			this.lblAffLong.AutoSize = true;
			this.lblAffLong.Location = new System.Drawing.Point(13, 51);
			this.lblAffLong.Name = "lblAffLong";
			this.lblAffLong.Size = new System.Drawing.Size(71, 13);
			this.lblAffLong.TabIndex = 1;
			this.lblAffLong.Text = "Longitude:";
			//
			// lblAffLat
			//
			this.lblAffLat.AutoSize = true;
			this.lblAffLat.Location = new System.Drawing.Point(13, 27);
			this.lblAffLat.Name = "lblAffLat";
			this.lblAffLat.Size = new System.Drawing.Size(61, 13);
			this.lblAffLat.TabIndex = 0;
			this.lblAffLat.Text = "Latitude:";
			//
			// groupBox3
			//
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Controls.Add(this.txtSpeedMs);
			this.groupBox3.Controls.Add(this.txtSpeedKmh);
			this.groupBox3.Controls.Add(this.txtTime);
			this.groupBox3.Controls.Add(this.label2);
			this.groupBox3.Controls.Add(this.label3);
			this.groupBox3.Controls.Add(this.label6);
			this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
			this.groupBox3.ForeColor = System.Drawing.Color.Yellow;
			this.groupBox3.Location = new System.Drawing.Point(7, 3);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(294, 108);
			this.groupBox3.TabIndex = 54;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Speed and time";
			//
			// txtSpeedMs
			//
			this.txtSpeedMs.Location = new System.Drawing.Point(103, 49);
			this.txtSpeedMs.Name = "txtSpeedMs";
			this.txtSpeedMs.ReadOnly = true;
			this.txtSpeedMs.Size = new System.Drawing.Size(179, 20);
			this.txtSpeedMs.TabIndex = 4;
			//
			// txtSpeedKmh
			//
			this.txtSpeedKmh.Location = new System.Drawing.Point(103, 24);
			this.txtSpeedKmh.Name = "txtSpeedKmh";
			this.txtSpeedKmh.ReadOnly = true;
			this.txtSpeedKmh.Size = new System.Drawing.Size(179, 20);
			this.txtSpeedKmh.TabIndex = 3;
			//
			// txtTime
			//
			this.txtTime.Location = new System.Drawing.Point(103, 74);
			this.txtTime.Name = "txtTime";
			this.txtTime.ReadOnly = true;
			this.txtTime.Size = new System.Drawing.Size(179, 20);
			this.txtTime.TabIndex = 3;
			//
			// label2
			//
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 51);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(37, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "m/s:";
			//
			// label3
			//
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(13, 27);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(45, 13);
			this.label3.TabIndex = 0;
			this.label3.Text = "km/h:";
			//
			// label6
			//
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(13, 77);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(81, 13);
			this.label6.TabIndex = 0;
			this.label6.Text = "GPS time:";
			//
			// groupBox4
			//
			this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox4.Controls.Add(this.txtStatut);
			this.groupBox4.Controls.Add(this.label1);
			this.groupBox4.Controls.Add(this.txtPdop);
			this.groupBox4.Controls.Add(this.txtNbSatellite);
			this.groupBox4.Controls.Add(this.label4);
			this.groupBox4.Controls.Add(this.label5);
			this.groupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
			this.groupBox4.ForeColor = System.Drawing.Color.Yellow;
			this.groupBox4.Location = new System.Drawing.Point(7, 2);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(602, 59);
			this.groupBox4.TabIndex = 55;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Signal quality";
			//
			// txtStatut
			//
			this.txtStatut.Location = new System.Drawing.Point(103, 25);
			this.txtStatut.Name = "txtStatut";
			this.txtStatut.ReadOnly = true;
			this.txtStatut.Size = new System.Drawing.Size(146, 20);
			this.txtStatut.TabIndex = 7;
			//
			// label1
			//
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(14, 28);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(49, 13);
			this.label1.TabIndex = 6;
			this.label1.Text = "Status:";
			//
			// txtPdop
			//
			this.txtPdop.Location = new System.Drawing.Point(538, 25);
			this.txtPdop.Name = "txtPdop";
			this.txtPdop.ReadOnly = true;
			this.txtPdop.Size = new System.Drawing.Size(57, 20);
			this.txtPdop.TabIndex = 5;
			//
			// txtNbSatellite
			//
			this.txtNbSatellite.Location = new System.Drawing.Point(416, 25);
			this.txtNbSatellite.Name = "txtNbSatellite";
			this.txtNbSatellite.ReadOnly = true;
			this.txtNbSatellite.Size = new System.Drawing.Size(43, 20);
			this.txtNbSatellite.TabIndex = 4;
			//
			// label4
			//
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(477, 28);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(44, 13);
			this.label4.TabIndex = 2;
			this.label4.Text = "Pdop:";
			//
			// label5
			//
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(280, 28);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(130, 13);
			this.label5.TabIndex = 1;
			this.label5.Text = "Satellite count:";
			//
			// pnlGpsInfo
			//
			this.pnlGpsInfo.Controls.Add(this.panel1);
			this.pnlGpsInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlGpsInfo.Location = new System.Drawing.Point(5, 5);
			this.pnlGpsInfo.Name = "pnlGpsInfo";
			this.pnlGpsInfo.Size = new System.Drawing.Size(620, 241);
			this.pnlGpsInfo.TabIndex = 53;
			//
			// panel1
			//
			this.panel1.Controls.Add(this.groupBox1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(620, 241);
			this.panel1.TabIndex = 53;
			//
			// LocalisationUI
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.ClientSize = new System.Drawing.Size(630, 453);
			this.Name = "LocalisationUI";
			this.Text = "GPS agent";
			this.pnlGeneralInfos.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.pnlGpsInfo.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txtPdop;
        private System.Windows.Forms.TextBox txtNbSatellite;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtSpeedMs;
        private System.Windows.Forms.TextBox txtSpeedKmh;
        private System.Windows.Forms.TextBox txtTime;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtAltitude;
        private System.Windows.Forms.TextBox txtLongitude;
        private System.Windows.Forms.TextBox txtLatitude;
        private System.Windows.Forms.Label lblAffAlt;
        private System.Windows.Forms.Label lblAffLong;
        private System.Windows.Forms.Label lblAffLat;
        private System.Windows.Forms.TextBox txtStatut;
        private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel pnlGpsInfo;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.SplitContainer splitContainer1;
    }
}
