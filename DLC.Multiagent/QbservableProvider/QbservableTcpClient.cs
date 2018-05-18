using System;
using System.Net;
using System.Reactive.Linq;
using System.Runtime.Remoting.Messaging;

namespace QbservableProvider
{
	public sealed class QbservableTcpClient<TSource>
	{
		private readonly IPEndPoint endPoint;
		private readonly IRemotingFormatter formatter;
		private readonly LocalEvaluator localEvaluator;

		public QbservableTcpClient(IPAddress address, int port)
			: this(new IPEndPoint(address, port))
		{
		}

		public QbservableTcpClient(IPAddress address, int port, params Type[] knownTypes)
			: this(new IPEndPoint(address, port), knownTypes)
		{
		}

		public QbservableTcpClient(IPAddress address, int port, LocalEvaluator localEvaluator)
			: this(new IPEndPoint(address, port), localEvaluator)
		{
		}

		public QbservableTcpClient(IPAddress address, int port, IRemotingFormatter formatter)
			: this(new IPEndPoint(address, port), formatter)
		{
		}

		public QbservableTcpClient(IPAddress address, int port, IRemotingFormatter formatter, LocalEvaluator localEvaluator)
			: this(new IPEndPoint(address, port), formatter, localEvaluator)
		{
		}

		public QbservableTcpClient(IPAddress address, int port, IRemotingFormatter formatter, params Type[] knownTypes)
			: this(new IPEndPoint(address, port), formatter, knownTypes)
		{
		}

		public QbservableTcpClient(IPEndPoint endPoint)
			: this(endPoint, QbservableTcpServer.CreateDefaultFormatter())
		{
		}

		public QbservableTcpClient(IPEndPoint endPoint, params Type[] knownTypes)
			: this(endPoint, QbservableTcpServer.CreateDefaultFormatter(), knownTypes)
		{
		}

		public QbservableTcpClient(IPEndPoint endPoint, LocalEvaluator localEvaluator)
			: this(endPoint, QbservableTcpServer.CreateDefaultFormatter(), localEvaluator)
		{
		}

		public QbservableTcpClient(IPEndPoint endPoint, IRemotingFormatter formatter)
			: this(endPoint, formatter, new ImmediateLocalEvaluator())
		{
		}

		public QbservableTcpClient(IPEndPoint endPoint, IRemotingFormatter formatter, params Type[] knownTypes)
			: this(endPoint, formatter, new ImmediateLocalEvaluator(knownTypes))
		{
		}

		public QbservableTcpClient(IPEndPoint endPoint, IRemotingFormatter formatter, LocalEvaluator localEvaluator)
		{
			this.endPoint = endPoint;
			this.formatter = formatter;
			this.localEvaluator = localEvaluator;
		}

		public IQbservable<TSource> Query()
		{
			return new TcpClientQbservableProvider(typeof(TSource), endPoint, formatter, localEvaluator).CreateQuery<TSource>();
		}

		public IQbservable<TSource> Query(object argument)
		{
			return new TcpClientQbservableProvider(typeof(TSource), endPoint, formatter, localEvaluator, argument).CreateQuery<TSource>();
		}
	}
}