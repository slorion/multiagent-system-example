using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QbservableProvider.Expressions
{
	[Serializable]
	internal sealed class SerializableTryExpression : SerializableExpression
	{
		public readonly SerializableExpression Body;
		public readonly SerializableExpression Fault;
		public readonly SerializableExpression Finally;
		public readonly IList<Tuple<SerializableExpression, SerializableExpression, Type, SerializableParameterExpression>> Handlers;

		public SerializableTryExpression(TryExpression expression, SerializableExpressionConverter converter)
			: base(expression)
		{
			Body = converter.Convert(expression.Body);
			Fault = converter.Convert(expression.Fault);
			Finally = converter.Convert(expression.Finally);
			Handlers = expression.Handlers
				.Select(h => Tuple.Create(
					converter.Convert(h.Body),
					converter.Convert(h.Filter),
					h.Test,
					converter.Convert<SerializableParameterExpression>(h.Variable)))
				.ToList();
		}

		internal override Expression Convert()
		{
			return Expression.MakeTry(
				Type,
				Body.TryConvert(),
				Finally.TryConvert(),
				Fault.TryConvert(),
				Handlers.Select(h => Expression.MakeCatchBlock(
					h.Item3,
					h.Item4.TryConvert<ParameterExpression>(),
					h.Item1.TryConvert(),
					h.Item2.TryConvert())));
		}
	}
}