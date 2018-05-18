using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using QbservableProvider.Properties;

namespace QbservableProvider
{
	internal sealed class DefaultServerDuplexQbservableProtocolSink : ServerDuplexQbservableProtocolSink<QbservableMessage>
	{
		private readonly DefaultQbservableProtocol protocol;

		public DefaultServerDuplexQbservableProtocolSink(DefaultQbservableProtocol protocol)
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
					case QbservableProtocolMessageKind.DuplexResponse:
						HandleResponse(duplexMessage.Id, duplexMessage.Value);
						break;
					case QbservableProtocolMessageKind.DuplexErrorResponse:
						HandleErrorResponse(duplexMessage.Id, duplexMessage.Error);
						break;
					case QbservableProtocolMessageKind.DuplexSubscribeResponse:
						HandleSubscribeResponse(duplexMessage.Id, (int) duplexMessage.Value);
						break;
					case QbservableProtocolMessageKind.DuplexGetEnumeratorResponse:
						HandleGetEnumeratorResponse(duplexMessage.Id, (int) duplexMessage.Value);
						break;
					case QbservableProtocolMessageKind.DuplexGetEnumeratorErrorResponse:
						HandleGetEnumeratorErrorResponse(duplexMessage.Id, duplexMessage.Error);
						break;
					case QbservableProtocolMessageKind.DuplexEnumeratorResponse:
						HandleEnumeratorResponse(duplexMessage.Id, (Tuple<bool, object>) duplexMessage.Value);
						break;
					case QbservableProtocolMessageKind.DuplexEnumeratorErrorResponse:
						HandleEnumeratorErrorResponse(duplexMessage.Id, duplexMessage.Error);
						break;
					case QbservableProtocolMessageKind.DuplexOnNext:
						HandleOnNext(duplexMessage.Id, duplexMessage.Value);
						break;
					case QbservableProtocolMessageKind.DuplexOnCompleted:
						HandleOnCompleted(duplexMessage.Id);
						break;
					case QbservableProtocolMessageKind.DuplexOnError:
						HandleOnError(duplexMessage.Id, duplexMessage.Error);
						break;
					default:
						throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Errors.ProtocolUnknownMessageKindFormat, duplexMessage.Kind));
				}

				duplexMessage.Handled = true;
			}

			return Task.FromResult(message);
		}

		public override object Invoke(int clientId, object[] arguments)
		{
			return protocol.ServerSendDuplexMessage(clientId, id => DuplexQbservableMessage.CreateInvoke(id, arguments, protocol));
		}

		public override IDisposable Subscribe(int clientId, Action<object> onNext, Action<Exception> onError, Action onCompleted)
		{
			return protocol.ServerSendSubscribeDuplexMessage(clientId, onNext, onError, onCompleted);
		}

		public override int GetEnumerator(int clientId)
		{
			return (int) protocol.ServerSendDuplexMessage(clientId, id => DuplexQbservableMessage.CreateGetEnumerator(id, protocol));
		}

		public override Tuple<bool, object> MoveNext(int enumeratorId)
		{
			return (Tuple<bool, object>) protocol.ServerSendEnumeratorDuplexMessage(enumeratorId, id => DuplexQbservableMessage.CreateMoveNext(id, protocol));
		}

		public override void ResetEnumerator(int enumeratorId)
		{
			protocol.ServerSendEnumeratorDuplexMessage(enumeratorId, id => DuplexQbservableMessage.CreateResetEnumerator(id, protocol));
		}

		public override void DisposeEnumerator(int enumeratorId)
		{
			protocol.SendDuplexMessageAsync(DuplexQbservableMessage.CreateDisposeEnumerator(enumeratorId, protocol));
		}
	}
}