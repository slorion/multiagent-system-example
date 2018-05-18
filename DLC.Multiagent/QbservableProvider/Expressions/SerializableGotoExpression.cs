using System;
using System.Linq.Expressions;

namespace QbservableProvider.Expressions
{
	[Serializable]
	internal sealed class SerializableGotoExpression : SerializableExpression
	{
		public readonly GotoExpressionKind Kind;
		public readonly string TargetName;
		public readonly Type TargetType;
		public readonly SerializableExpression Value;

		public SerializableGotoExpression(GotoExpression expression, SerializableExpressionConverter converter)
			: base(expression)
		{
			Kind = expression.Kind;
			TargetName = expression.Target.Name;
			TargetType = expression.Target.Type;
			Value = converter.Convert(expression.Value);
		}

		internal override Expression Convert()
		{
			return Expression.MakeGoto(
				Kind,
				Expression.Label(TargetType, TargetName),
				Value.TryConvert(),
				Type);
		}
	}
}