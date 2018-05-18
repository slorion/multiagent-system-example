using System;
using System.Linq.Expressions;
using System.Reflection;

namespace QbservableProvider.Expressions
{
	[Serializable]
	internal sealed class SerializableMemberExpression : SerializableExpression
	{
		public readonly Tuple<MemberInfo, Type[]> Member;
		public readonly SerializableExpression Expr;

		public SerializableMemberExpression(MemberExpression expression, SerializableExpressionConverter converter)
			: base(expression)
		{
			Expr = converter.Convert(expression.Expression);
			Member = converter.Convert(expression.Member);
		}

		internal override Expression Convert()
		{
			return Expression.MakeMemberAccess(
				Expr.TryConvert(),
				SerializableExpressionConverter.Convert(Member));
		}
	}
}