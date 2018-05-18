using DLC.Framework.UI.Forms;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DLC.Multiagent.DesktopApp.UI
{
	internal class ReactiveForm
		: StickyForm
	{
		private readonly List<IDisposable> _observers = new List<IDisposable>();

		public ReactiveForm()
			: base()
		{
			this.IsSticky = true;
			this.StickGap = 20;
			this.IsWindowPositionSavedOnClose = true;
			this.IsWindowSizeSavedOnClose = true;
		}

		protected void RegisterObserver(IDisposable observer)
		{
			if (observer == null) throw new ArgumentNullException("observer");

			_observers.Add(observer);
		}

		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			base.OnFormClosed(e);

			foreach (var observer in _observers)
				observer.Dispose();
			_observers.Clear();
		}
	}
}