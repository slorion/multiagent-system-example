namespace DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI
{
	partial class PersonNameInputBox
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
			this.lblPrompt = new System.Windows.Forms.Label();
			this.lblFirstName = new System.Windows.Forms.Label();
			this.txtFirstName = new System.Windows.Forms.TextBox();
			this.txtLastName = new System.Windows.Forms.TextBox();
			this.lblLastName = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// lblPrompt
			// 
			this.lblPrompt.AutoSize = true;
			this.lblPrompt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblPrompt.ForeColor = System.Drawing.Color.Yellow;
			this.lblPrompt.Location = new System.Drawing.Point(12, 9);
			this.lblPrompt.Name = "lblPrompt";
			this.lblPrompt.Size = new System.Drawing.Size(352, 16);
			this.lblPrompt.TabIndex = 0;
			this.lblPrompt.Text = "Veuillez entrer le prénom et le nom du conducteur:";
			// 
			// lblFirstName
			// 
			this.lblFirstName.AutoSize = true;
			this.lblFirstName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblFirstName.ForeColor = System.Drawing.Color.Yellow;
			this.lblFirstName.Location = new System.Drawing.Point(12, 41);
			this.lblFirstName.Name = "lblFirstName";
			this.lblFirstName.Size = new System.Drawing.Size(65, 16);
			this.lblFirstName.TabIndex = 1;
			this.lblFirstName.Text = "Prénom:";
			// 
			// txtFirstName
			// 
			this.txtFirstName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtFirstName.Location = new System.Drawing.Point(83, 38);
			this.txtFirstName.Name = "txtFirstName";
			this.txtFirstName.Size = new System.Drawing.Size(350, 20);
			this.txtFirstName.TabIndex = 2;
			// 
			// txtLastName
			// 
			this.txtLastName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtLastName.Location = new System.Drawing.Point(83, 65);
			this.txtLastName.Name = "txtLastName";
			this.txtLastName.Size = new System.Drawing.Size(350, 20);
			this.txtLastName.TabIndex = 3;
			// 
			// lblLastName
			// 
			this.lblLastName.AutoSize = true;
			this.lblLastName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblLastName.ForeColor = System.Drawing.Color.Yellow;
			this.lblLastName.Location = new System.Drawing.Point(12, 68);
			this.lblLastName.Name = "lblLastName";
			this.lblLastName.Size = new System.Drawing.Size(44, 16);
			this.lblLastName.TabIndex = 4;
			this.lblLastName.Text = "Nom:";
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.CausesValidation = false;
			this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.button1.Location = new System.Drawing.Point(358, 91);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 26);
			this.button1.TabIndex = 5;
			this.button1.Text = "&Cancel";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// button2
			// 
			this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button2.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.button2.Location = new System.Drawing.Point(277, 91);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 26);
			this.button2.TabIndex = 6;
			this.button2.Text = "&OK";
			this.button2.UseVisualStyleBackColor = true;
			// 
			// errorProvider
			// 
			this.errorProvider.ContainerControl = this;
			// 
			// PersonNameInputBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(456, 124);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.lblLastName);
			this.Controls.Add(this.txtLastName);
			this.Controls.Add(this.txtFirstName);
			this.Controls.Add(this.lblFirstName);
			this.Controls.Add(this.lblPrompt);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PersonNameInputBox";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Nouveau conducteur";
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblPrompt;
		private System.Windows.Forms.Label lblFirstName;
		private System.Windows.Forms.TextBox txtFirstName;
		private System.Windows.Forms.TextBox txtLastName;
		private System.Windows.Forms.Label lblLastName;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.ErrorProvider errorProvider;
	}
}