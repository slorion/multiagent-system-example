using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QbservableProvider.Expressions
{
	[Serializable]
	internal sealed class SerializableListInitExpression : SerializableExpression
	{
		public readonly IList<Tuple<Tuple<MethodInfo, Type[]>, IList<SerializableExpression>>> Initializers;
		public readonly SerializableNewExpression NewExpression;

		public SerializableListInitExpression(ListInitExpression expression, SerializableExpressionConverter converter)
			: base(expression)
		{
			Initializers = expression.Initializers.Select(i => Tuple.Create(converter.Convert(i.AddMethod), converter.Convert(i.Arguments))).ToList();
			NewExpression = converter.Convert<SerializableNewExpression>(expression.NewExpression);
		}

		internal override Expression Convert()
		{
			return Expression.ListInit(
				NewExpression.TryConvert<NewExpression>(),
				Initializers.Select(i => Expression.ElementInit(SerializableExpressionConverter.Convert(i.Item1), i.Item2.TryConvert())));
		}
	}
}