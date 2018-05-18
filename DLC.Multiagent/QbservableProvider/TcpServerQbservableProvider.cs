using System;
using System.Linq.Expressions;
using System.Reactive.Linq;

namespace QbservableProvider
{
	internal sealed class TcpServerQbservableProvider<TSource> : IParameterizedQbservableProvider
	{
		public QbservableProtocol Protocol
		{
			get
			{
				return protocol;
			}
		}

		public QbservableServiceOptions Options
		{
			get
			{
				return options;
			}
		}

		private readonly QbservableProtocol protocol;
		private readonly QbservableServiceOptions options;
		private readonly Func<object, IQbservable<TSource>> sourceSelector;

		public TcpServerQbservableProvider(
			QbservableProtocol protocol,
			QbservableServiceOptions options,
			Func<object, IQbservable<TSource>> sourceSelector)
		{
			this.protocol = protocol;
			this.options = options;
			this.sourceSelector = sourceSelector;
		}

		public IQbservable<TSource> GetSource(object argument)
		{
			return sourceSelector(argument);
		}

		public IQbservable<TResult> CreateQuery<TResult>(Expression expression)
		{
			return new TcpServerQuery<TSource, TResult>(this, expression, null);
		}

		public IQbservable<TResult> CreateQuery<TResult>(Expression expression, object argument)
		{
			return new TcpServerQuery<TSource, TResult>(this, expression, argument);
		}
	}
}