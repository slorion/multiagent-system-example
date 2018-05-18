namespace DLC.Scientific.Acquisition.Agents.DistanceAgent.UI
{
	partial class CalibrationUI
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CalibrationUI));
			this.btnStart = new System.Windows.Forms.Button();
			this.btnStop = new System.Windows.Forms.Button();
			this.txtOldCalibrationLeft = new System.Windows.Forms.TextBox();
			this.lblOldCalibrationLeft = new System.Windows.Forms.Label();
			this.txtNewCalibrationLeft = new System.Windows.Forms.TextBox();
			this.btnValidate = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.timerUpdateView = new System.Windows.Forms.Timer(this.components);
			this.lblOldPulsePerKm = new System.Windows.Forms.Label();
			this.gbOldCalibration = new System.Windows.Forms.GroupBox();
			this.txtOldCalibrationRight = new System.Windows.Forms.TextBox();
			this.lblOldCalibrationRight = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.gbNewCalibration = new System.Windows.Forms.GroupBox();
			this.txtNewCalibrationRight = new System.Windows.Forms.TextBox();
			this.lblNewCalibrationRight = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.lblNewCalibrationLeft = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.lblDistance = new System.Windows.Forms.Label();
			this.txtDistance = new System.Windows.Forms.TextBox();
			this.pnlGeneralInfos.SuspendLayout();
			this.gbOldCalibration.SuspendLayout();
			this.gbNewCalibration.SuspendLayout();
			this.SuspendLayout();
			//
			// pnlGeneralInfos
			//
			this.pnlGeneralInfos.Controls.Add(this.lblDistance);
			this.pnlGeneralInfos.Controls.Add(this.txtDistance);
			this.pnlGeneralInfos.Controls.Add(this.btnStop);
			this.pnlGeneralInfos.Controls.Add(this.btnStart);
			this.pnlGeneralInfos.Controls.Add(this.gbOldCalibration);
			this.pnlGeneralInfos.Controls.Add(this.btnCancel);
			this.pnlGeneralInfos.Controls.Add(this.btnValidate);
			this.pnlGeneralInfos.Controls.Add(this.gbNewCalibration);
			this.pnlGeneralInfos.Size = new System.Drawing.Size(318, 492);
			this.pnlGeneralInfos.Controls.SetChildIndex(this.gbNewCalibration, 0);
			this.pnlGeneralInfos.Controls.SetChildIndex(this.btnValidate, 0);
			this.pnlGeneralInfos.Controls.SetChildIndex(this.btnCancel, 0);
			this.pnlGeneralInfos.Controls.SetChildIndex(this.gbOldCalibration, 0);
			this.pnlGeneralInfos.Controls.SetChildIndex(this.btnStart, 0);
			this.pnlGeneralInfos.Controls.SetChildIndex(this.btnStop, 0);
			this.pnlGeneralInfos.Controls.SetChildIndex(this.txtDistance, 0);
			this.pnlGeneralInfos.Controls.SetChildIndex(this.lblDistance, 0);
			//
			// btnStart
			//
			this.btnStart.Location = new System.Drawing.Point(14, 217);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(123, 23);
			this.btnStart.TabIndex = 0;
			this.btnStart.Text = "Start calibration";
			this.btnStart.UseVisualStyleBackColor = true;
			this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
			//
			// btnStop
			//
			this.btnStop.Location = new System.Drawing.Point(181, 217);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(121, 23);
			this.btnStop.TabIndex = 1;
			this.btnStop.Text = "Stop calibration";
			this.btnStop.UseVisualStyleBackColor = true;
			this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
			//
			// txtOldCalibrationLeft
			//
			this.txtOldCalibrationLeft.Location = new System.Drawing.Point(121, 21);
			this.txtOldCalibrationLeft.Name = "txtOldCalibrationLeft";
			this.txtOldCalibrationLeft.ReadOnly = true;
			this.txtOldCalibrationLeft.Size = new System.Drawing.Size(95, 20);
			this.txtOldCalibrationLeft.TabIndex = 2;
			//
			// lblOldCalibrationLeft
			//
			this.lblOldCalibrationLeft.AutoSize = true;
			this.lblOldCalibrationLeft.Location = new System.Drawing.Point(22, 24);
			this.lblOldCalibrationLeft.Name = "lblOldCalibrationLeft";
			this.lblOldCalibrationLeft.Size = new System.Drawing.Size(92, 13);
			this.lblOldCalibrationLeft.TabIndex = 3;
			this.lblOldCalibrationLeft.Text = "Encodeur gauche";
			//
			// txtNewCalibrationLeft
			//
			this.txtNewCalibrationLeft.Location = new System.Drawing.Point(121, 22);
			this.txtNewCalibrationLeft.Name = "txtNewCalibrationLeft";
			this.txtNewCalibrationLeft.ReadOnly = true;
			this.txtNewCalibrationLeft.Size = new System.Drawing.Size(95, 20);
			this.txtNewCalibrationLeft.TabIndex = 4;
			//
			// btnValidate
			//
			this.btnValidate.Location = new System.Drawing.Point(14, 253);
			this.btnValidate.Name = "btnValidate";
			this.btnValidate.Size = new System.Drawing.Size(123, 23);
			this.btnValidate.TabIndex = 6;
			this.btnValidate.Text = "Apply calibration";
			this.btnValidate.UseVisualStyleBackColor = true;
			this.btnValidate.Click += new System.EventHandler(this.btnValidate_Click);
			//
			// btnCancel
			//
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(181, 253);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(121, 23);
			this.btnCancel.TabIndex = 7;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			//
			// lblOldPulsePerKm
			//
			this.lblOldPulsePerKm.AutoSize = true;
			this.lblOldPulsePerKm.Location = new System.Drawing.Point(222, 24);
			this.lblOldPulsePerKm.Name = "lblOldPulsePerKm";
			this.lblOldPulsePerKm.Size = new System.Drawing.Size(57, 13);
			this.lblOldPulsePerKm.TabIndex = 9;
			this.lblOldPulsePerKm.Text = "pulse / km";
			//
			// gbOldCalibration
			//
			this.gbOldCalibration.Controls.Add(this.txtOldCalibrationRight);
			this.gbOldCalibration.Controls.Add(this.lblOldCalibrationRight);
			this.gbOldCalibration.Controls.Add(this.label2);
			this.gbOldCalibration.Controls.Add(this.txtOldCalibrationLeft);
			this.gbOldCalibration.Controls.Add(this.lblOldCalibrationLeft);
			this.gbOldCalibration.Controls.Add(this.lblOldPulsePerKm);
			this.gbOldCalibration.ForeColor = System.Drawing.Color.White;
			this.gbOldCalibration.Location = new System.Drawing.Point(14, 41);
			this.gbOldCalibration.Name = "gbOldCalibration";
			this.gbOldCalibration.Size = new System.Drawing.Size(297, 82);
			this.gbOldCalibration.TabIndex = 13;
			this.gbOldCalibration.TabStop = false;
			this.gbOldCalibration.Text = "Current calibration";
			//
			// txtOldCalibrationRight
			//
			this.txtOldCalibrationRight.Location = new System.Drawing.Point(121, 50);
			this.txtOldCalibrationRight.Name = "txtOldCalibrationRight";
			this.txtOldCalibrationRight.ReadOnly = true;
			this.txtOldCalibrationRight.Size = new System.Drawing.Size(95, 20);
			this.txtOldCalibrationRight.TabIndex = 10;
			//
			// lblOldCalibrationRight
			//
			this.lblOldCalibrationRight.AutoSize = true;
			this.lblOldCalibrationRight.Location = new System.Drawing.Point(22, 53);
			this.lblOldCalibrationRight.Name = "lblOldCalibrationRight";
			this.lblOldCalibrationRight.Size = new System.Drawing.Size(76, 13);
			this.lblOldCalibrationRight.TabIndex = 11;
			this.lblOldCalibrationRight.Text = "Right encoder";
			//
			// label2
			//
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(222, 53);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(57, 13);
			this.label2.TabIndex = 12;
			this.label2.Text = "pulse / km";
			//
			// gbNewCalibration
			//
			this.gbNewCalibration.Controls.Add(this.txtNewCalibrationRight);
			this.gbNewCalibration.Controls.Add(this.lblNewCalibrationRight);
			this.gbNewCalibration.Controls.Add(this.label3);
			this.gbNewCalibration.Controls.Add(this.lblNewCalibrationLeft);
			this.gbNewCalibration.Controls.Add(this.label5);
			this.gbNewCalibration.Controls.Add(this.txtNewCalibrationLeft);
			this.gbNewCalibration.ForeColor = System.Drawing.Color.White;
			this.gbNewCalibration.Location = new System.Drawing.Point(14, 129);
			this.gbNewCalibration.Name = "gbNewCalibration";
			this.gbNewCalibration.Size = new System.Drawing.Size(297, 82);
			this.gbNewCalibration.TabIndex = 14;
			this.gbNewCalibration.TabStop = false;
			this.gbNewCalibration.Text = "New calibration";
			//
			// txtNewCalibrationRight
			//
			this.txtNewCalibrationRight.Location = new System.Drawing.Point(121, 50);
			this.txtNewCalibrationRight.Name = "txtNewCalibrationRight";
			this.txtNewCalibrationRight.ReadOnly = true;
			this.txtNewCalibrationRight.Size = new System.Drawing.Size(95, 20);
			this.txtNewCalibrationRight.TabIndex = 10;
			//
			// lblNewCalibrationRight
			//
			this.lblNewCalibrationRight.AutoSize = true;
			this.lblNewCalibrationRight.Location = new System.Drawing.Point(22, 53);
			this.lblNewCalibrationRight.Name = "lblNewCalibrationRight";
			this.lblNewCalibrationRight.Size = new System.Drawing.Size(76, 13);
			this.lblNewCalibrationRight.TabIndex = 11;
			this.lblNewCalibrationRight.Text = "Right encoder";
			//
			// label3
			//
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(222, 53);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(57, 13);
			this.label3.TabIndex = 12;
			this.label3.Text = "pulse / km";
			//
			// lblNewCalibrationLeft
			//
			this.lblNewCalibrationLeft.AutoSize = true;
			this.lblNewCalibrationLeft.Location = new System.Drawing.Point(22, 25);
			this.lblNewCalibrationLeft.Name = "lblNewCalibrationLeft";
			this.lblNewCalibrationLeft.Size = new System.Drawing.Size(92, 13);
			this.lblNewCalibrationLeft.TabIndex = 3;
			this.lblNewCalibrationLeft.Text = "Left encoder";
			//
			// label5
			//
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(222, 25);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(57, 13);
			this.label5.TabIndex = 9;
			this.label5.Text = "pulse / km";
			//
			// lblDistance
			//
			this.lblDistance.AutoSize = true;
			this.lblDistance.ForeColor = System.Drawing.Color.White;
			this.lblDistance.Location = new System.Drawing.Point(14, 18);
			this.lblDistance.Name = "lblDistance";
			this.lblDistance.Size = new System.Drawing.Size(129, 13);
			this.lblDistance.TabIndex = 16;
			this.lblDistance.Text = "Total travelled distance";
			//
			// txtDistance
			//
			this.txtDistance.Location = new System.Drawing.Point(145, 15);
			this.txtDistance.Name = "txtDistance";
			this.txtDistance.ReadOnly = true;
			this.txtDistance.Size = new System.Drawing.Size(95, 20);
			this.txtDistance.TabIndex = 15;
			//
			// CalibrationUI
			//
			this.AcceptButton = this.btnValidate;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(318, 492);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Location = new System.Drawing.Point(0, 0);
			this.MaximizeBox = false;
			this.Name = "CalibrationUI";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "«";
			this.pnlGeneralInfos.ResumeLayout(false);
			this.pnlGeneralInfos.PerformLayout();
			this.gbOldCalibration.ResumeLayout(false);
			this.gbOldCalibration.PerformLayout();
			this.gbNewCalibration.ResumeLayout(false);
			this.gbNewCalibration.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.TextBox txtOldCalibrationLeft;
		private System.Windows.Forms.Label lblOldCalibrationLeft;
		private System.Windows.Forms.TextBox txtNewCalibrationLeft;
		private System.Windows.Forms.Button btnValidate;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Timer timerUpdateView;
		private System.Windows.Forms.Label lblOldPulsePerKm;
		private System.Windows.Forms.GroupBox gbOldCalibration;
		private System.Windows.Forms.TextBox txtOldCalibrationRight;
		private System.Windows.Forms.Label lblOldCalibrationRight;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox gbNewCalibration;
		private System.Windows.Forms.TextBox txtNewCalibrationRight;
		private System.Windows.Forms.Label lblNewCalibrationRight;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label lblNewCalibrationLeft;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label lblDistance;
		private System.Windows.Forms.TextBox txtDistance;
	}
}