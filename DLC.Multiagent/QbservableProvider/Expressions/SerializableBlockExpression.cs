using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QbservableProvider.Expressions
{
	[Serializable]
	internal sealed class SerializableBlockExpression : SerializableExpression
	{
		public readonly IList<SerializableExpression> Expressions;
		public readonly SerializableExpression Result;
		public readonly IList<SerializableParameterExpression> Variables;

		public SerializableBlockExpression(BlockExpression expression, SerializableExpressionConverter converter)
			: base(expression)
		{
			Expressions = converter.Convert(expression.Expressions);
			Result = converter.Convert(expression.Result);
			Variables = converter.Convert<SerializableParameterExpression>(expression.Variables);
		}

		internal override Expression Convert()
		{
			return Expression.Block(
				Result.Type,
				Variables.TryConvert<ParameterExpression>(),
				Expressions.TryConvert());
		}
	}
}