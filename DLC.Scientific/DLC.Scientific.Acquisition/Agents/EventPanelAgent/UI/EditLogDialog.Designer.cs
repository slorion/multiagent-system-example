namespace DLC.Scientific.Acquisition.Agents.EventPanelAgent.UI
{
	partial class EditLogDialog
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
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.lblComment = new System.Windows.Forms.Label();
			this.txtComment = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.udCorrection = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.udCorrection)).BeginInit();
			this.SuspendLayout();
			//
			// btnOK
			//
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(191, 180);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "&OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			//
			// btnCancel
			//
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(272, 180);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "&Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			//
			// lblComment
			//
			this.lblComment.AutoSize = true;
			this.lblComment.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblComment.ForeColor = System.Drawing.Color.Yellow;
			this.lblComment.Location = new System.Drawing.Point(13, 13);
			this.lblComment.Name = "lblComment";
			this.lblComment.Size = new System.Drawing.Size(79, 13);
			this.lblComment.TabIndex = 4;
			this.lblComment.Text = "Comment";
			//
			// txtComment
			//
			this.txtComment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtComment.Location = new System.Drawing.Point(137, 10);
			this.txtComment.Multiline = true;
			this.txtComment.Name = "txtComment";
			this.txtComment.Size = new System.Drawing.Size(210, 113);
			this.txtComment.TabIndex = 0;
			//
			// label1
			//
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.Color.Yellow;
			this.label1.Location = new System.Drawing.Point(12, 137);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(119, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "Manual correction";
			//
			// udCorrection
			//
			this.udCorrection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
			this.udCorrection.Location = new System.Drawing.Point(137, 135);
			this.udCorrection.Maximum = new decimal(new int[] {
            500000,
            0,
            0,
            0});
			this.udCorrection.Minimum = new decimal(new int[] {
            500000,
            0,
            0,
            -2147483648});
			this.udCorrection.Name = "udCorrection";
			this.udCorrection.Size = new System.Drawing.Size(160, 20);
			this.udCorrection.TabIndex = 1;
			this.udCorrection.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			//
			// label2
			//
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.ForeColor = System.Drawing.Color.Yellow;
			this.label2.Location = new System.Drawing.Point(303, 137);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(44, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "meters";
			//
			// EditLogDialog
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(359, 215);
			this.ControlBox = false;
			this.Controls.Add(this.label2);
			this.Controls.Add(this.udCorrection);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtComment);
			this.Controls.Add(this.lblComment);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "EditLogDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Event modification";
			((System.ComponentModel.ISupportInitialize)(this.udCorrection)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label lblComment;
		private System.Windows.Forms.TextBox txtComment;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown udCorrection;
		private System.Windows.Forms.Label label2;
	}
}