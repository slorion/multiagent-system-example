using System;
using System.Linq.Expressions;

namespace QbservableProvider.Expressions
{
	[Serializable]
	internal sealed class SerializableConditionalExpression : SerializableExpression
	{
		public readonly SerializableExpression IfFalse;
		public readonly SerializableExpression IfTrue;
		public readonly SerializableExpression Test;

		public SerializableConditionalExpression(ConditionalExpression expression, SerializableExpressionConverter converter)
			: base(expression)
		{
			IfFalse = converter.Convert(expression.IfFalse);
			IfTrue = converter.Convert(expression.IfTrue);
			Test = converter.Convert(expression.Test);
		}

		internal override Expression Convert()
		{
			return Expression.Condition(
				Test.TryConvert(),
				IfTrue.TryConvert(),
				IfFalse.TryConvert(),
				Type);
		}
	}
}