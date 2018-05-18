using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QbservableProvider.Expressions
{
	[Serializable]
	internal sealed class SerializableSwitchExpression : SerializableExpression
	{
		public readonly IList<Tuple<SerializableExpression, IList<SerializableExpression>>> Cases;
		public readonly Tuple<MethodInfo, Type[]> Comparison;
		public readonly SerializableExpression DefaultBody;
		public readonly SerializableExpression SwitchValue;

		public SerializableSwitchExpression(SwitchExpression expression, SerializableExpressionConverter converter)
			: base(expression)
		{
			Cases = expression.Cases.Select(c => Tuple.Create(converter.Convert(c.Body), converter.Convert(c.TestValues))).ToList();
			Comparison = converter.Convert(expression.Comparison);
			DefaultBody = converter.Convert(expression.DefaultBody);
			SwitchValue = converter.Convert(expression.SwitchValue);
		}

		internal override Expression Convert()
		{
			return Expression.Switch(
				Type,
				SwitchValue.TryConvert(),
				DefaultBody.TryConvert(),
				SerializableExpressionConverter.Convert(Comparison),
				Cases.Select(c => Expression.SwitchCase(c.Item1.TryConvert(), c.Item2.TryConvert())));
		}
	}
}