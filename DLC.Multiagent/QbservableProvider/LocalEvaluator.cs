using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace QbservableProvider
{
	public abstract class LocalEvaluator : LocalEvaluationContext
	{
		protected LocalEvaluator(params Type[] knownTypes)
			: base(knownTypes)
		{
		}

		public Expression EvaluateCompilerGenerated(MemberExpression member, QbservableProtocol protocol)
		{
			var closure = member.Expression as ConstantExpression;

			if (closure == null)
			{
				return null;
			}

			var instance = closure.Value;

			object value;
			Type type;

			var field = member.Member as FieldInfo;

			if (field != null)
			{
				type = field.FieldType;
				value = field.GetValue(instance);
			}
			else
			{
				var property = (PropertyInfo) member.Member;

				type = property.PropertyType;
				value = property.GetValue(instance);
			}

			var result = TryEvaluateSequences(value, type, protocol);

			return result == null
				? Expression.Constant(value, type)
				: result.IsLeft
					? Expression.Constant(result.Left, type)
					: result.Right;
		}

		internal Expression GetValue(MemberExpression member, ExpressionVisitor visitor, QbservableProtocol protocol)
		{
			var property = member.Member as PropertyInfo;

			if (property != null)
			{
				return GetValue(property, member, visitor, protocol);
			}
			else
			{
				return GetValue((FieldInfo) member.Member, member, visitor, protocol);
			}
		}

		public abstract Expression GetValue(PropertyInfo property, MemberExpression member, ExpressionVisitor visitor, QbservableProtocol protocol);

		public abstract Expression GetValue(FieldInfo field, MemberExpression member, ExpressionVisitor visitor, QbservableProtocol protocol);

		public abstract Expression Invoke(MethodCallExpression call, ExpressionVisitor visitor, QbservableProtocol protocol);

		protected Either<object, Expression> TryEvaluateSequences(object value, Type type, QbservableProtocol protocol)
		{
			if (value != null)
			{
				var isSequence = type == typeof(IEnumerable)
											|| type.IsGenericType &&
													 (type.GetGenericTypeDefinition() == typeof(IEnumerable<>)
												 || type.GetGenericTypeDefinition() == typeof(IObservable<>));

				if (isSequence || !IsTypeKnown(value))
				{
					var result = TryEvaluateEnumerable(value, type, protocol);

					if (result != null)
					{
						return result;
					}
					else
					{
						var expression = TryEvaluateObservable(value, type, protocol);

						if (expression != null)
						{
							return Either.Right<object, Expression>(expression);
						}
					}
				}
			}

			return null;
		}

		protected abstract Either<object, Expression> TryEvaluateEnumerable(object value, Type type, QbservableProtocol protocol);

		protected abstract Expression TryEvaluateObservable(object value, Type type, QbservableProtocol protocol);
	}
}