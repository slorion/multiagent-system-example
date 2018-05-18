using System;
using System.Linq.Expressions;

namespace QbservableProvider.Expressions
{
	[Serializable]
	internal sealed class SerializableLabelExpression : SerializableExpression
	{
		public readonly SerializableExpression DefaultValue;
		public readonly string TargetName;
		public readonly Type TargetType;

		public SerializableLabelExpression(LabelExpression expression, SerializableExpressionConverter converter)
			: base(expression)
		{
			DefaultValue = converter.Convert(expression.DefaultValue);
			TargetName = expression.Target.Name;
			TargetType = expression.Target.Type;
		}

		internal override Expression Convert()
		{
			return Expression.Label(
				Expression.Label(TargetType, TargetName),
				DefaultValue.TryConvert());
		}
	}
}