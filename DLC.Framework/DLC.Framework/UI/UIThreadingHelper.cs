using System;
using System.Threading;
using System.Windows.Forms;

namespace DLC.Framework.UI
{
	public static class UIThreadingHelper
	{
		public static void DispatchUI(Action action, bool asynchronous = true)
		{
			DispatchUI(action, SynchronizationContext.Current);
		}

		public static void DispatchUI(Action action, SynchronizationContext context, bool asynchronous = true)
		{
			if (action == null) throw new ArgumentNullException("action");
			if (context == null) throw new ArgumentNullException("context");

			if (asynchronous)
				context.Post(_ => action(), null);
			else
				context.Send(_ => action(), null);
		}

		public static void RunInNewUIThread(Action action, ApartmentState apartmentState = ApartmentState.STA)
		{
			if (action == null) throw new ArgumentNullException("action");

			var uiThread = new Thread(
				() =>
				{
					Application.ThreadException += (s, e) => ErrorHandler.LogAndShowError(e.Exception);
					action();
				});
			uiThread.SetApartmentState(apartmentState);

			uiThread.Start();
		}
	}
}