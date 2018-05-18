using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace QbservableProvider.Expressions
{
	[Serializable]
	internal sealed class SerializableIndexExpression : SerializableExpression
	{
		public readonly IList<SerializableExpression> Arguments;
		public readonly PropertyInfo Indexer;
		public readonly SerializableExpression Object;

		public SerializableIndexExpression(IndexExpression expression, SerializableExpressionConverter converter)
			: base(expression)
		{
			Arguments = converter.Convert(expression.Arguments);
			Indexer = expression.Indexer;
			Object = converter.Convert(expression.Object);
		}

		internal override Expression Convert()
		{
			return Expression.MakeIndex(
				Object.TryConvert(),
				Indexer,
				Arguments.TryConvert());
		}
	}
}