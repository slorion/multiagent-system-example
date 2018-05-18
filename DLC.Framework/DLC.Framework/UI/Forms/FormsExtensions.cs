using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace DLC.Framework.UI.Forms
{
	public static class FormsExtensions
	{
		/// <summary>
		/// The DesignMode property does not correctly tell you if you are in design mode.
		/// InVSDesigner is a corrected version of that property.
		/// See https://connect.microsoft.com/VisualStudio/feedback/details/553305
		/// and http://stackoverflow.com/a/2693338/238419
		/// </summary>
		public static bool InVSDesigner(this Control ctrl)
		{
			if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
				return true;

			while (ctrl != null)
			{
				if (ctrl.Site != null && ctrl.Site.DesignMode)
					return true;

				ctrl = ctrl.Parent;
			}

			return false;
		}
	}
}