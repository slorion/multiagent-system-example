using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using QbservableProvider.Properties;

namespace QbservableProvider
{
	internal sealed class DefaultClientDuplexQbservableProtocolSink : ClientDuplexQbservableProtocolSink<QbservableMessage>
	{
		private readonly DefaultQbservableProtocol protocol;

		public DefaultClientDuplexQbservableProtocolSink(DefaultQbservableProtocol protocol)
		{
			this.protocol = protocol;
		}

		public override Task InitializeAsync(QbservableProtocol<QbservableMessage> protocol, CancellationToken cancel)
		{
			return Task.FromResult(false);
		}

		public override Task<QbservableMessage> SendingAsync(QbservableMessage message, CancellationToken cancel)
		{
			return Task.FromResult(message);
		}

		public override Task<QbservableMessage> ReceivingAsync(QbservableMessage message, CancellationToken cancel)
		{
			DuplexQbservableMessage duplexMessage;

			if (DuplexQbservableMessage.TryParse(message, protocol, out duplexMessage))
			{
				message = duplexMessage;

				switch (duplexMessage.Kind)
				{
					case QbservableProtocolMessageKind.DuplexInvoke:
						Invoke(duplexMessage.Id, (object[]) duplexMessage.Value);
						break;
					case QbservableProtocolMessageKind.DuplexSubscribe:
						Subscribe(duplexMessage.Id);
						break;
					case QbservableProtocolMessageKind.DuplexDisposeSubscription:
						DisposeSubscription(duplexMessage.Id.ClientId);
						break;
					case QbservableProtocolMessageKind.DuplexGetEnumerator:
						GetEnumerator(duplexMessage.Id);
						break;
					case QbservableProtocolMessageKind.DuplexMoveNext:
						MoveNext(duplexMessage.Id);
						break;
					case QbservableProtocolMessageKind.DuplexResetEnumerator:
						ResetEnumerator(duplexMessage.Id);
						break;
					case QbservableProtocolMessageKind.DuplexDisposeEnumerator:
						DisposeEnumerator(duplexMessage.Id.ClientId);
						break;
					default:
						throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Errors.ProtocolUnknownMessageKindFormat, duplexMessage.Kind));
				}

				duplexMessage.Handled = true;
			}

			return Task.FromResult(message);
		}

		protected override void SendResponse(DuplexCallbackId id, object result)
		{
			protocol.SendDuplexMessageAsync(DuplexQbservableMessage.CreateResponse(id, result, protocol));
		}

		protected override void SendError(DuplexCallbackId id, Exception error)
		{
			protocol.SendDuplexMessageAsync(DuplexQbservableMessage.CreateErrorResponse(id, error, protocol));
		}

		protected override void SendSubscribeResponse(DuplexCallbackId id, int clientSubscriptionId)
		{
			protocol.SendDuplexMessageAsync(DuplexQbservableMessage.CreateSubscribeResponse(id, clientSubscriptionId, protocol));
		}

		public override void SendOnNext(DuplexCallbackId id, object value)
		{
			protocol.SendDuplexMessageAsync(DuplexQbservableMessage.CreateOnNext(id, value, protocol));
		}

		public override void SendOnError(DuplexCallbackId id, Exception error)
		{
			protocol.SendDuplexMessageAsync(DuplexQbservableMessage.CreateOnError(id, error, protocol));
		}

		public override void SendOnCompleted(DuplexCallbackId id)
		{
			protocol.SendDuplexMessageAsync(DuplexQbservableMessage.CreateOnCompleted(id, protocol));
		}

		protected override void SendGetEnumeratorResponse(DuplexCallbackId id, int clientEnumeratorId)
		{
			protocol.SendDuplexMessageAsync(DuplexQbservableMessage.CreateGetEnumeratorResponse(id, clientEnumeratorId, protocol));
		}

		protected override void SendGetEnumeratorError(DuplexCallbackId id, Exception error)
		{
			protocol.SendDuplexMessageAsync(DuplexQbservableMessage.CreateGetEnumeratorError(id, error, protocol));
		}

		protected override void SendEnumeratorResponse(DuplexCallbackId id, bool result, object current)
		{
			protocol.SendDuplexMessageAsync(DuplexQbservableMessage.CreateEnumeratorResponse(id, result, current, protocol));
		}

		protected override void SendEnumeratorError(DuplexCallbackId id, Exception error)
		{
			protocol.SendDuplexMessageAsync(DuplexQbservableMessage.CreateEnumeratorError(id, error, protocol));
		}
	}
}