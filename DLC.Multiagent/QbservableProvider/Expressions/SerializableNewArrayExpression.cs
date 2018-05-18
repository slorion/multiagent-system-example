using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QbservableProvider.Expressions
{
	[Serializable]
	internal sealed class SerializableNewArrayExpression : SerializableExpression
	{
		public readonly IList<SerializableExpression> Expressions;

		public SerializableNewArrayExpression(NewArrayExpression expression, SerializableExpressionConverter converter)
			: base(expression)
		{
			Expressions = converter.Convert(expression.Expressions);
		}

		internal override Expression Convert()
		{
			return Expression.NewArrayInit(
				Type.GetElementType(),
				Expressions.TryConvert());
		}
	}
}