using System.Windows.Forms;

namespace DLC.Scientific.Acquisition.Agents.AcquisitionManagerAgent.UI
{
	public partial class PersonNameInputBox
		: Form
	{
		public PersonNameInputBox(string title, string prompt)
		{
			InitializeComponent();

			this.Text = title;
			lblPrompt.Text = prompt;
		}

		public string FirstName { get { return txtFirstName.Text; } }
		public string LastName { get { return txtLastName.Text; } }

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			base.OnFormClosing(e);

			if (e.CloseReason == CloseReason.UserClosing)
			{
				txtFirstName.Text = txtFirstName.Text.Trim();
				txtLastName.Text = txtLastName.Text.Trim();

				if (this.DialogResult == DialogResult.OK)
				{
					errorProvider.SetError(txtFirstName, "");
					errorProvider.SetError(txtLastName, "");

					if (string.IsNullOrEmpty(txtFirstName.Text))
					{
						errorProvider.SetError(txtFirstName, "First name is mandatory.");
						e.Cancel = true;
					}

					if (string.IsNullOrEmpty(txtLastName.Text))
					{
						errorProvider.SetError(txtLastName, "Last name is mandatory.");
						e.Cancel = true;
					}
				}
			}
		}
	}
}