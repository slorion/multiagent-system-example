using CodeBits;
using DLC.Scientific.Acquisition.Agents.FileTransferAgent.Properties;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace DLC.Scientific.Acquisition.Agents.FileTransferAgent.UI
{
	internal class CustomGroupCellElement
		: GridGroupContentCellElement
	{
		private readonly Color _defaultProgressBarColor = Color.FromArgb(255, 51, 153, 255);

		private StackLayoutElement _stack;
		private LightVisualElement _headerElement;
		private RadProgressBarElement _progressElement;
		private LightVisualElement _hasErrorsElement;

		public CustomGroupCellElement(GridViewColumn column, GridRowElement row)
			: base(column, row)
		{
		}

		protected override Type ThemeEffectiveType { get { return typeof(GridGroupContentCellElement); } }

		protected override void CreateChildElements()
		{
			base.CreateChildElements();

			_stack = new StackLayoutElement { Orientation = Orientation.Horizontal, AutoSize = false, Margin = new Padding(2) };
			_headerElement = new LightVisualElement { AutoSize = false, Margin = default(Padding) };
			_hasErrorsElement = new LightVisualElement { AutoSize = false, Margin = default(Padding) };

			_progressElement = new RadProgressBarElement {
				AutoSize = false,
				Margin = new Padding(0, 3, 0, 0),
				SmoothingMode = SmoothingMode.AntiAlias
			};

			_stack.Children.Add(_headerElement);
			_stack.Children.Add(_progressElement);
			_stack.Children.Add(_hasErrorsElement);

			this.Children.Add(_stack);
		}

		public override void Initialize(GridViewColumn column, GridRowElement row)
		{
			base.Initialize(column, row);

			var height = this.TableElement.GroupHeaderHeight + 8;

			_headerElement.Size = new Size(this.ViewTemplate.Columns["destinationFolder"].Width, height);
			_progressElement.Size = new Size(this.ViewTemplate.Columns["progressBar"].Width - 6, height - 6);
			_hasErrorsElement.Size = new Size(this.ViewTemplate.Columns["hasErrors"].Width, height);
			_stack.Size = new Size(_headerElement.Size.Width + _progressElement.Size.Width + _hasErrorsElement.Size.Width, height);

			_stack.Margin = new Padding(Math.Max(0, 1 - row.RowInfo.HierarchyLevel) * this.TableElement.GroupIndent, 0, 0, 0);

			row.GridControl.Resize +=
				(s, e) =>
				{
					_headerElement.Size = new Size(this.ViewTemplate.Columns["destinationFolder"].Width, height);
					_progressElement.Size = new Size(this.ViewTemplate.Columns["progressBar"].Width - 6, height - 6);
					_hasErrorsElement.Size = new Size(this.ViewTemplate.Columns["hasErrors"].Width, height);
					_stack.Size = new Size(_headerElement.Size.Width + _progressElement.Size.Width + _hasErrorsElement.Size.Width, height);
				};

			_progressElement.Visibility = row.GridControl.AutoExpandGroups ? ElementVisibility.Hidden : ElementVisibility.Visible;
			_hasErrorsElement.Visibility = row.GridControl.AutoExpandGroups ? ElementVisibility.Hidden : ElementVisibility.Visible;

			row.GridControl.GroupExpanded +=
				(s, e) =>
				{
					var isExpanded = this.RowElement.GridControl != null && RadGridViewHelper.GetGroups(this.RowElement.GridControl.Groups).All(g => g.IsExpanded);

					_progressElement.Visibility = isExpanded ? ElementVisibility.Hidden : ElementVisibility.Visible;
					_hasErrorsElement.Visibility = isExpanded ? ElementVisibility.Hidden : ElementVisibility.Visible;
				};


			// force grid to honor its AutoExpandGroups property when a group is initialized
			if (row.GridControl.AutoExpandGroups)
				row.Data.Group.Expand();
			else
				row.Data.Group.Collapse();
		}

		public override void SetContent()
		{
			base.SetContent();

			var gridRow = this.RowInfo as GridViewGroupRowInfo;
			if (gridRow == null)
				return;

			int remainingFileCount = 0;
			long copiedBytes = 0;
			long totalBytes = 0;
			bool hasErrors = false;

			foreach (var dataRow in RadGridViewHelper.GetChildDataRows(gridRow))
			{
				remainingFileCount = Math.Max(remainingFileCount, dataRow.Field<int>("remainingFileCount"));
				copiedBytes += dataRow.Field<long>("copiedBytes");
				totalBytes += dataRow.Field<long>("totalBytes");
				hasErrors |= dataRow.Field<bool>("hasErrors");
			}

			_headerElement.Text = gridRow.HeaderText;
			_hasErrorsElement.Image = hasErrors ? Resources.ExclamationRed : Resources.CheckedGreen;

			_progressElement.Value1 = totalBytes <= 0 ? 0 : Convert.ToInt32((double) copiedBytes / (double) totalBytes * 100);
			_progressElement.Text = string.Format("{0} remaining ({1}/{2})", remainingFileCount, ByteSizeFriendlyName.Build(copiedBytes), ByteSizeFriendlyName.Build(totalBytes));
			_progressElement.IndicatorElement1.BackColor = (hasErrors || _progressElement.Value1 < 100) ? _defaultProgressBarColor : Color.LimeGreen;

			this.Text = string.Empty;
		}
	}
}