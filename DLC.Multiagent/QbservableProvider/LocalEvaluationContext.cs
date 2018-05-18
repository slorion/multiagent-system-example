using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using QbservableProvider.Properties;

namespace QbservableProvider
{
	public class LocalEvaluationContext : KnownTypeContext
	{
		private static readonly IEnumerable<Assembly> defaultKnownAssemblies = new List<Assembly>()
			{
				typeof(LocalEvaluationContext).Assembly,
				typeof(int).Assembly, 
				typeof(System.Uri).Assembly, 
				typeof(System.Data.DataSet).Assembly, 
				typeof(System.Xml.XmlReader).Assembly, 
				typeof(System.Xml.Linq.XElement).Assembly,
				typeof(System.Linq.Enumerable).Assembly, 
				typeof(System.Reactive.Linq.Observable).Assembly, 
				typeof(System.Reactive.Linq.Qbservable).Assembly, 
				typeof(System.Reactive.Notification).Assembly, 
				typeof(System.Reactive.IEventPattern<,>).Assembly,
				typeof(System.Reactive.Concurrency.TaskPoolScheduler).Assembly, 
				typeof(Rxx.Parsers.Linq.Parser).Assembly
			}
			.AsReadOnly();

		internal IDictionary<string, ParameterExpression> ReplacedParameters
		{
			get
			{
				return replacedParameters;
			}
		}

		private readonly Dictionary<string, ParameterExpression> replacedParameters = new Dictionary<string, ParameterExpression>();

		public LocalEvaluationContext(params Type[] knownTypes)
			: base(defaultKnownAssemblies, knownTypes)
		{
		}

		internal bool EnsureKnownTypes(IEnumerable<MemberInfo> members)
		{
			var processedAny = false;

			if (members != null)
			{
				foreach (var type in members.Select(member => member.DeclaringType).Distinct())
				{
					processedAny |= EnsureKnownType(type);
				}
			}

			return processedAny;
		}

		internal bool EnsureKnownType(MemberInfo member, Action<Type> replaceCompilerGeneratedType = null, Action<Type, Type> unknownType = null, Action<Type> genericArgumentsUpdated = null)
		{
			if (member != null)
			{
				return EnsureKnownType(member.DeclaringType, replaceCompilerGeneratedType, unknownType, genericArgumentsUpdated);
			}
			else
			{
				return false;		// return value indicates whether the type has been processed, not whether it's serializable.
			}
		}

		internal bool EnsureKnownType(MethodInfo method, Action<Type> replaceCompilerGeneratedType = null, Action<Type, Type> unknownType = null, Action<MethodInfo> genericMethodArgumentsUpdated = null)
		{
			if (method != null)
			{
				return EnsureKnownType(method.DeclaringType, replaceCompilerGeneratedType, unknownType)
						|| EnsureGenericTypeArgumentsSerializable(method, genericMethodArgumentsUpdated);
			}
			else
			{
				return false;		// return value indicates whether the type has been processed, not whether it's serializable.
			}
		}

		internal bool EnsureKnownType(LabelTarget target, Action<Type> replaceCompilerGeneratedType = null, Action<Type, Type> unknownType = null, Action<Type> genericArgumentsUpdated = null)
		{
			if (target != null)
			{
				return EnsureKnownType(target.Type, replaceCompilerGeneratedType, unknownType, genericArgumentsUpdated);
			}
			else
			{
				return false;		// return value indicates whether the type has been processed, not whether it's serializable.
			}
		}

		internal bool EnsureKnownType(Type type, Action<Type> replaceCompilerGeneratedType = null, Action<Type, Type> unknownType = null, Action<Type> genericArgumentsUpdated = null)
		{
			if (type == null)
			{
				return false;		// return value indicates whether the type has been processed, not whether it's serializable.
			}

			return EnsureCompilerGeneratedTypeIsReplaced(type, replaceCompilerGeneratedType)
					|| EnsureKnownTypeHierarchy(type, unknownType)
					|| EnsureGenericTypeArgumentsSerializable(type, genericArgumentsUpdated);
		}

		private bool EnsureCompilerGeneratedTypeIsReplaced(Type type, Action<Type> replaceCompilerGeneratedType = null)
		{
			if (type == typeof(CompilerGenerated))
			{
				throw new InvalidOperationException(Errors.ExpressionVisitedCompilerTypeTwice);
			}

			if (type.GetCustomAttribute<CompilerGeneratedAttribute>(inherit: true) != null)
			{
				if (replaceCompilerGeneratedType != null)
				{
					replaceCompilerGeneratedType(type);
				}
				else
				{
					throw new InvalidOperationException(Errors.ExpressionUnsupportedCompilerType);
				}

				return true;
			}

			return false;
		}

		private bool EnsureKnownTypeHierarchy(Type type, Action<Type, Type> unknownType = null)
		{
			Type current = type;

			do
			{
				if (!IsKnownType(current))
				{
					if (unknownType != null)
					{
						unknownType(current, type);

						return true;
					}
					else if (current == type)
					{
						throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Errors.ExpressionUnknownType, type.FullName));
					}
					else
					{
						throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Errors.ExpressionUnknownBaseType, type.FullName, current.FullName));
					}
				}
			}
			while ((type = type.DeclaringType) != null);

			return false;
		}

		private bool EnsureGenericTypeArgumentsSerializable(Type type, Action<Type> genericArgumentsUpdated = null)
		{
			if (type.IsGenericType)
			{
				if (EnsureGenericTypeArgumentsSerializable(ref type))
				{
					if (genericArgumentsUpdated != null)
					{
						genericArgumentsUpdated(type);
					}
					else
					{
						throw new InvalidOperationException(Errors.ExpressionUnsupportedCompilerTypeAsTypeArg);
					}

					return true;
				}
			}

			return false;
		}

		private bool EnsureGenericTypeArgumentsSerializable(MethodInfo method, Action<MethodInfo> genericArgumentsUpdated = null)
		{
			if (method.IsGenericMethod)
			{
				if (EnsureGenericTypeArgumentsSerializable(ref method))
				{
					if (genericArgumentsUpdated != null)
					{
						genericArgumentsUpdated(method);
					}
					else
					{
						throw new InvalidOperationException(Errors.ExpressionUnsupportedCompilerTypeAsMethodTypeArg);
					}

					return true;
				}
			}

			return false;
		}

		private bool EnsureGenericTypeArgumentsSerializable(ref Type type)
		{
			var genericTypeArguments = type.GetGenericArguments();

			Type oldType = type;
			Type newType = null;

			EnsureGenericTypeArgumentsSerializable(
				genericTypeArguments,
				() => newType = oldType.GetGenericTypeDefinition().MakeGenericType(genericTypeArguments));

			if (newType != null)
			{
				type = newType;

				return true;
			}

			return false;
		}

		private bool EnsureGenericTypeArgumentsSerializable(ref MethodInfo method)
		{
			var genericTypeArguments = method.GetGenericArguments();

			MethodInfo oldMethodInfo = method;
			MethodInfo newMethodInfo = null;

			EnsureGenericTypeArgumentsSerializable(
				genericTypeArguments,
				() => newMethodInfo = oldMethodInfo.GetGenericMethodDefinition().MakeGenericMethod(genericTypeArguments));

			if (newMethodInfo != null)
			{
				method = newMethodInfo;

				return true;
			}

			return false;
		}

		private void EnsureGenericTypeArgumentsSerializable(IList<Type> genericTypeArguments, Action updateType)
		{
			var replacedAny = false;

			for (int i = 0; i < genericTypeArguments.Count; i++)
			{
				var argument = genericTypeArguments[i];

				var replaced = EnsureKnownType(
					argument,
					replaceCompilerGeneratedType: _ =>
					{
						genericTypeArguments[i] = typeof(CompilerGenerated);
						replacedAny = true;
					});

				if (!replaced && argument.IsGenericType && EnsureGenericTypeArgumentsSerializable(ref argument))
				{
					genericTypeArguments[i] = argument;
					replacedAny = true;
				}
			}

			if (replacedAny)
			{
				updateType();
			}
		}
	}
}