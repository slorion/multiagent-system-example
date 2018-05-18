using System;
using System.Linq.Expressions;
using System.Reflection;

namespace QbservableProvider.Expressions
{
	[Serializable]
	internal sealed class SerializableBinaryExpression : SerializableExpression
	{
		public readonly SerializableLambdaExpression Conversion;
		public readonly bool IsLiftedToNull;
		public readonly SerializableExpression Left;
		public readonly Tuple<MethodInfo, Type[]> Method;
		public readonly SerializableExpression Right;

		public SerializableBinaryExpression(BinaryExpression expression, SerializableExpressionConverter converter)
			: base(expression)
		{
			Conversion = converter.Convert<SerializableLambdaExpression>(expression.Conversion);
			IsLiftedToNull = expression.IsLiftedToNull;
			Left = converter.Convert(expression.Left);
			Method = converter.Convert(expression.Method);
			Right = converter.Convert(expression.Right);
		}

		internal override Expression Convert()
		{
			return Expression.MakeBinary(
				NodeType,
				Left.TryConvert(),
				Right.TryConvert(),
				IsLiftedToNull,
				SerializableExpressionConverter.Convert(Method),
				Conversion.TryConvert<LambdaExpression>());
		}
	}
}