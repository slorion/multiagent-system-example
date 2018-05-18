using QbservableProvider;
using System;

namespace DLC.Multiagent
{
	internal class RxService
	{
		public RxService(IObservable<TcpClientTermination> stateDataSource, int listenPort, IDisposable connection)
		{
			if (stateDataSource == null) throw new ArgumentNullException("stateDataSource");
			if (listenPort < 0 || listenPort > 65535) throw new ArgumentOutOfRangeException("listenPort");
			if (connection == null) throw new ArgumentNullException("connection");

			this.StateDataSource = stateDataSource;
			this.ListenPort = listenPort;
			this.Connection = connection;
		}

		public IObservable<TcpClientTermination> StateDataSource { get; private set; }
		public int ListenPort { get; private set; }
		public IDisposable Connection { get; private set; }
	}
}