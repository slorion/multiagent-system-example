using System;

namespace QbservableProvider
{
	internal sealed class DuplexQbservableMessage : QbservableMessage
	{
		public DuplexCallbackId Id
		{
			get
			{
				return id;
			}
		}

		public object Value
		{
			get
			{
				return value;
			}
		}

		public Exception Error
		{
			get
			{
				return error;
			}
		}

		private readonly DuplexCallbackId id;
		private readonly object value;
		private readonly Exception error;

		private DuplexQbservableMessage(QbservableProtocolMessageKind kind, DuplexCallbackId id, object value, byte[] data)
			: base(kind, data, data.Length)
		{
			this.id = id;
			this.value = value;
		}

		private DuplexQbservableMessage(QbservableProtocolMessageKind kind, DuplexCallbackId id, Exception error, byte[] data)
			: base(kind, data, data.Length)
		{
			this.id = id;
			this.error = error;
		}

		private DuplexQbservableMessage(QbservableProtocolMessageKind kind, DuplexCallbackId id, object value, byte[] data, long length)
			: base(kind, data, length)
		{
			this.id = id;
			this.value = value;
		}

		private DuplexQbservableMessage(QbservableProtocolMessageKind kind, DuplexCallbackId id, Exception error, byte[] data, long length)
			: base(kind, data, length)
		{
			this.id = id;
			this.error = error;
		}

		public static bool TryParse(QbservableMessage message, QbservableProtocol protocol, out DuplexQbservableMessage duplexMessage)
		{
			switch (message.Kind)
			{
				case QbservableProtocolMessageKind.DuplexInvoke:
				case QbservableProtocolMessageKind.DuplexResponse:
				case QbservableProtocolMessageKind.DuplexSubscribeResponse:
				case QbservableProtocolMessageKind.DuplexGetEnumeratorResponse:
				case QbservableProtocolMessageKind.DuplexEnumeratorResponse:
				case QbservableProtocolMessageKind.DuplexOnNext:
				// The following cases are handled the same as the above cases to ensure that extra data is read, though it's unexpected.
				case QbservableProtocolMessageKind.DuplexOnCompleted:
				case QbservableProtocolMessageKind.DuplexSubscribe:
				case QbservableProtocolMessageKind.DuplexDisposeSubscription:
				case QbservableProtocolMessageKind.DuplexGetEnumerator:
				case QbservableProtocolMessageKind.DuplexMoveNext:
				case QbservableProtocolMessageKind.DuplexResetEnumerator:
				case QbservableProtocolMessageKind.DuplexDisposeEnumerator:
					duplexMessage = new DuplexQbservableMessage(
						message.Kind,
						BitConverter.ToInt64(message.Data, 0),
						protocol.Deserialize<object>(message.Data, offset: DuplexCallbackId.Size),
						message.Data,
						message.Length);
					return true;
				case QbservableProtocolMessageKind.DuplexErrorResponse:
				case QbservableProtocolMessageKind.DuplexGetEnumeratorErrorResponse:
				case QbservableProtocolMessageKind.DuplexEnumeratorErrorResponse:
				case QbservableProtocolMessageKind.DuplexOnError:
					duplexMessage = new DuplexQbservableMessage(
						message.Kind,
						BitConverter.ToInt64(message.Data, 0),
						protocol.Deserialize<Exception>(message.Data, offset: DuplexCallbackId.Size),
						message.Data,
						message.Length);
					return true;
				default:
					duplexMessage = null;
					return false;
			}
		}

		public static DuplexQbservableMessage CreateInvoke(DuplexCallbackId id, object[] arguments, QbservableProtocol protocol)
		{
			return new DuplexQbservableMessage(QbservableProtocolMessageKind.DuplexInvoke, id, arguments, Serialize(id, arguments, protocol));
		}

		public static DuplexQbservableMessage CreateSubscribe(DuplexCallbackId id, QbservableProtocol protocol)
		{
			return CreateWithoutValue(QbservableProtocolMessageKind.DuplexSubscribe, id, protocol);
		}

		public static DuplexQbservableMessage CreateDisposeSubscription(int subscriptionId, QbservableProtocol protocol)
		{
			return CreateWithoutValue(QbservableProtocolMessageKind.DuplexDisposeSubscription, subscriptionId, protocol);
		}

		public static DuplexQbservableMessage CreateGetEnumerator(DuplexCallbackId id, DefaultQbservableProtocol protocol)
		{
			return CreateWithoutValue(QbservableProtocolMessageKind.DuplexGetEnumerator, id, protocol);
		}

		public static DuplexQbservableMessage CreateGetEnumeratorResponse(DuplexCallbackId id, int clientEnumeratorId, DefaultQbservableProtocol protocol)
		{
			return new DuplexQbservableMessage(QbservableProtocolMessageKind.DuplexGetEnumeratorResponse, id, clientEnumeratorId, Serialize(id, clientEnumeratorId, protocol));
		}

		public static DuplexQbservableMessage CreateGetEnumeratorError(DuplexCallbackId id, Exception error, DefaultQbservableProtocol protocol)
		{
			return new DuplexQbservableMessage(QbservableProtocolMessageKind.DuplexGetEnumeratorErrorResponse, id, error, Serialize(id, error, protocol));
		}

		public static DuplexQbservableMessage CreateEnumeratorResponse(DuplexCallbackId id, bool result, object current, DefaultQbservableProtocol protocol)
		{
			var value = Tuple.Create(result, current);

			return new DuplexQbservableMessage(QbservableProtocolMessageKind.DuplexEnumeratorResponse, id, value, Serialize(id, value, protocol));
		}

		public static DuplexQbservableMessage CreateEnumeratorError(DuplexCallbackId id, Exception error, DefaultQbservableProtocol protocol)
		{
			return new DuplexQbservableMessage(QbservableProtocolMessageKind.DuplexEnumeratorErrorResponse, id, error, Serialize(id, error, protocol));
		}

		public static DuplexQbservableMessage CreateMoveNext(DuplexCallbackId id, QbservableProtocol protocol)
		{
			return CreateWithoutValue(QbservableProtocolMessageKind.DuplexMoveNext, id, protocol);
		}

		public static DuplexQbservableMessage CreateResetEnumerator(DuplexCallbackId id, QbservableProtocol protocol)
		{
			return CreateWithoutValue(QbservableProtocolMessageKind.DuplexResetEnumerator, id, protocol);
		}

		public static DuplexQbservableMessage CreateDisposeEnumerator(int clientId, QbservableProtocol protocol)
		{
			return CreateWithoutValue(QbservableProtocolMessageKind.DuplexDisposeEnumerator, clientId, protocol);
		}

		public static DuplexQbservableMessage CreateResponse(DuplexCallbackId id, object value, QbservableProtocol protocol)
		{
			return new DuplexQbservableMessage(QbservableProtocolMessageKind.DuplexResponse, id, value, Serialize(id, value, protocol));
		}

		public static DuplexQbservableMessage CreateErrorResponse(DuplexCallbackId id, Exception error, QbservableProtocol protocol)
		{
			return new DuplexQbservableMessage(QbservableProtocolMessageKind.DuplexErrorResponse, id, error, Serialize(id, error, protocol));
		}

		public static DuplexQbservableMessage CreateSubscribeResponse(DuplexCallbackId id, int clientSubscriptionId, QbservableProtocol protocol)
		{
			return new DuplexQbservableMessage(QbservableProtocolMessageKind.DuplexSubscribeResponse, id, clientSubscriptionId, Serialize(id, clientSubscriptionId, protocol));
		}

		public static DuplexQbservableMessage CreateOnNext(DuplexCallbackId id, object value, QbservableProtocol protocol)
		{
			return new DuplexQbservableMessage(QbservableProtocolMessageKind.DuplexOnNext, id, value, Serialize(id, value, protocol));
		}

		public static DuplexQbservableMessage CreateOnCompleted(DuplexCallbackId id, QbservableProtocol protocol)
		{
			return CreateWithoutValue(QbservableProtocolMessageKind.DuplexOnCompleted, id, protocol);
		}

		public static DuplexQbservableMessage CreateOnError(DuplexCallbackId id, Exception error, QbservableProtocol protocol)
		{
			return new DuplexQbservableMessage(QbservableProtocolMessageKind.DuplexOnError, id, error, Serialize(id, error, protocol));
		}

		private static DuplexQbservableMessage CreateWithoutValue(QbservableProtocolMessageKind kind, DuplexCallbackId id, QbservableProtocol protocol)
		{
			return new DuplexQbservableMessage(kind, id, value: null, data: Serialize(id, null, protocol));
		}

		private static byte[] Serialize(DuplexCallbackId id, object value, QbservableProtocol protocol)
		{
			var idData = BitConverter.GetBytes(id);

			long serializedDataLength;
			var serializedData = protocol.Serialize(value, out serializedDataLength);

			var data = new byte[idData.Length + serializedDataLength];

			Array.Copy(idData, data, idData.Length);
			Array.Copy(serializedData, 0, data, idData.Length, serializedDataLength);

			return data;
		}

		public override string ToString()
		{
			return "{" + Kind + ", " + id + ", Length = " + Length + "}";
		}
	}
}