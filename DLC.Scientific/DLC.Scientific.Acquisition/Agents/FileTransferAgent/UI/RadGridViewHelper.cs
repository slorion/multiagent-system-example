using System;
using System.Collections.Generic;
using System.Data;
using Telerik.WinControls.UI;

namespace DLC.Scientific.Acquisition.Agents.FileTransferAgent.UI
{
	internal static class RadGridViewHelper
	{
		public static IEnumerable<DataRow> GetChildDataRows(GridViewGroupRowInfo groupRow)
		{
			if (groupRow == null) throw new ArgumentNullException("groupRow");

			foreach (var childRow in groupRow.ChildRows)
			{
				if (childRow is GridViewGroupRowInfo)
				{
					foreach (var sub in GetChildDataRows((GridViewGroupRowInfo) childRow))
						yield return sub;
				}
				else
				{
					var dataRowView = childRow.DataBoundItem as DataRowView;
					if (dataRowView != null)
						yield return dataRowView.Row;
				}
			}
		}

		public static IEnumerable<DataGroup> GetGroups(DataGroupCollection groups)
		{
			if (groups == null)
				yield break;

			foreach (DataGroup group in groups)
			{
				yield return group;

				foreach (DataGroup child in GetGroups(group.Groups))
					yield return child;
			}
		}
	}
}