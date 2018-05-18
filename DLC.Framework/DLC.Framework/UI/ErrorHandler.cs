using NLog;
using NLog.Fluent;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DLC.Framework.UI
{
	public static class ErrorHandler
	{
		public static async Task<bool> Try(Action action, string errorTitle, string errorMessage, bool canRetry = false)
		{
			if (action == null) throw new ArgumentNullException("action");

			Exception error;
			bool doRetry = false;

			do
			{
				try
				{
					action();
					error = null;
				}
				catch (Exception ex)
				{
					error = ex;
				}

				if (error != null)
				{
					doRetry = await LogAndShowError(error, errorTitle, errorMessage, canRetry).ConfigureAwait(false);
				}
			} while (error != null && doRetry);

			return error == null;
		}

		public static Task<bool> LogAndShowError(Exception ex, string errorTitle = null, string errorMessage = null, bool canRetry = false)
		{
			if (ex == null)
				ex = new ArgumentNullException("ex", "No exception was provided.");

			if (string.IsNullOrEmpty(errorTitle))
				errorTitle = "Unknown error";

			if (!string.IsNullOrEmpty(errorMessage))
				errorMessage += Environment.NewLine + Environment.NewLine;

			// MessageBox display must not be blocking
			return Task.Run(() => MessageBox.Show(errorMessage + ex.ToString(), errorTitle, canRetry ? MessageBoxButtons.RetryCancel : MessageBoxButtons.OK, MessageBoxIcon.Error))
				.ContinueWith(
					t =>
					{
						bool retry = t.IsCompleted && t.Result == DialogResult.Retry;
						Log.Level(retry ? LogLevel.Warn : LogLevel.Error).Message(errorMessage).Exception(ex).Write();
						return retry;
					});
		}
	}
}