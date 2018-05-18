/* Original file modified by Sébastien Lorion */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;

namespace QbservableProvider
{
	internal sealed class TcpClientQuery<TResult> : QbservableBase<TResult, TcpClientQbservableProvider>
	{
		private readonly object argument;

		public TcpClientQuery(TcpClientQbservableProvider provider, object argument)
			: base(provider)
		{
			this.argument = argument;
		}

		public TcpClientQuery(TcpClientQbservableProvider provider, object argument, Expression expression)
			: base(provider, expression)
		{
			this.argument = argument;
		}

		protected override IDisposable SubscribeCore(IObserver<TResult> observer)
		{
			return
				(from socket in ObservableSocket.Connect(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp, Provider.EndPoint)
				 from result in
					 Observable.Create<TResult>(
						 innerObserver =>
						 {
							 var cancel = new CancellationDisposable();

							 // disable Nagle algorithm so that observed events are received as soon as they happen
							 socket.NoDelay = true;

							 var subscription = Observable.Using(
								 () => new NetworkStream(socket, ownsSocket: false),
								 stream => ReadObservable(stream, cancel.Token))
								 .Subscribe(innerObserver);

							 return new CompositeDisposable(subscription, cancel);
						 })
						 .Finally(socket.Close)
				 select result)
				.Subscribe(observer);
		}

		private IObservable<TResult> ReadObservable(Stream stream, CancellationToken cancel)
		{
			return from protocol in QbservableProtocol.NegotiateClientAsync(stream, Provider.Formatter, cancel).ToObservable()
						 from result in protocol
							.ExecuteClient<TResult>(PrepareExpression(protocol), argument)
							.Finally(protocol.Dispose)
						 select result;
		}

		public Expression PrepareExpression(QbservableProtocol protocol)
		{
			QbservableProviderDiagnostics.DebugPrint(Expression, "TcpClientQuery Original Expression");

			if (!Expression.Type.IsGenericType
				|| (Expression.Type.GetGenericTypeDefinition() != typeof(IQbservable<>)
					&& Expression.Type.GetGenericTypeDefinition() != typeof(TcpClientQuery<>)))
			{
				throw new InvalidOperationException("The query must end as an IQbservable<T>.");
			}

			var visitor = ReplaceConstantsVisitor.CreateForGenericTypeByDefinition(
				typeof(TcpClientQuery<>),
				(_, actualType) => Activator.CreateInstance(typeof(QbservableSourcePlaceholder<>).MakeGenericType(actualType.GetGenericArguments()[0]), true),
				type => typeof(IQbservable<>).MakeGenericType(type.GetGenericArguments()[0]));

			var result = visitor.Visit(Expression);

			if (visitor.ReplacedConstants == 0)
			{
				throw new InvalidOperationException("A queryable observable service was not found in the query.");
			}

			var evaluator = Provider.LocalEvaluator;

			if (!evaluator.IsKnownType(Provider.SourceType))
			{
				evaluator.AddKnownType(Provider.SourceType);
			}

			var evaluationVisitor = new LocalEvaluationVisitor(evaluator, protocol);

			var preparedExpression = evaluationVisitor.Visit(result);

			QbservableProviderDiagnostics.DebugPrint(preparedExpression, "TcpClientQuery Rewritten Expression");

			return preparedExpression;
		}
	}
}