using System.Drawing;
using System.Windows.Forms;

namespace DLC.Scientific.Acquisition.Agents.BgrDirectionalAgent.UI
{
	internal class HiddenSelectionCheckedListBox
		: CheckedListBox
	{
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			// hack to remove selection rectangle

			var state = e.State;
			state &= ~DrawItemState.Focus;
			state &= ~DrawItemState.Selected;
			var e2 = new DrawItemEventArgs(e.Graphics, e.Font, e.Bounds, e.Index, state, Color.Yellow, Color.Black);

			base.OnDrawItem(e2);
		}
	}
}