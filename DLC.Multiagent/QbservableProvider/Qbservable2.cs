using System;
using System.Net;
using System.Reactive.Linq;
using System.Runtime.Remoting.Messaging;

namespace QbservableProvider
{
	// TODO: Specify code contracts - applies to every type throughout this project, not just Qbservable2

	public static class Qbservable2
	{
		public static IObservable<TcpClientTermination> ServeQbservableTcp<TSource>(
			this IObservable<TSource> source,
			IPEndPoint endPoint)
		{
			return QbservableTcpServer.CreateService<object, TSource>(endPoint, _ => source);
		}

		public static IObservable<TcpClientTermination> ServeQbservableTcp<TSource>(
			this IObservable<TSource> source,
			IPEndPoint endPoint,
			QbservableServiceOptions options)
		{
			return QbservableTcpServer.CreateService<object, TSource>(endPoint, options, _ => source);
		}

		public static IObservable<TcpClientTermination> ServeQbservableTcp<TSource>(
			this IObservable<TSource> source,
			IPEndPoint endPoint,
			IRemotingFormatter formatter)
		{
			return QbservableTcpServer.CreateService<object, TSource>(endPoint, formatter, _ => source);
		}

		public static IObservable<TcpClientTermination> ServeQbservableTcp<TSource>(
			this IObservable<TSource> source,
			IPEndPoint endPoint,
			IRemotingFormatter formatter,
			QbservableServiceOptions options)
		{
			return QbservableTcpServer.CreateService<object, TSource>(endPoint, formatter, options, _ => source);
		}

		public static IObservable<TcpClientTermination> ServeTcp<TSource>(
			this IQbservable<TSource> source,
			IPEndPoint endPoint)
		{
			return QbservableTcpServer.CreateService<object, TSource>(endPoint, _ => source);
		}

		public static IObservable<TcpClientTermination> ServeTcp<TSource>(
			this IQbservable<TSource> source,
			IPEndPoint endPoint,
			QbservableServiceOptions options)
		{
			return QbservableTcpServer.CreateService<object, TSource>(endPoint, options, _ => source);
		}

		public static IObservable<TcpClientTermination> ServeTcp<TSource>(
			this IQbservable<TSource> source,
			IPEndPoint endPoint,
			IRemotingFormatter formatter)
		{
			return QbservableTcpServer.CreateService<object, TSource>(endPoint, formatter, _ => source);
		}

		public static IObservable<TcpClientTermination> ServeTcp<TSource>(
			this IQbservable<TSource> source,
			IPEndPoint endPoint,
			IRemotingFormatter formatter,
			QbservableServiceOptions options)
		{
			return QbservableTcpServer.CreateService<object, TSource>(endPoint, formatter, options, _ => source);
		}
	}
}