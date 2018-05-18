using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;
using QbservableProvider.Properties;
using System.IO;

namespace QbservableProvider
{
	public abstract class QbservableProtocol<TMessage> : QbservableProtocol
		where TMessage : IProtocolMessage
	{
		protected IList<QbservableProtocolSink<TMessage>> Sinks
		{
			get
			{
				return sinks;
			}
		}

		private readonly List<QbservableProtocolSink<TMessage>> sinks = new List<QbservableProtocolSink<TMessage>>();

		protected QbservableProtocol(Stream stream, IRemotingFormatter formatter, CancellationToken cancel)
			: base(stream, formatter, cancel)
		{
			Contract.Ensures(IsClient);
		}

		protected QbservableProtocol(Stream stream, IRemotingFormatter formatter, QbservableServiceOptions serviceOptions, CancellationToken cancel)
			: base(stream, formatter, serviceOptions, cancel)
		{
			Contract.Ensures(!IsClient);
		}

		protected virtual IEnumerable<QbservableProtocolSink<TMessage>> CreateClientSinks()
		{
			// for derived types
			yield break;
		}

		protected virtual IEnumerable<QbservableProtocolSink<TMessage>> CreateServerSinks()
		{
			// for derived types
			yield break;
		}

		protected abstract ClientDuplexQbservableProtocolSink<TMessage> CreateClientDuplexSink();

		protected abstract ServerDuplexQbservableProtocolSink<TMessage> CreateServerDuplexSink();

		internal sealed override IClientDuplexQbservableProtocolSink CreateClientDuplexSinkInternal()
		{
			return CreateClientDuplexSink();
		}

		internal sealed override IServerDuplexQbservableProtocolSink CreateServerDuplexSinkInternal()
		{
			return CreateServerDuplexSink();
		}

		internal sealed override async Task ServerReceiveAsync()
		{
			Contract.Requires(!IsClient);

			while (!Cancel.IsCancellationRequested)
			{
				var message = await ReceiveMessageAsync().ConfigureAwait(false);

				if (ServerHandleClientShutDown(message))
				{
					break;
				}
				else if (!message.Handled)
				{
					throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Errors.ProtocolUnknownMessageKindFormat, message));
				}
			}
		}

		protected abstract bool ServerHandleClientShutDown(TMessage message);

		protected async Task SendMessageAsync(TMessage message)
		{
			message = await ApplySinksForSending(message).ConfigureAwait(false);

			await SendMessageCoreAsync(message).ConfigureAwait(false);
		}

		protected async Task<TMessage> ReceiveMessageAsync()
		{
			var message = await ReceiveMessageCoreAsync().ConfigureAwait(false);

			return await ApplySinksForReceiving(message).ConfigureAwait(false);
		}

		protected abstract Task SendMessageCoreAsync(TMessage message);

		protected abstract Task<TMessage> ReceiveMessageCoreAsync();

		internal sealed override async Task InitializeSinksAsync()
		{
			if (IsClient)
			{
				sinks.AddRange(CreateClientSinks());
			}
			else
			{
				sinks.AddRange(CreateServerSinks());

				if (ServiceOptions.EnableDuplex)
				{
					sinks.Add(CreateServerDuplexSink());
				}
			}

			foreach (var sink in sinks)
			{
				await sink.InitializeAsync(this, Cancel).ConfigureAwait(false);
			}
		}

		private async Task<TMessage> ApplySinksForSending(TMessage message)
		{
			foreach (var sink in sinks)
			{
				message = await sink.SendingAsync(message, Cancel).ConfigureAwait(false);
			}

			return message;
		}

		private async Task<TMessage> ApplySinksForReceiving(TMessage message)
		{
			foreach (var sink in sinks)
			{
				message = await sink.ReceivingAsync(message, Cancel).ConfigureAwait(false);
			}

			return message;
		}

		public sealed override TSink FindSink<TSink>()
		{
			return sinks.OfType<TSink>().FirstOrDefault();
		}

		public sealed override TSink GetOrAddSink<TSink>(Func<TSink> createSink)
		{
			var sink = FindSink<TSink>();

			if (sink == null)
			{
				sink = createSink();
				sinks.Add((QbservableProtocolSink<TMessage>) (object) sink);
			}

			return sink;
		}
	}
}