using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace QbservableProvider
{
	[Serializable]
	public class KnownTypeContext
	{
		private readonly HashSet<Assembly> knownAssemblies;
		private readonly HashSet<Type> knownTypes;

		public KnownTypeContext(params Type[] knownTypes)
			: this((IEnumerable<Type>) knownTypes)
		{
			Contract.Requires(Contract.ForAll(knownTypes, type => !type.IsGenericType || type.IsGenericTypeDefinition));
		}

		public KnownTypeContext(IEnumerable<Type> knownTypes)
			: this(null, knownTypes)
		{
			Contract.Requires(Contract.ForAll(knownTypes, type => !type.IsGenericType || type.IsGenericTypeDefinition));
		}

		public KnownTypeContext(IEnumerable<Assembly> knownAssemblies, params Type[] additionalKnownTypes)
			: this(knownAssemblies, (IEnumerable<Type>) additionalKnownTypes)
		{
			Contract.Requires(Contract.ForAll(additionalKnownTypes, type => !type.IsGenericType || type.IsGenericTypeDefinition));
		}

		public KnownTypeContext(IEnumerable<Assembly> knownAssemblies, IEnumerable<Type> additionalKnownTypes)
		{
			Contract.Requires(Contract.ForAll(additionalKnownTypes, type => !type.IsGenericType || type.IsGenericTypeDefinition));

			this.knownAssemblies = new HashSet<Assembly>(knownAssemblies ?? Enumerable.Empty<Assembly>());
			this.knownTypes = new HashSet<Type>(additionalKnownTypes ?? Enumerable.Empty<Type>());
		}

		public bool IsTypeInKnownAssembly(Type type)
		{
			return knownAssemblies.Contains(type.Assembly);
		}

		public bool IsTypeKnown(object value)
		{
			return value == null || IsKnownType(value.GetType());
		}

		public virtual bool IsKnownType(Type type)
		{
			return type == null
					|| type.IsPrimitive
					|| type.IsArray && IsKnownType(type.GetElementType())
					|| IsTypeInKnownAssembly(type)
					|| knownTypes.Contains(type.IsGenericType ? type.GetGenericTypeDefinition() : type);
		}

		public void AddKnownType(Type type)
		{
			knownTypes.Add(type);
		}
	}
}