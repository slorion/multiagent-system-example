using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QbservableProvider.Expressions
{
	[Serializable]
	internal sealed class SerializableMemberInitExpression : SerializableExpression
	{
		public readonly IList<Tuple<Tuple<MemberInfo, Type[]>, MemberBindingType, SerializableExpression, List<Tuple<Tuple<MethodInfo, Type[]>, IList<SerializableExpression>>>, IList<object>>> Bindings;
		public readonly SerializableNewExpression NewExpression;

		public SerializableMemberInitExpression(MemberInitExpression expression, SerializableExpressionConverter converter)
			: base(expression)
		{
			Bindings = expression.Bindings.Select(converter.Convert).ToList();
			NewExpression = converter.Convert<SerializableNewExpression>(expression.NewExpression);
		}

		internal override Expression Convert()
		{
			return Expression.MemberInit(
				NewExpression.TryConvert<NewExpression>(),
				Bindings.Select(SerializableExpressionConverter.Convert));
		}
	}
}