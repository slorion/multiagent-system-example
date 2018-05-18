using System;
using System.Linq.Expressions;
using System.Net;
using System.Reactive.Linq;
using System.Runtime.Remoting.Messaging;

namespace QbservableProvider
{
	internal sealed class TcpClientQbservableProvider : IQbservableProvider
	{
		public Type SourceType
		{
			get
			{
				return sourceType;
			}
		}

		public IPEndPoint EndPoint
		{
			get
			{
				return endPoint;
			}
		}

		public IRemotingFormatter Formatter
		{
			get
			{
				return formatter;
			}
		}

		public LocalEvaluator LocalEvaluator
		{
			get
			{
				return localEvaluator;
			}
		}

		private readonly Type sourceType;
		private readonly IPEndPoint endPoint;
		private readonly IRemotingFormatter formatter;
		private readonly LocalEvaluator localEvaluator;
		private readonly object argument;

		public TcpClientQbservableProvider(Type sourceType, IPEndPoint endPoint, IRemotingFormatter formatter, LocalEvaluator localEvaluator)
		{
			this.sourceType = sourceType;
			this.endPoint = endPoint;
			this.formatter = formatter;
			this.localEvaluator = localEvaluator;
		}

		public TcpClientQbservableProvider(Type sourceType, IPEndPoint endPoint, IRemotingFormatter formatter, LocalEvaluator localEvaluator, object argument)
			: this(sourceType, endPoint, formatter, localEvaluator)
		{
			this.argument = argument;
		}

		internal IQbservable<TResult> CreateQuery<TResult>()
		{
			return new TcpClientQuery<TResult>(this, argument);
		}

		public IQbservable<TResult> CreateQuery<TResult>(Expression expression)
		{
			return new TcpClientQuery<TResult>(this, argument, expression);
		}
	}
}