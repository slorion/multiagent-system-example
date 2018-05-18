using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QbservableProvider
{
	[Serializable]
	internal sealed class CompilerGenerated
	{
		private static readonly ConstructorInfo constructor = typeof(CompilerGenerated).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(KeyValuePair<string, object>[]) }, null);
		private static readonly ConstructorInfo propertyConstructor = typeof(KeyValuePair<string, object>).GetConstructor(new[] { typeof(string), typeof(object) });
		private static readonly MethodInfo getPropertyMethod = typeof(CompilerGenerated).GetMethods().Where(m => m.Name == "GetProperty").First();
		private static readonly MethodInfo setPropertyMethod = typeof(CompilerGenerated).GetMethod("SetProperty");

		private readonly Dictionary<string, object> properties = new Dictionary<string, object>();

		public CompilerGenerated()
		{
		}

		private CompilerGenerated(KeyValuePair<string, object>[] properties)
		{
			foreach (var property in properties)
			{
				this.properties.Add(property.Key, property.Value);
			}
		}

		public static NewExpression New(IEnumerable<MemberInfo> members, IEnumerable<Expression> arguments)
		{
			return Expression.New(
				constructor,
				Expression.NewArrayInit(
					typeof(KeyValuePair<string, object>),
					members.Zip(arguments, (property, argument) =>
						Expression.New(propertyConstructor,
							Expression.Constant(property.Name),
							Expression.Convert(argument, typeof(object))))));
		}

		public static MethodCallExpression Get(Expression instance, MemberInfo member, Func<Type, Type> updateGenericTypeArguments)
		{
			string name;
			Type type;

			var property = member as PropertyInfo;

			if (property != null)
			{
				name = property.Name;
				type = updateGenericTypeArguments(property.PropertyType);
			}
			else
			{
				var field = (FieldInfo) member;

				name = field.Name;
				type = updateGenericTypeArguments(field.FieldType);
			}

			return Expression.Call(instance, getPropertyMethod.MakeGenericMethod(type), Expression.Constant(name));
		}

		public static MethodCallExpression Set(Expression left, Expression right)
		{
			var member = (MemberExpression) left;
			var property = (PropertyInfo) member.Member;

			return Expression.Call(member, setPropertyMethod, Expression.Constant(property.Name), right);
		}

		public T GetProperty<T>(string name)
		{
			return (T) properties[name];
		}

		public void SetProperty(string name, object value)
		{
			properties[name] = value;
		}
	}
}