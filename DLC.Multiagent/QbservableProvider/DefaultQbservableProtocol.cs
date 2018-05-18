/* Original file modified by Sébastien Lorion */

using System;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;
using QbservableProvider.Expressions;
using QbservableProvider.Properties;

namespace QbservableProvider
{
	internal sealed class DefaultQbservableProtocol : QbservableProtocol<QbservableMessage>
	{
		public DefaultQbservableProtocol(Stream stream, IRemotingFormatter formatter, CancellationToken cancel)
			: base(stream, formatter, cancel)
		{
		}

		public DefaultQbservableProtocol(Stream stream, IRemotingFormatter formatter, QbservableServiceOptions serviceOptions, CancellationToken cancel)
			: base(stream, formatter, serviceOptions, cancel)
		{
		}

		protected sealed override ClientDuplexQbservableProtocolSink<QbservableMessage> CreateClientDuplexSink()
		{
			return new DefaultClientDuplexQbservableProtocolSink(this);
		}

		protected sealed override ServerDuplexQbservableProtocolSink<QbservableMessage> CreateServerDuplexSink()
		{
			return new DefaultServerDuplexQbservableProtocolSink(this);
		}

		protected override async Task ClientSendQueryAsync(Expression expression, object argument)
		{
			if (argument != null)
			{
				await SendMessageAsync(QbservableProtocolMessageKind.Argument, argument).ConfigureAwait(false);
			}

			var converter = new SerializableExpressionConverter();

			await SendMessageAsync(QbservableProtocolMessageKind.Subscribe, converter.Convert(expression)).ConfigureAwait(false);
		}

		protected override IObservable<TResult> ClientReceive<TResult>()
		{
			return Observable.Create<TResult>(o =>
			{
				var subscription = Observable.Create<TResult>(
					async observer =>
					{
						do
						{
							var message = await ReceiveMessageAsync().ConfigureAwait(false);

							switch (message.Kind)
							{
								case QbservableProtocolMessageKind.OnNext:
									observer.OnNext(Deserialize<TResult>(message.Data));
									break;
								case QbservableProtocolMessageKind.OnCompleted:
									Deserialize<object>(message.Data);	// just in case data is sent, though it's unexpected.
									observer.OnCompleted();
									goto Return;
								case QbservableProtocolMessageKind.OnError:
									observer.OnError(Deserialize<Exception>(message.Data));
									goto Return;
								case QbservableProtocolMessageKind.ShutDown:
									ShutDownWithoutResponse(GetShutDownReason(message, QbservableProtocolShutDownReason.None));
									goto Return;
								default:
									if (!message.Handled)
									{
										throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Errors.ProtocolUnknownMessageKindFormat, message.Kind));
									}
									break;
							}
						}
						while (!Cancel.IsCancellationRequested);

					Return:
						return () => { };
					})
					.Subscribe(o);

				return new CompositeDisposable(
					subscription,
					Disposable.Create(async () =>
					{
						try
						{
							await ShutDownAsync(QbservableProtocolShutDownReason.ClientTerminated).ConfigureAwait(false);
						}
						catch (OperationCanceledException)
						{
						}
						catch (Exception ex)
						{
							CancelAllCommunication(ex);
						}
					}));
			});
		}

		protected override async Task<Tuple<Expression, object>> ServerReceiveQueryAsync()
		{
			do
			{
				var message = await ReceiveMessageAsync().ConfigureAwait(false);

				object argument;

				if (message.Kind == QbservableProtocolMessageKind.Argument)
				{
					argument = Deserialize<object>(message.Data);

					message = await ReceiveMessageAsync().ConfigureAwait(false);
				}
				else
				{
					argument = null;
				}

				if (message.Kind == QbservableProtocolMessageKind.Subscribe)
				{
					var converter = new SerializableExpressionConverter();

					return Tuple.Create(converter.Convert(Deserialize<SerializableExpression>(message.Data)), argument);
				}
				else if (ServerHandleClientShutDown(message))
				{
					throw new OperationCanceledException();
				}
				else if (!message.Handled)
				{
					throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Errors.ProtocolExpectedMessageSubscribeFormat, message.Kind));
				}
			}
			while (true);
		}

		protected override Task ServerSendAsync(NotificationKind kind, object data)
		{
			var messageKind = GetMessageKind(kind);

			if (messageKind == QbservableProtocolMessageKind.OnCompleted)
			{
				return SendMessageAsync(new QbservableMessage(messageKind));
			}
			else
			{
				return SendMessageAsync(messageKind, data);
			}
		}

		private QbservableProtocolShutDownReason GetShutDownReason(QbservableMessage message, QbservableProtocolShutDownReason defaultReason)
		{
			if (message.Data.Length > 0)
			{
				return (QbservableProtocolShutDownReason) message.Data[0];
			}
			else
			{
				return defaultReason;
			}
		}

		protected override bool ServerHandleClientShutDown(QbservableMessage message)
		{
			if (message.Kind == QbservableProtocolMessageKind.ShutDown)
			{
				var reason = GetShutDownReason(message, QbservableProtocolShutDownReason.ClientTerminated);

				ShutDownWithoutResponse(reason);

				return true;
			}

			return false;
		}

		protected override Task ShutDownCoreAsync()
		{
			return SendMessageAsync(new QbservableMessage(QbservableProtocolMessageKind.ShutDown, (byte) ShutDownReason));
		}

		private Task SendMessageAsync(QbservableProtocolMessageKind kind, object data)
		{
			long length;
			return SendMessageAsync(new QbservableMessage(kind, Serialize(data, out length), length));
		}

		protected override Task SendMessageCoreAsync(QbservableMessage message)
		{
			var lengthBytes = BitConverter.GetBytes(message.Length);

			var buffer = new byte[1L + lengthBytes.Length + message.Length];

			buffer[0] = (byte) message.Kind;

			Array.Copy(lengthBytes, 0, buffer, 1, lengthBytes.Length);

			if (message.Length > 0)
			{
				Array.Copy(message.Data, 0L, buffer, 1L + lengthBytes.Length, message.Length);
			}

			return SendAsync(buffer, 0, buffer.Length);
		}

		protected override async Task<QbservableMessage> ReceiveMessageCoreAsync()
		{
			var buffer = new byte[1024];

			await ReceiveAsync(buffer, 0, 9).ConfigureAwait(false);

			var messageKind = (QbservableProtocolMessageKind) buffer[0];
			var length = BitConverter.ToInt64(buffer, 1);

			if (length > 0)
			{
				using (var stream = new MemoryStream((int) length))
				{
					long remainder = length;

					do
					{
						int count = Math.Min(buffer.Length, remainder > int.MaxValue ? int.MaxValue : (int) remainder);

						var read = await ReceiveAsync(buffer, 0, count).ConfigureAwait(false);

						stream.Write(buffer, 0, read);

						remainder -= read;
					}
					while (remainder > 0);

					return new QbservableMessage(messageKind, stream.ToArray());
				}
			}

			return new QbservableMessage(messageKind, new byte[0]);
		}

		private static QbservableProtocolMessageKind GetMessageKind(NotificationKind kind)
		{
			switch (kind)
			{
				case NotificationKind.OnNext:
					return QbservableProtocolMessageKind.OnNext;
				case NotificationKind.OnCompleted:
					return QbservableProtocolMessageKind.OnCompleted;
				case NotificationKind.OnError:
					return QbservableProtocolMessageKind.OnError;
				default:
					throw new ArgumentOutOfRangeException("kind");
			}
		}

		private void SendDuplexMessage(DuplexQbservableMessage message)
		{
			SendMessageAsync(message).Wait(Cancel);
		}

		internal async Task SendDuplexMessageAsync(DuplexQbservableMessage message)
		{
			try
			{
				await SendMessageAsync(message).ConfigureAwait(false);
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception ex)
			{
				CancelAllCommunication(ex);
			}
		}

		internal object ServerSendDuplexMessage(int clientId, Func<DuplexCallbackId, DuplexQbservableMessage> messageFactory)
		{
			return ServerSendDuplexMessage(clientId, messageFactory, sink => sink.RegisterInvokeCallback);
		}

		internal object ServerSendEnumeratorDuplexMessage(int clientId, Func<DuplexCallbackId, DuplexQbservableMessage> messageFactory)
		{
			return ServerSendDuplexMessage(clientId, messageFactory, sink => sink.RegisterEnumeratorCallback);
		}

		private object ServerSendDuplexMessage(
			int clientId,
			Func<DuplexCallbackId, DuplexQbservableMessage> messageFactory,
			Func<IServerDuplexQbservableProtocolSink, Func<int, Action<object>, Action<Exception>, DuplexCallbackId>> registrationSelector)
		{
			var waitForResponse = new ManualResetEventSlim(false);

			ExceptionDispatchInfo error = null;
			object result = null;

			var duplexSink = FindSink<IServerDuplexQbservableProtocolSink>();

			var id = registrationSelector(duplexSink)(
				clientId,
				value =>
				{
					result = value;
					waitForResponse.Set();
				},
				ex =>
				{
					error = ExceptionDispatchInfo.Capture(ex);
					waitForResponse.Set();
				});

			var message = messageFactory(id);

			SendDuplexMessage(message);

			waitForResponse.Wait(Cancel);

			if (error != null)
			{
				error.Throw();
			}

			return result;
		}

		internal IDisposable ServerSendSubscribeDuplexMessage(
			int clientId,
			Action<object> onNext,
			Action<Exception> onError,
			Action onCompleted)
		{
			var duplexSink = FindSink<IServerDuplexQbservableProtocolSink>();

			var registration = duplexSink.RegisterObservableCallbacks(
				clientId,
				onNext,
				onError,
				onCompleted,
				subscriptionId => SendDuplexMessageAsync(DuplexQbservableMessage.CreateDisposeSubscription(subscriptionId, this)));

			var id = registration.Item1;
			var subscription = registration.Item2;

			var message = DuplexQbservableMessage.CreateSubscribe(id, this);

			SendDuplexMessage(message);

			return subscription;
		}
	}
}