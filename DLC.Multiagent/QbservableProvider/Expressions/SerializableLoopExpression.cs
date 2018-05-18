using System;
using System.Linq.Expressions;

namespace QbservableProvider.Expressions
{
	[Serializable]
	internal sealed class SerializableLoopExpression : SerializableExpression
	{
		public readonly SerializableExpression Body;
		public readonly string BreakLabelName;
		public readonly Type BreakLabelType;
		public readonly string ContinueLabelName;
		public readonly Type ContinueLabelType;

		public SerializableLoopExpression(LoopExpression expression, SerializableExpressionConverter converter)
			: base(expression)
		{
			Body = converter.Convert(expression.Body);
			BreakLabelType = expression.BreakLabel.Type;
			BreakLabelName = expression.BreakLabel.Name;
			ContinueLabelType = expression.ContinueLabel.Type;
			ContinueLabelName = expression.ContinueLabel.Name;
		}

		internal override Expression Convert()
		{
			return Expression.Loop(
				Body.TryConvert(),
				Expression.Label(BreakLabelType, BreakLabelName),
				Expression.Label(ContinueLabelType, ContinueLabelName));
		}
	}
}