using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;

namespace QbservableProvider
{
	public abstract class QbservableBase<TData, TProvider> : ObservableBase<TData>, IQbservable<TData>
		where TProvider : IQbservableProvider
	{
		public Type ElementType
		{
			get
			{
				return elementType;
			}
		}

		public Expression Expression
		{
			get
			{
				return expression;
			}
		}

		public TProvider Provider
		{
			get
			{
				return provider;
			}
		}

		IQbservableProvider IQbservable.Provider
		{
			get
			{
				return Provider;
			}
		}

		private static readonly Type elementType = typeof(TData);
		private readonly TProvider provider;
		private readonly Expression expression;

		protected QbservableBase(TProvider provider)
		{
			this.provider = provider;
			this.expression = Expression.Constant(this);
		}

		[SuppressMessage("Microsoft.Contracts", "RequiresAtCall-typeof(IQbservable<TData>).IsAssignableFrom(expression.Type)")]
		protected QbservableBase(TProvider provider, Expression expression)
		{
			Contract.Requires(provider != null);
			Contract.Requires(expression != null);
			Contract.Requires(typeof(IQbservable<TData>).IsAssignableFrom(expression.Type));

			this.provider = provider;
			this.expression = expression;
		}

		protected bool IsSource(Expression expression)
		{
			var constant = expression as ConstantExpression;

			return constant != null && constant.Value == this;
		}
	}
}