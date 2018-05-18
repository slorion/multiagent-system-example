namespace DLC.Scientific.Acquisition.Agents.EventPanelAgent.UI
{
	partial class RoadEventGroupControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.picIcon = new System.Windows.Forms.PictureBox();
			this.picState = new System.Windows.Forms.PictureBox();
			this.ttpMain = new System.Windows.Forms.ToolTip(this.components);
			((System.ComponentModel.ISupportInitialize)(this.picIcon)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picState)).BeginInit();
			this.SuspendLayout();
			// 
			// picIcon
			// 
			this.picIcon.BackColor = System.Drawing.Color.Transparent;
			this.picIcon.Location = new System.Drawing.Point(0, 0);
			this.picIcon.Name = "picIcon";
			this.picIcon.Size = new System.Drawing.Size(64, 64);
			this.picIcon.TabIndex = 0;
			this.picIcon.TabStop = false;
			this.picIcon.Click += new System.EventHandler(this.RoadEventGroupControl_Click);
			// 
			// picState
			// 
			this.picState.BackColor = System.Drawing.Color.Transparent;
			this.picState.Location = new System.Drawing.Point(0, 70);
			this.picState.Name = "picState";
			this.picState.Size = new System.Drawing.Size(64, 33);
			this.picState.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.picState.TabIndex = 1;
			this.picState.TabStop = false;
			this.picState.Click += new System.EventHandler(this.RoadEventGroupControl_Click);
			// 
			// RoadEventGroupControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.picState);
			this.Controls.Add(this.picIcon);
			this.Name = "RoadEventGroupControl";
			this.Size = new System.Drawing.Size(65, 105);
			this.Click += new System.EventHandler(this.RoadEventGroupControl_Click);
			((System.ComponentModel.ISupportInitialize)(this.picIcon)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picState)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox picIcon;
		private System.Windows.Forms.PictureBox picState;
		private System.Windows.Forms.ToolTip ttpMain;
	}
}
