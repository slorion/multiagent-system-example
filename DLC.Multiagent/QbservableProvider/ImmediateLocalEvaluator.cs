using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using QbservableProvider.Properties;

namespace QbservableProvider
{
	public class ImmediateLocalEvaluator : DuplexLocalEvaluator
	{
		public ImmediateLocalEvaluator(params Type[] knownTypes)
			: base(knownTypes)
		{
		}

		public override Expression GetValue(PropertyInfo property, MemberExpression member, ExpressionVisitor visitor, QbservableProtocol protocol)
		{
			object instance = Evaluate(member.Expression, visitor, Errors.ExpressionMemberMissingLocalInstanceFormat, member.Member);

			var value = property.GetValue(instance);

			var either = TryEvaluateSequences(value, property.PropertyType, protocol);

			return either == null
				? Expression.Constant(value, property.PropertyType)
				: either.IsLeft
					? Expression.Constant(either.Left, property.PropertyType)
					: either.Right;
		}

		public override Expression GetValue(FieldInfo field, MemberExpression member, ExpressionVisitor visitor, QbservableProtocol protocol)
		{
			object instance = Evaluate(member.Expression, visitor, Errors.ExpressionMemberMissingLocalInstanceFormat, member.Member);

			var value = field.GetValue(instance);

			var either = TryEvaluateSequences(value, field.FieldType, protocol);

			return either == null
				? Expression.Constant(value, field.FieldType)
				: either.IsLeft
					? Expression.Constant(either.Left, field.FieldType)
					: either.Right;
		}

		public override Expression Invoke(MethodCallExpression call, ExpressionVisitor visitor, QbservableProtocol protocol)
		{
			if (call.Method.ReturnType == typeof(void))
			{
				throw new InvalidOperationException(Errors.ExpressionCallLocalVoidFormat);
			}

			object instance = Evaluate(call.Object, visitor, Errors.ExpressionCallMissingLocalInstanceFormat, call.Method);

			var result = call.Method.Invoke(instance, EvaluateArguments(call, visitor).ToArray());

			var either = TryEvaluateSequences(result, call.Type, protocol);

			return either == null
					? Expression.Constant(result, call.Type)
					: either.IsLeft
						? Expression.Constant(either.Left, call.Type)
						: either.Right;
		}

		private static object[] EvaluateArguments(MethodCallExpression call, ExpressionVisitor visitor)
		{
			if (call.Arguments == null)
			{
				return null;
			}
			else
			{
				return call.Arguments
					.Select(e => Evaluate(e, visitor, Errors.ExpressionCallMissingLocalArgumentFormat, call.Method))
					.ToArray();
			}
		}

		protected override Either<object, Expression> TryEvaluateEnumerable(object value, Type type, QbservableProtocol protocol)
		{
			var iterator = value as IEnumerable;

			if (iterator != null)
			{
				var iteratorType = iterator.GetType();

				if (iteratorType.GetCustomAttribute<CompilerGeneratedAttribute>(true) != null
					|| (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
					|| type == typeof(IEnumerable))
				{
					value = EvaluateIterator(iterator);

					return Either.Left<object, Expression>(value);
				}
			}

			return null;
		}

		private static object EvaluateIterator(IEnumerable iterator)
		{
			var genericIterator = iterator.GetType().GetGenericInterfaceFromDefinition(typeof(IEnumerable<>));

			var dataType = genericIterator == null ? typeof(object) : genericIterator.GetGenericArguments()[0];

			var list = Activator.CreateInstance(typeof(List<>).MakeGenericType(dataType));

			var add = list.GetType().GetMethod("Add");

			foreach (var item in iterator)
			{
				add.Invoke(list, new[] { item });
			}

			return list;
		}
	}
}