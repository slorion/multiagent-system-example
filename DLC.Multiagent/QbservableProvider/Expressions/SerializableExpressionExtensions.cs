using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QbservableProvider.Expressions
{
	internal static class SerializableExpressionExtensions
	{
		public static TExpression TryConvert<TExpression>(this SerializableExpression expression)
			where TExpression : Expression
		{
			return (TExpression) expression.TryConvert();
		}

		public static Expression TryConvert(this SerializableExpression expression)
		{
			if (expression == null)
			{
				return null;
			}
			else
			{
				return expression.ConvertWithCache();
			}
		}

		public static IEnumerable<TExpression> TryConvert<TExpression>(this IEnumerable<SerializableExpression> expressions)
			where TExpression : Expression
		{
			return expressions.TryConvert().Cast<TExpression>();
		}

		public static IEnumerable<Expression> TryConvert(this IEnumerable<SerializableExpression> expressions)
		{
			if (expressions == null)
			{
				return null;
			}
			else
			{
				return expressions.Select(e => e.TryConvert());
			}
		}
	}
}