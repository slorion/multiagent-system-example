namespace DLC.Scientific.Acquisition.Tools.ItineraryGenerator
{
	partial class MainUI
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
			this.btnGenerateItiFiles = new System.Windows.Forms.Button();
			this.pbGeneration = new System.Windows.Forms.ProgressBar();
			this.txtDestination = new System.Windows.Forms.TextBox();
			this.btnBrowseDestination = new System.Windows.Forms.Button();
			this.lblDestinationLabel = new System.Windows.Forms.Label();
			this.lstInputFiles = new System.Windows.Forms.ListBox();
			this.lblCurrentGeneratedFileLabel = new System.Windows.Forms.Label();
			this.lblCurrentGeneratedFile = new System.Windows.Forms.Label();
			this.lblInputFilesLabel = new System.Windows.Forms.Label();
			this.clstAllowedBGRDataTypes = new System.Windows.Forms.CheckedListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.rdoGeocodageDateNow = new System.Windows.Forms.RadioButton();
			this.rdoGeocodageDateGps = new System.Windows.Forms.RadioButton();
			this.lblGeocodageDateLabel = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			//
			// btnGenerateItiFiles
			//
			this.btnGenerateItiFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnGenerateItiFiles.Location = new System.Drawing.Point(475, 260);
			this.btnGenerateItiFiles.Name = "btnGenerateItiFiles";
			this.btnGenerateItiFiles.Size = new System.Drawing.Size(108, 23);
			this.btnGenerateItiFiles.TabIndex = 6;
			this.btnGenerateItiFiles.Text = "&Generate ITI files";
			this.btnGenerateItiFiles.UseVisualStyleBackColor = true;
			this.btnGenerateItiFiles.Click += new System.EventHandler(this.btnGenerateItiFiles_Click);
			//
			// pbGeneration
			//
			this.pbGeneration.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pbGeneration.Location = new System.Drawing.Point(12, 305);
			this.pbGeneration.Name = "pbGeneration";
			this.pbGeneration.Size = new System.Drawing.Size(571, 23);
			this.pbGeneration.TabIndex = 9;
			//
			// txtDestination
			//
			this.txtDestination.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtDestination.Location = new System.Drawing.Point(75, 233);
			this.txtDestination.Name = "txtDestination";
			this.txtDestination.Size = new System.Drawing.Size(473, 20);
			this.txtDestination.TabIndex = 4;
			//
			// btnBrowseDestination
			//
			this.btnBrowseDestination.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBrowseDestination.Location = new System.Drawing.Point(554, 231);
			this.btnBrowseDestination.Name = "btnBrowseDestination";
			this.btnBrowseDestination.Size = new System.Drawing.Size(29, 23);
			this.btnBrowseDestination.TabIndex = 5;
			this.btnBrowseDestination.Text = "...";
			this.btnBrowseDestination.UseVisualStyleBackColor = true;
			this.btnBrowseDestination.Click += new System.EventHandler(this.btnBrowseDestination_Click);
			//
			// lblDestinationLabel
			//
			this.lblDestinationLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblDestinationLabel.AutoSize = true;
			this.lblDestinationLabel.Location = new System.Drawing.Point(9, 236);
			this.lblDestinationLabel.Name = "lblDestinationLabel";
			this.lblDestinationLabel.Size = new System.Drawing.Size(60, 13);
			this.lblDestinationLabel.TabIndex = 3;
			this.lblDestinationLabel.Text = "Destination";
			//
			// lstInputFiles
			//
			this.lstInputFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lstInputFiles.FormattingEnabled = true;
			this.lstInputFiles.Location = new System.Drawing.Point(12, 79);
			this.lstInputFiles.Name = "lstInputFiles";
			this.lstInputFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.lstInputFiles.Size = new System.Drawing.Size(571, 134);
			this.lstInputFiles.TabIndex = 2;
			this.lstInputFiles.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lstInputFiles_KeyUp);
			//
			// lblCurrentGeneratedFileLabel
			//
			this.lblCurrentGeneratedFileLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblCurrentGeneratedFileLabel.AutoSize = true;
			this.lblCurrentGeneratedFileLabel.Location = new System.Drawing.Point(9, 289);
			this.lblCurrentGeneratedFileLabel.Name = "lblCurrentGeneratedFileLabel";
			this.lblCurrentGeneratedFileLabel.Size = new System.Drawing.Size(49, 13);
			this.lblCurrentGeneratedFileLabel.TabIndex = 7;
			this.lblCurrentGeneratedFileLabel.Text = "Processing";
			//
			// lblCurrentGeneratedFile
			//
			this.lblCurrentGeneratedFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblCurrentGeneratedFile.AutoSize = true;
			this.lblCurrentGeneratedFile.Location = new System.Drawing.Point(70, 289);
			this.lblCurrentGeneratedFile.Name = "lblCurrentGeneratedFile";
			this.lblCurrentGeneratedFile.Size = new System.Drawing.Size(24, 13);
			this.lblCurrentGeneratedFile.TabIndex = 8;
			this.lblCurrentGeneratedFile.Text = "n/a";
			//
			// lblInputFilesLabel
			//
			this.lblInputFilesLabel.AutoSize = true;
			this.lblInputFilesLabel.Location = new System.Drawing.Point(9, 63);
			this.lblInputFilesLabel.Name = "lblInputFilesLabel";
			this.lblInputFilesLabel.Size = new System.Drawing.Size(103, 13);
			this.lblInputFilesLabel.TabIndex = 1;
			this.lblInputFilesLabel.Text = "GPS traces to process";
			//
			// clstAllowedBGRDataTypes
			//
			this.clstAllowedBGRDataTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
			this.clstAllowedBGRDataTypes.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.clstAllowedBGRDataTypes.CheckOnClick = true;
			this.clstAllowedBGRDataTypes.FormattingEnabled = true;
			this.clstAllowedBGRDataTypes.Location = new System.Drawing.Point(102, 7);
			this.clstAllowedBGRDataTypes.MultiColumn = true;
			this.clstAllowedBGRDataTypes.Name = "clstAllowedBGRDataTypes";
			this.clstAllowedBGRDataTypes.Size = new System.Drawing.Size(478, 15);
			this.clstAllowedBGRDataTypes.TabIndex = 0;
			//
			// label1
			//
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 7);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(39, 13);
			this.label1.TabIndex = 10;
			this.label1.Text = "Inclure";
			//
			// rdoGeocodageDateNow
			//
			this.rdoGeocodageDateNow.AutoSize = true;
			this.rdoGeocodageDateNow.Checked = true;
			this.rdoGeocodageDateNow.Location = new System.Drawing.Point(0, 4);
			this.rdoGeocodageDateNow.Name = "rdoGeocodageDateNow";
			this.rdoGeocodageDateNow.Size = new System.Drawing.Size(77, 17);
			this.rdoGeocodageDateNow.TabIndex = 11;
			this.rdoGeocodageDateNow.TabStop = true;
			this.rdoGeocodageDateNow.Text = "Today";
			this.rdoGeocodageDateNow.UseVisualStyleBackColor = true;
			//
			// rdoGeocodageDateGps
			//
			this.rdoGeocodageDateGps.AutoSize = true;
			this.rdoGeocodageDateGps.Location = new System.Drawing.Point(83, 4);
			this.rdoGeocodageDateGps.Name = "rdoGeocodageDateGps";
			this.rdoGeocodageDateGps.Size = new System.Drawing.Size(82, 17);
			this.rdoGeocodageDateGps.TabIndex = 12;
			this.rdoGeocodageDateGps.TabStop = true;
			this.rdoGeocodageDateGps.Text = "GPS time";
			this.rdoGeocodageDateGps.UseVisualStyleBackColor = true;
			//
			// lblGeocodageDateLabel
			//
			this.lblGeocodageDateLabel.AutoSize = true;
			this.lblGeocodageDateLabel.Location = new System.Drawing.Point(9, 30);
			this.lblGeocodageDateLabel.Name = "lblGeocodageDateLabel";
			this.lblGeocodageDateLabel.Size = new System.Drawing.Size(87, 13);
			this.lblGeocodageDateLabel.TabIndex = 13;
			this.lblGeocodageDateLabel.Text = "Geocoding date";
			//
			// panel1
			//
			this.panel1.Controls.Add(this.rdoGeocodageDateNow);
			this.panel1.Controls.Add(this.rdoGeocodageDateGps);
			this.panel1.Location = new System.Drawing.Point(102, 24);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(481, 24);
			this.panel1.TabIndex = 14;
			//
			// MainUI
			//
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(592, 340);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.lblGeocodageDateLabel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.clstAllowedBGRDataTypes);
			this.Controls.Add(this.lblInputFilesLabel);
			this.Controls.Add(this.lblCurrentGeneratedFile);
			this.Controls.Add(this.lblCurrentGeneratedFileLabel);
			this.Controls.Add(this.lstInputFiles);
			this.Controls.Add(this.lblDestinationLabel);
			this.Controls.Add(this.btnBrowseDestination);
			this.Controls.Add(this.txtDestination);
			this.Controls.Add(this.pbGeneration);
			this.Controls.Add(this.btnGenerateItiFiles);
			this.Name = "MainUI";
			this.Text = "ITI files generator";
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainUI_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainUI_DragEnter);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnGenerateItiFiles;
		private System.Windows.Forms.ProgressBar pbGeneration;
		private System.Windows.Forms.TextBox txtDestination;
		private System.Windows.Forms.Button btnBrowseDestination;
		private System.Windows.Forms.Label lblDestinationLabel;
		private System.Windows.Forms.ListBox lstInputFiles;
		private System.Windows.Forms.Label lblCurrentGeneratedFileLabel;
		private System.Windows.Forms.Label lblCurrentGeneratedFile;
		private System.Windows.Forms.Label lblInputFilesLabel;
		private System.Windows.Forms.CheckedListBox clstAllowedBGRDataTypes;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton rdoGeocodageDateNow;
		private System.Windows.Forms.RadioButton rdoGeocodageDateGps;
		private System.Windows.Forms.Label lblGeocodageDateLabel;
		private System.Windows.Forms.Panel panel1;
	}
}

