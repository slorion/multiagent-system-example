using DLC.Framework.Reactive;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Threading;

namespace DLC.Multiagent.Logging
{
	internal static class BrokerLogDataSource
	{
		private static readonly DeferredSubject<BrokerLogEntry> _dataSource = new SubjectSlim<BrokerLogEntry>().ToDeferred();
		private static object _initialized;

		public static IObservable<BrokerLogEntry> DataSource { get { return _dataSource; } }

		public static void EnsureInitialize()
		{
			var initialized = Interlocked.CompareExchange(ref _initialized, new object(), null);

			if (initialized == null)
			{
				LogManager.ConfigurationChanged +=
					(ss, ee) =>
					{
						// NLog configuration internally reloads when LogManager.Configuration is re/assigned (bad design!)
						// and it seems to be the only way to propagate our changes:
						//   - LogManager.Configuration.Reload() reloads the original file configuration
						//   - LogManager.ReconfigExistingLoggers() does not process our new LogTarget
						//
						// When setting LogManager.Configuration, NLog raise the event ConfigurationChanged
						// so we must first check that our LogTarget is not already registered

						if (LogManager.Configuration != null && LogManager.Configuration.FindTargetByName("Multiagent-AgentLogDataSource") == null)
						{
							var logTarget = new MethodCallTarget();
							logTarget.ClassName = typeof(BrokerLogDataSource).AssemblyQualifiedName;
							logTarget.MethodName = "LogCallback";
							logTarget.Parameters.Add(new MethodCallParameter("${longdate}"));
							logTarget.Parameters.Add(new MethodCallParameter("${level}"));
							logTarget.Parameters.Add(new MethodCallParameter("${logger}"));
							logTarget.Parameters.Add(new MethodCallParameter("${event-context:item=agent-id}"));
							logTarget.Parameters.Add(new MethodCallParameter("${message}"));
							logTarget.Parameters.Add(new MethodCallParameter("${exception:format=tostring}"));

							LogManager.Configuration.AddTarget("Multiagent-AgentLogDataSource", logTarget);
							LogManager.Configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, logTarget));

							LogManager.Configuration = LogManager.Configuration;
						}
					};

				// force a configuration reload to register our LogTarget (see comment above)
				LogManager.Configuration = LogManager.Configuration;
			}
		}

		// The method must be public and static (https://github.com/nlog/nlog/wiki/MethodCall-target)
		public static void LogCallback(string timestamp, string level, string source, string agentId, string message, string exception)
		{
			_dataSource.OnNext(new BrokerLogEntry { Timestamp = DateTime.Parse(timestamp), Level = (BrokerLogLevel) Enum.Parse(typeof(BrokerLogLevel), level, true), Source = source, AgentId = agentId, Message = message, Exception = exception });
		}
	}
}