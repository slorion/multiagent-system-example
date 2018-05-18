using System;
using System.Linq.Expressions;

namespace QbservableProvider.Expressions
{
	[Serializable]
	internal sealed class SerializableParameterExpression : SerializableExpression
	{
		public readonly string Name;

		public SerializableParameterExpression(ParameterExpression expression, SerializableExpressionConverter converter)
			: base(expression)
		{
			Name = expression.Name;
		}

		internal override Expression Convert()
		{
			return Expression.Parameter(
				Type,
				Name);
		}
	}
}