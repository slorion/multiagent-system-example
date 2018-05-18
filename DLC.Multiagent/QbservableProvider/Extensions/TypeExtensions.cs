using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;

namespace QbservableProvider
{
	internal static class TypeExtensions
	{
		private static readonly MethodInfo upCastMethod = typeof(TypeExtensions).GetMethod("UpCast", BindingFlags.NonPublic | BindingFlags.Static);

		public static IObservable<object> UpCast(this Type dataType, object observable)
		{
			return (IObservable<object>) upCastMethod.MakeGenericMethod(dataType).Invoke(null, new[] { observable });
		}

		private static IObservable<object> UpCast<TSource>(IObservable<TSource> source)
		{
			return source.Select(value => (object) value);
		}

		public static Type GetGenericInterfaceFromDefinition(this Type type, Type interfaceTypeDefinition)
		{
			Contract.Requires(interfaceTypeDefinition.IsGenericTypeDefinition);

			return type.GetInterfaces()
				.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceTypeDefinition)
				.FirstOrDefault();
		}
	}
}