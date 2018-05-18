using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;

namespace QbservableProvider
{
	/// <summary>
	/// Provides the basic algorithm for a single observable communication channel between a client and server.
	/// </summary>
	public abstract class QbservableProtocol : IDisposable
	{
		public bool IsClient
		{
			get
			{
				return isClient;
			}
		}

		public IList<ExceptionDispatchInfo> Exceptions
		{
			get
			{
				return errors.AsReadOnly();
			}
		}

		public QbservableServiceOptions ServiceOptions
		{
			get
			{
				return serviceOptions;
			}
		}

		public QbservableProtocolShutDownReason ShutDownReason
		{
			get;
			private set;
		}

		protected IRemotingFormatter Formatter
		{
			get
			{
				return formatter;
			}
		}

		protected CancellationToken Cancel
		{
			get
			{
				return cancel;
			}
		}

		private readonly CancellationTokenSource protocolCancellation = new CancellationTokenSource();
		private readonly AsyncConsumerQueue<bool> sendQ = new AsyncConsumerQueue<bool>();
		private readonly AsyncConsumerQueue<int> receiveQ = new AsyncConsumerQueue<int>();
		private readonly List<ExceptionDispatchInfo> errors = new List<ExceptionDispatchInfo>();
		private readonly CancellationToken cancel;
		private readonly Stream stream;
		private readonly IRemotingFormatter formatter;
		private readonly QbservableServiceOptions serviceOptions;
		private readonly bool isClient;

		internal QbservableProtocol(Stream stream, IRemotingFormatter formatter, CancellationToken cancel)
		{
			Contract.Ensures(IsClient);

			this.isClient = true;
			this.stream = stream;
			this.formatter = formatter;
			this.cancel = protocolCancellation.Token;

			cancel.Register(protocolCancellation.Cancel, useSynchronizationContext: false);
		}

		internal QbservableProtocol(Stream stream, IRemotingFormatter formatter, QbservableServiceOptions serviceOptions, CancellationToken cancel)
			: this(stream, formatter, cancel)
		{
			Contract.Ensures(!IsClient);

			this.serviceOptions = serviceOptions;
			this.isClient = false;
		}

		public static async Task<QbservableProtocol> NegotiateClientAsync(Stream stream, IRemotingFormatter formatter, CancellationToken cancel)
		{
			// TODO: Enable protocol registration and implement actual protocol negotiation

			var protocol = new DefaultQbservableProtocol(stream, formatter, cancel);

			const int ping = 123;

			var buffer = BitConverter.GetBytes(ping);

			await protocol.SendAsync(buffer, 0, 4).ConfigureAwait(false);
			await protocol.ReceiveAsync(buffer, 0, 4).ConfigureAwait(false);

			Contract.Assume(BitConverter.ToInt32(buffer, 0) == ping);

			return protocol;
		}

		public static async Task<QbservableProtocol> NegotiateServerAsync(Stream stream, IRemotingFormatter formatter, QbservableServiceOptions serviceOptions, CancellationToken cancel)
		{
			// TODO: Enable protocol registration and implement actual protocol negotiation

			var protocol = new DefaultQbservableProtocol(stream, formatter, serviceOptions, cancel);

			var buffer = new byte[4];

			await protocol.ReceiveAsync(buffer, 0, 4).ConfigureAwait(false);
			await protocol.SendAsync(buffer, 0, 4).ConfigureAwait(false);

			return protocol;
		}

		internal abstract IClientDuplexQbservableProtocolSink CreateClientDuplexSinkInternal();

		internal abstract IServerDuplexQbservableProtocolSink CreateServerDuplexSinkInternal();

		public abstract TSink FindSink<TSink>();

		public abstract TSink GetOrAddSink<TSink>(Func<TSink> createSink);

		public void CancelAllCommunication()
		{
			try
			{
				protocolCancellation.Cancel();
			}
			catch (AggregateException ex)
			{
				errors.Add(ExceptionDispatchInfo.Capture(ex));
			}
			catch (OperationCanceledException)
			{
			}
		}

		public void CancelAllCommunication(Exception exception)
		{
			errors.Add(ExceptionDispatchInfo.Capture(exception));

			if (!protocolCancellation.IsCancellationRequested)
			{
				if (ShutDownReason == QbservableProtocolShutDownReason.None)
				{
					ShutDownReason = QbservableProtocolShutDownReason.ServerError;
				}

				CancelAllCommunication();
			}
		}

		public IObservable<TResult> ExecuteClient<TResult>(Expression expression, object argument)
		{
			Contract.Requires(IsClient);

			return (from _ in InitializeSinksAsync().ToObservable()
							from __ in ClientSendQueryAsync(expression, argument).ToObservable()
							from result in ClientReceive<TResult>()
							select result)
						 .Catch<TResult, OperationCanceledException>(
							 ex =>
							 {
								 if (errors.Count == 1)
								 {
									 return Observable.Throw<TResult>(errors[0].SourceException);
								 }
								 else if (errors.Count > 1)
								 {
									 return Observable.Throw<TResult>(new AggregateException(errors.Select(e => e.SourceException)));
								 }
								 else
								 {
									 return Observable.Throw<TResult>(ex);
								 }
							 })
							.Catch<TResult, Exception>(
								ex =>
								{
									// Cancellation is required in case client-side code is awaiting socket communication in the background; e.g., via a sink
									CancelAllCommunication(ex);

									return Observable.Throw<TResult>(ex);
								});
		}

		public async Task ExecuteServerAsync(IQbservableProvider provider)
		{
			Contract.Requires(!IsClient);

			Task receivingAsync = null;
			ExceptionDispatchInfo fatalException = null;

			try
			{
				await InitializeSinksAsync().ConfigureAwait(false);

				var input = await ServerReceiveQueryAsync().ConfigureAwait(false);

				receivingAsync = ServerReceiveAsync();

				try
				{
					await ExecuteServerQueryAsync(input, provider).ConfigureAwait(false);
				}
				catch (OperationCanceledException)
				{
					throw;
				}
				catch (Exception ex)
				{
					if (ShutDownReason == QbservableProtocolShutDownReason.None)
					{
						ShutDownReason = QbservableProtocolShutDownReason.BadClientRequest;
					}

					CancelAllCommunication(ex);

					fatalException = ExceptionDispatchInfo.Capture(ex);
				}
			}
			catch (OperationCanceledException ex)
			{
				if (errors.Count == 1)
				{
					if (ShutDownReason == QbservableProtocolShutDownReason.None)
					{
						ShutDownReason = QbservableProtocolShutDownReason.ServerError;
					}

					fatalException = errors[0];
				}
				else if (errors.Count > 1)
				{
					if (ShutDownReason == QbservableProtocolShutDownReason.None)
					{
						ShutDownReason = QbservableProtocolShutDownReason.ServerError;
					}

					fatalException = ExceptionDispatchInfo.Capture(new AggregateException(errors.Select(e => e.SourceException)));
				}

				if (ShutDownReason == QbservableProtocolShutDownReason.None)
				{
					ShutDownReason = QbservableProtocolShutDownReason.ClientTerminated;
				}

				fatalException = ExceptionDispatchInfo.Capture(ex);
			}
			catch (Exception ex)
			{
				CancelAllCommunication(ex);

				fatalException = ExceptionDispatchInfo.Capture(ex);
			}

			if (receivingAsync != null)
			{
				try
				{
					await receivingAsync.ConfigureAwait(false);
				}
				catch (OperationCanceledException)
				{
				}
				catch (Exception ex)
				{
					errors.Add(ExceptionDispatchInfo.Capture(ex));
				}
			}

			if (fatalException != null)
			{
				fatalException.Throw();
			}
		}

		internal abstract Task ServerReceiveAsync();

		private async Task ExecuteServerQueryAsync(Tuple<Expression, object> input, IQbservableProvider provider)
		{
			var shutDownReason = QbservableProtocolShutDownReason.ObservableTerminated;

			ExceptionDispatchInfo createQueryError = null;

			if (input == null)
			{
				shutDownReason = QbservableProtocolShutDownReason.ProtocolTerminated;
			}
			else
			{
				var expression = input.Item1;
				var argument = input.Item2;

				if (expression == null)
				{
					shutDownReason = QbservableProtocolShutDownReason.ProtocolTerminated;
				}
				else
				{
					Type dataType = null;
					object observable = null;

					try
					{
						observable = CreateQuery(provider, expression, argument, out dataType);
					}
					catch (Exception ex)
					{
						shutDownReason = QbservableProtocolShutDownReason.ServerError;
						createQueryError = ExceptionDispatchInfo.Capture(ex);
					}

					if (createQueryError == null)
					{
						await SendObservableAsync(observable, dataType, serviceOptions.SendServerErrorsToClients, cancel).ConfigureAwait(false);
					}
					else if (serviceOptions.SendServerErrorsToClients)
					{
						await ServerSendAsync(NotificationKind.OnError, createQueryError.SourceException).ConfigureAwait(false);
					}
				}
			}

			await ShutDownAsync(shutDownReason).ConfigureAwait(false);

			if (createQueryError != null)
			{
				createQueryError.Throw();
			}
		}

		private async Task SendObservableAsync(object untypedObservable, Type dataType, bool sendServerErrorsToClients, CancellationToken cancel)
		{
			var networkErrors = new ConcurrentBag<ExceptionDispatchInfo>();

			ExceptionDispatchInfo expressionSecurityError = null;
			ExceptionDispatchInfo qbservableSubscriptionError = null;
			ExceptionDispatchInfo qbservableError = null;

			var terminationKind = NotificationKind.OnCompleted;

			try
			{
				var cancelSubscription = new CancellationTokenSource();

				cancel.Register(cancelSubscription.Cancel);

				IObservable<object> observable;

				new PermissionSet(PermissionState.Unrestricted).Assert();

				try
				{
					observable = dataType.UpCast(untypedObservable);
				}
				finally
				{
					PermissionSet.RevertAssert();
				}

				await observable.ForEachAsync(
					async data =>
					{
						try
						{
							await ServerSendAsync(NotificationKind.OnNext, data).ConfigureAwait(false);
						}
						catch (OperationCanceledException)
						{
						}
						catch (Exception ex)
						{
							/* Collecting exceptions handles a possible race condition.  Since this code is using a fire-and-forget model to 
							 * subscribe to the observable, due to the async OnNext handler, it's possible that more than one SendAsync task
							 * can be executing concurrently.  As a result, cancelling the cancelSubscription below does not guarantee that 
							 * this catch block won't run again.
							 */
							networkErrors.Add(ExceptionDispatchInfo.Capture(ex));

							cancelSubscription.Cancel();
						}
					},
					cancelSubscription.Token)
					.ConfigureAwait(false);
			}
			catch (OperationCanceledException)
			{
				if (cancel.IsCancellationRequested && networkErrors.Count == 0)
				{
					throw;
				}
			}
			catch (ExpressionSecurityException ex)
			{
				ShutDownReason = QbservableProtocolShutDownReason.ExpressionSecurityViolation;

				expressionSecurityError = ExceptionDispatchInfo.Capture(ex);
			}
			catch (QbservableSubscriptionException ex)
			{
				ShutDownReason = QbservableProtocolShutDownReason.ExpressionSubscriptionException;

				qbservableSubscriptionError = ExceptionDispatchInfo.Capture(ex.InnerException ?? ex);
			}
			catch (TargetInvocationException ex)
			{
				if (ex.InnerException is QbservableSubscriptionException)
				{
					ShutDownReason = QbservableProtocolShutDownReason.ExpressionSubscriptionException;

					qbservableSubscriptionError = ExceptionDispatchInfo.Capture(ex.InnerException.InnerException ?? ex.InnerException);
				}
				else
				{
					qbservableSubscriptionError = ExceptionDispatchInfo.Capture(ex);
				}
			}
			catch (Exception ex)
			{
				terminationKind = NotificationKind.OnError;
				qbservableError = ExceptionDispatchInfo.Capture(ex);
			}

			var error = expressionSecurityError ?? qbservableSubscriptionError;

			if (error != null)
			{
				if (networkErrors.Count > 0)
				{
					// It's not technically a network error, but since the client can't receive it anyway add it so that it's thrown later
					networkErrors.Add(error);
				}
				else
				{
					if (sendServerErrorsToClients || expressionSecurityError != null)
					{
						var exception = expressionSecurityError == null
							? error.SourceException
							: new SecurityException(error.SourceException.Message);		// Remove stack trace

						await ServerSendAsync(NotificationKind.OnError, exception).ConfigureAwait(false);
					}

					error.Throw();
				}
			}

			/* There's an acceptable race condition here whereby ForEachAsync is cancelled by the external cancellation token though
			 * it's still executing a fire-and-forget task.  It's possible for the fire-and-forget task to throw before seeing the 
			 * cancellation, yet after the following code has already executed.  In that case, since the cancellation was requested 
			 * externally, it's acceptable for the cancellation to simply beat the send error, thus the error can safely be ignored.
			 */
			if (networkErrors.Count > 0)
			{
				throw new AggregateException(networkErrors.Select(e => e.SourceException));
			}
			else
			{
				await ServerSendAsync(terminationKind, (qbservableError == null ? null : qbservableError.SourceException)).ConfigureAwait(false);

				if (qbservableError != null)
				{
					qbservableError.Throw();
				}
			}
		}

		private static object CreateQuery(IQbservableProvider provider, Expression expression, object argument, out Type type)
		{
			Contract.Requires(provider != null);
			Contract.Requires(expression != null);
			Contract.Requires(expression.Type.IsGenericType);
			Contract.Requires(!expression.Type.IsGenericTypeDefinition);
			Contract.Requires(expression.Type.GetGenericTypeDefinition() == typeof(IQbservable<>));

			type = expression.Type.GetGenericArguments()[0];

			var parameterized = provider as IParameterizedQbservableProvider;

			new PermissionSet(PermissionState.Unrestricted).Assert();

			try
			{
				if (parameterized != null)
				{
					return provider.GetType()
						.GetInterfaceMap(typeof(IParameterizedQbservableProvider))
						.TargetMethods
						.First()
						.MakeGenericMethod(type)
						.Invoke(provider, new[] { expression, argument });
				}
				else
				{
					return provider.GetType()
						.GetInterfaceMap(typeof(IQbservableProvider))
						.TargetMethods
						.First()
						.MakeGenericMethod(type)
						.Invoke(provider, new[] { expression });
				}
			}
			finally
			{
				PermissionSet.RevertAssert();
			}
		}

		protected abstract Task ClientSendQueryAsync(Expression expression, object argument);

		protected abstract Task<Tuple<Expression, object>> ServerReceiveQueryAsync();

		protected abstract Task ServerSendAsync(NotificationKind kind, object data);

		protected abstract IObservable<TResult> ClientReceive<TResult>();

		protected async Task ShutDownAsync(QbservableProtocolShutDownReason reason)
		{
			ShutDownReason = reason;

			await ShutDownCoreAsync().ConfigureAwait(false);
		}

		protected void ShutDownWithoutResponse(QbservableProtocolShutDownReason reason)
		{
			ShutDownReason = reason;

			CancelAllCommunication();
		}

		protected abstract Task ShutDownCoreAsync();

		protected Task SendAsync(byte[] buffer, int offset, int count)
		{
			return sendQ.EnqueueAsync(async () =>
				{
					try
					{
						await stream.WriteAsync(buffer, offset, count, Cancel).ConfigureAwait(false);
						await stream.FlushAsync(Cancel).ConfigureAwait(false);
						return true;
					}
					catch (ObjectDisposedException ex)		// Occurred sometimes during testing upon cancellation
					{
						throw new OperationCanceledException(ex.Message, ex);
					}
				});
		}

		protected Task<int> ReceiveAsync(byte[] buffer, int offset, int count)
		{
			return receiveQ.EnqueueAsync(async () =>
				{
					try
					{
						int read = await stream.ReadAsync(buffer, offset, count, Cancel).ConfigureAwait(false);

						if (read <= 0)
						{
							throw new InvalidOperationException("The connection was closed without sending all of the data.");
						}

						return read;
					}
					catch (ObjectDisposedException ex)		// Occurred sometimes during testing upon cancellation
					{
						throw new OperationCanceledException(ex.Message, ex);
					}
				});
		}

		internal abstract Task InitializeSinksAsync();

		public byte[] Serialize(object data, out long length)
		{
			using (var memory = new MemoryStream())
			{
				if (data == null)
				{
					memory.WriteByte(1);
				}
				else
				{
					memory.WriteByte(0);

					new SecurityPermission(SecurityPermissionFlag.SerializationFormatter).Assert();

					try
					{
						Formatter.Serialize(memory, data);
					}
					finally
					{
						CodeAccessPermission.RevertAssert();
					}
				}

				length = memory.Length;

				return memory.GetBuffer();
			}
		}

		public T Deserialize<T>(byte[] data)
		{
			return Deserialize<T>(data, offset: 0);
		}

		public T Deserialize<T>(byte[] data, int offset)
		{
			if (data == null || data.Length == 0)
			{
				if (offset > 0)
				{
					throw new InvalidOperationException();
				}

				return (T) (object) null;
			}

			using (var memory = new MemoryStream(data))
			{
				memory.Position = offset;

				var isNullDataFlag = memory.ReadByte();

				Contract.Assume(isNullDataFlag == 0 || isNullDataFlag == 1);

				if (isNullDataFlag == 1)
				{
					return (T) (object) null;
				}
				else
				{
					new SecurityPermission(SecurityPermissionFlag.SerializationFormatter).Assert();

					try
					{
						return (T) Formatter.Deserialize(memory);
					}
					finally
					{
						CodeAccessPermission.RevertAssert();
					}
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "protocolCancellation",
			Justification = "Async usage causes ObjectDisposedExceptions to occur in unexpected places that should simply respect cancellation and stop silently.")]
		protected virtual void Dispose(bool disposing)
		{
			// for derived classes
		}
	}
}