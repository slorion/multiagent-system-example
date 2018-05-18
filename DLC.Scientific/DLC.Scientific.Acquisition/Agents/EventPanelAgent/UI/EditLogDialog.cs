using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using System;
using System.Windows.Forms;

namespace DLC.Scientific.Acquisition.Agents.EventPanelAgent.UI
{
	public partial class EditLogDialog
		: Form
	{
		private readonly RoadEventData _data;

		public EditLogDialog(RoadEventData data)
		{
			if (data == null) throw new ArgumentNullException("data");

			InitializeComponent();

			_data = data;
			txtComment.Text = data.Comment;
			udCorrection.Value = Convert.ToDecimal(data.DistanceManualCorrection);
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			_data.Comment = txtComment.Text;
			_data.DistanceManualCorrection = Convert.ToDouble(udCorrection.Value);
		}
	}
}