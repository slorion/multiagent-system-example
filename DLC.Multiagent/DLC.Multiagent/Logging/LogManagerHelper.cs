using DLC.Framework;
using NLog;
using NLog.Targets;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DLC.Multiagent.Logging
{
	internal static class LogManagerHelper
	{
		public static string GetBrokerLog(bool archive)
		{
			// FileTarget does not offer any native method to manually archive, so we must implement that feature
			// if that becomes too fragile, a pull request should be considered
			// https://github.com/NLog/NLog/blob/master/src/NLog/Targets/FileTarget.cs

			var fileTarget = LogManager.Configuration.AllTargets.OfType<FileTarget>().FirstOrDefault();

			if (fileTarget != null)
			{
				var logPath = fileTarget.FileName.Render(new LogEventInfo());

				if (File.Exists(logPath))
				{
					if (archive)
					{
						var now = DateTimePrecise.Now;

						var newLogPath = Path.Combine(Path.GetDirectoryName(logPath), Path.GetFileNameWithoutExtension(logPath) + "-" + now.ToString("yyyy-MM-ddTHHmmssfff") + Path.GetExtension(logPath));

						var mi = fileTarget.GetType().GetMethod("InvalidateCacheItem", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						mi.Invoke(fileTarget, new object[] { logPath });

						File.Move(logPath, newLogPath);

						logPath = newLogPath;
					}

					return File.ReadAllText(logPath);
				}
			}

			return string.Empty;
		}
	}
}