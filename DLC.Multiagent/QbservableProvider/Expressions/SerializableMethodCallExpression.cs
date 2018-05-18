using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace QbservableProvider.Expressions
{
	[Serializable]
	internal sealed class SerializableMethodCallExpression : SerializableExpression
	{
		public readonly IList<SerializableExpression> Arguments;
		public readonly Tuple<MethodInfo, Type[]> Method;
		public readonly SerializableExpression Object;

		public SerializableMethodCallExpression(MethodCallExpression expression, SerializableExpressionConverter converter)
			: base(expression)
		{
			Arguments = converter.Convert(expression.Arguments);
			Method = converter.Convert(expression.Method);
			Object = converter.Convert(expression.Object);
		}

		internal override Expression Convert()
		{
			return Expression.Call(
				Object.TryConvert(),
				SerializableExpressionConverter.Convert(Method),
				Arguments.TryConvert());
		}
	}
}