using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QbservableProvider.Expressions
{
	[Serializable]
	internal sealed class SerializableLambdaExpression : SerializableExpression
	{
		public readonly SerializableExpression Body;
		public readonly string Name;
		public readonly IList<SerializableParameterExpression> Parameters;
		public readonly bool TailCall;

		public SerializableLambdaExpression(LambdaExpression expression, SerializableExpressionConverter converter)
			: base(expression)
		{
			Body = converter.Convert(expression.Body);
			Name = expression.Name;
			Parameters = converter.Convert<SerializableParameterExpression>(expression.Parameters);
			TailCall = expression.TailCall;
		}

		internal override Expression Convert()
		{
			return Expression.Lambda(
				Type,
				Body.TryConvert(),
				Name,
				TailCall,
				Parameters.TryConvert<ParameterExpression>());
		}
	}
}