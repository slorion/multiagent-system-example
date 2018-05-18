using System;
using System.Collections.Generic;
using System.Linq;

namespace QbservableProvider
{
	[Serializable]
	internal sealed class DuplexCallbackEnumerable<T> : DuplexCallback, IEnumerable<T>
	{
		public DuplexCallbackEnumerable(int id)
			: base(id)
		{
		}

		public IEnumerator<T> GetEnumerator()
		{
			// A try..catch block is required because the Rx SelectMany operator doesn't send an exception from GetEnumerator to OnError.
			try
			{
				var enumeratorId = Sink.GetEnumerator(Id);

				return new DuplexCallbackEnumerator(enumeratorId, Protocol, Sink);
			}
			catch (Exception ex)
			{
				Protocol.CancelAllCommunication(ex);

				return Enumerable.Empty<T>().GetEnumerator();
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private sealed class DuplexCallbackEnumerator : IEnumerator<T>
		{
			public T Current
			{
				get
				{
					return (T) current;
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}

			private readonly int enumeratorId;
			private readonly IServerDuplexQbservableProtocolSink sink;
			private readonly QbservableProtocol protocol;
			private object current;

			public DuplexCallbackEnumerator(int enumeratorId, QbservableProtocol protocol, IServerDuplexQbservableProtocolSink sink)
			{
				this.enumeratorId = enumeratorId;
				this.protocol = protocol;
				this.sink = sink;
			}

			public bool MoveNext()
			{
				var result = sink.MoveNext(enumeratorId);

				if (result.Item1)
				{
					current = result.Item2;
				}

				return result.Item1;
			}

			public void Reset()
			{
				// A try..catch block may be required, though Rx doesn't call the Reset method at all.
				try
				{
					sink.ResetEnumerator(enumeratorId);
				}
				catch (Exception ex)
				{
					protocol.CancelAllCommunication(ex);
				}
			}

			public void Dispose()
			{
				// A try..catch block is required because the Rx SelectMany operator doesn't send an exception from Dispose to OnError.
				try
				{
					sink.DisposeEnumerator(enumeratorId);
				}
				catch (Exception ex)
				{
					protocol.CancelAllCommunication(ex);
				}
			}
		}
	}
}