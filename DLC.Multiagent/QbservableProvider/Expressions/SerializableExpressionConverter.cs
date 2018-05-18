using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QbservableProvider.Expressions
{
	internal sealed class SerializableExpressionConverter
	{
		private const List<Tuple<Tuple<MethodInfo, Type[]>, IList<SerializableExpression>>> noInitializers = null;
		private const SerializableExpression noExpression = null;
		private const IList<object> noRecursion = null;

		private readonly Dictionary<Expression, SerializableExpression> serialized = new Dictionary<Expression, SerializableExpression>();

		private BinaryExpression binary;
		private BlockExpression block;
		private ConditionalExpression conditional;
		private ConstantExpression constant;
		private DefaultExpression @default;
		private GotoExpression @goto;
		private IndexExpression index;
		private InvocationExpression invocation;
		private LabelExpression label;
		private LambdaExpression lambda;
		private ListInitExpression listInit;
		private LoopExpression loop;
		private MemberExpression member;
		private MemberInitExpression memberInit;
		private MethodCallExpression methodCall;
		private NewArrayExpression newArray;
		private NewExpression @new;
		private ParameterExpression parameter;
		private RuntimeVariablesExpression runtimeVariables;
		private SwitchExpression @switch;
		private TryExpression @try;
		private TypeBinaryExpression typeBinary;
		private UnaryExpression unary;

		public IList<TSerializableExpression> Convert<TSerializableExpression>(IEnumerable<Expression> expressions)
			where TSerializableExpression : SerializableExpression
		{
			if (expressions == null)
			{
				return new List<TSerializableExpression>(0);
			}
			else
			{
				return expressions.Select(Convert).Cast<TSerializableExpression>().ToList();
			}
		}

		public IList<SerializableExpression> Convert(IEnumerable<Expression> expressions)
		{
			if (expressions == null)
			{
				return new List<SerializableExpression>(0);
			}
			else
			{
				return expressions.Select(Convert).ToList();
			}
		}

		public TSerializableExpression Convert<TSerializableExpression>(Expression expression)
			where TSerializableExpression : SerializableExpression
		{
			return (TSerializableExpression) Convert(expression);
		}

		public SerializableExpression Convert(Expression expression)
		{
			if (expression == null)
			{
				return null;
			}
			else if (serialized.ContainsKey(expression))
			{
				/* Caching is required to maintain object references during serialization.
				 * See the comments on SerializableExpression.ConvertWithCache for more info.
				 */
				return serialized[expression];
			}
			else if ((methodCall = expression as MethodCallExpression) != null)
			{
				return serialized[expression] = new SerializableMethodCallExpression(methodCall, this);
			}
			else if ((lambda = expression as LambdaExpression) != null)
			{
				return serialized[expression] = new SerializableLambdaExpression(lambda, this);
			}
			else if ((constant = expression as ConstantExpression) != null)
			{
				return serialized[expression] = new SerializableConstantExpression(constant, this);
			}
			else if ((member = expression as MemberExpression) != null)
			{
				return serialized[expression] = new SerializableMemberExpression(member, this);
			}
			else if ((binary = expression as BinaryExpression) != null)
			{
				return serialized[expression] = new SerializableBinaryExpression(binary, this);
			}
			else if ((block = expression as BlockExpression) != null)
			{
				return serialized[expression] = new SerializableBlockExpression(block, this);
			}
			else if ((conditional = expression as ConditionalExpression) != null)
			{
				return serialized[expression] = new SerializableConditionalExpression(conditional, this);
			}
			else if ((@default = expression as DefaultExpression) != null)
			{
				return serialized[expression] = new SerializableDefaultExpression(@default, this);
			}
			else if ((@goto = expression as GotoExpression) != null)
			{
				return serialized[expression] = new SerializableGotoExpression(@goto, this);
			}
			else if ((index = expression as IndexExpression) != null)
			{
				return serialized[expression] = new SerializableIndexExpression(index, this);
			}
			else if ((invocation = expression as InvocationExpression) != null)
			{
				return serialized[expression] = new SerializableInvocationExpression(invocation, this);
			}
			else if ((label = expression as LabelExpression) != null)
			{
				return serialized[expression] = new SerializableLabelExpression(label, this);
			}
			else if ((listInit = expression as ListInitExpression) != null)
			{
				return serialized[expression] = new SerializableListInitExpression(listInit, this);
			}
			else if ((loop = expression as LoopExpression) != null)
			{
				return serialized[expression] = new SerializableLoopExpression(loop, this);
			}
			else if ((memberInit = expression as MemberInitExpression) != null)
			{
				return serialized[expression] = new SerializableMemberInitExpression(memberInit, this);
			}
			else if ((newArray = expression as NewArrayExpression) != null)
			{
				return serialized[expression] = new SerializableNewArrayExpression(newArray, this);
			}
			else if ((@new = expression as NewExpression) != null)
			{
				return serialized[expression] = new SerializableNewExpression(@new, this);
			}
			else if ((parameter = expression as ParameterExpression) != null)
			{
				return serialized[expression] = new SerializableParameterExpression(parameter, this);
			}
			else if ((runtimeVariables = expression as RuntimeVariablesExpression) != null)
			{
				return serialized[expression] = new SerializableRuntimeVariablesExpression(runtimeVariables, this);
			}
			else if ((@switch = expression as SwitchExpression) != null)
			{
				return serialized[expression] = new SerializableSwitchExpression(@switch, this);
			}
			else if ((@try = expression as TryExpression) != null)
			{
				return serialized[expression] = new SerializableTryExpression(@try, this);
			}
			else if ((typeBinary = expression as TypeBinaryExpression) != null)
			{
				return serialized[expression] = new SerializableTypeBinaryExpression(typeBinary, this);
			}
			else if ((unary = expression as UnaryExpression) != null)
			{
				return serialized[expression] = new SerializableUnaryExpression(unary, this);
			}
			else
			{
				throw new ArgumentOutOfRangeException("expression");
			}
		}

		public Expression Convert(SerializableExpression expression)
		{
			return expression.TryConvert();
		}

		// Workaround for a bug deserializing closed generic methods.
		// https://connect.microsoft.com/VisualStudio/feedback/details/736993/bound-generic-methodinfo-throws-argumentnullexception-on-deserialization
		public Tuple<MethodInfo, Type[]> Convert(MethodInfo method)
		{
			if (method != null && method.IsGenericMethod && !method.IsGenericMethodDefinition)
			{
				return Tuple.Create(method.GetGenericMethodDefinition(), method.GetGenericArguments());
			}
			else
			{
				return Tuple.Create(method, (Type[]) null);
			}
		}

		// Workaround for a bug deserializing closed generic methods.
		// https://connect.microsoft.com/VisualStudio/feedback/details/736993/bound-generic-methodinfo-throws-argumentnullexception-on-deserialization
		public static MethodInfo Convert(Tuple<MethodInfo, Type[]> method)
		{
			if (method.Item2 == null)
			{
				return method.Item1;
			}
			else
			{
				return method.Item1.MakeGenericMethod(method.Item2);
			}
		}

		public Tuple<MemberInfo, Type[]> Convert(MemberInfo member)
		{
			var method = member as MethodInfo;

			if (method != null)
			{
				var converted = Convert(method);

				return Tuple.Create((MemberInfo) converted.Item1, converted.Item2);
			}
			else
			{
				return Tuple.Create(member, (Type[]) null);
			}
		}

		public static MemberInfo Convert(Tuple<MemberInfo, Type[]> member)
		{
			var method = member.Item1 as MethodInfo;

			if (method != null)
			{
				return Convert(Tuple.Create(method, member.Item2));
			}
			else
			{
				return member.Item1;
			}
		}

		public IList<Tuple<MemberInfo, Type[]>> Convert(IEnumerable<MemberInfo> members)
		{
			if (members == null)
			{
				return new List<Tuple<MemberInfo, Type[]>>(0);
			}
			else
			{
				return members.Select(Convert).ToList();
			}
		}

		public Tuple<Tuple<MemberInfo, Type[]>, MemberBindingType, SerializableExpression, List<Tuple<Tuple<MethodInfo, Type[]>, IList<SerializableExpression>>>, IList<object>> Convert(MemberBinding binding)
		{
			switch (binding.BindingType)
			{
				case MemberBindingType.Assignment:
					var assign = (MemberAssignment) binding;

					return Tuple.Create(
						Convert(binding.Member),
						binding.BindingType,
						Convert(assign.Expression),
						noInitializers,
						noRecursion);
				case MemberBindingType.ListBinding:
					var list = (MemberListBinding) binding;

					return Tuple.Create(
						Convert(binding.Member),
						binding.BindingType,
						noExpression,
						list.Initializers.Select(i => Tuple.Create(Convert(i.AddMethod), Convert(i.Arguments))).ToList(),
						noRecursion);
				case MemberBindingType.MemberBinding:
					var member = (MemberMemberBinding) binding;

					return Tuple.Create(
						Convert(binding.Member),
						binding.BindingType,
						noExpression,
						noInitializers,
						(IList<object>) member.Bindings.Select(Convert).ToList());
				default:
					throw new InvalidOperationException("Unknown member binding type.");
			}
		}

		public static MemberBinding Convert(Tuple<Tuple<MemberInfo, Type[]>, MemberBindingType, SerializableExpression, List<Tuple<Tuple<MethodInfo, Type[]>, IList<SerializableExpression>>>, IList<object>> data)
		{
			switch (data.Item2)
			{
				case MemberBindingType.Assignment:
					return Expression.Bind(Convert(data.Item1), data.Item3.TryConvert());
				case MemberBindingType.ListBinding:
					return Expression.ListBind(Convert(data.Item1), data.Item4.Select(i => Expression.ElementInit(Convert(i.Item1), i.Item2.TryConvert())));
				case MemberBindingType.MemberBinding:
					return Expression.MemberBind(Convert(data.Item1), data.Item5
						.Cast<Tuple<Tuple<MemberInfo, Type[]>, MemberBindingType, SerializableExpression, List<Tuple<Tuple<MethodInfo, Type[]>, IList<SerializableExpression>>>, IList<object>>>()
						.Select(Convert));
				default:
					throw new InvalidOperationException("Unknown member binding type.");
			}
		}
	}
}