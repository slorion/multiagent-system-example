using System;
using System.Linq.Expressions;

namespace QbservableProvider.Expressions
{
	[Serializable]
	internal sealed class SerializableConstantExpression : SerializableExpression
	{
		public readonly object Value;

		public SerializableConstantExpression(ConstantExpression expression, SerializableExpressionConverter converter)
			: base(expression)
		{
			Value = expression.Value;
		}

		internal override Expression Convert()
		{
			return Expression.Constant(Value, Type);
		}
	}
}