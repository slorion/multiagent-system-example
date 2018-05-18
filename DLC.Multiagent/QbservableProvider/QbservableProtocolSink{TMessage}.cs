using System.Threading;
using System.Threading.Tasks;

namespace QbservableProvider
{
	public abstract class QbservableProtocolSink<TMessage>
		where TMessage : IProtocolMessage
	{
		public abstract Task InitializeAsync(QbservableProtocol<TMessage> protocol, CancellationToken cancel);

		public abstract Task<TMessage> SendingAsync(TMessage message, CancellationToken cancel);

		public abstract Task<TMessage> ReceivingAsync(TMessage message, CancellationToken cancel);
	}
}