using System;
using System.Collections.Concurrent;
using System.Xml;
using System.Xml.Serialization;

namespace DLC.Scientific.Core.Journalisation
{
	internal static class XmlSerializerCache
	{
		private static readonly ConcurrentDictionary<Type, XmlSerializer> _serializerCache = new ConcurrentDictionary<Type, XmlSerializer>();

		public static readonly XmlSerializerNamespaces BlankNamespace = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });

		public static XmlSerializer GetOrAdd(Type dataType, string rootName = null)
		{
			return _serializerCache.GetOrAdd(dataType, type => new XmlSerializer(type, string.IsNullOrEmpty(rootName) ? null : new XmlRootAttribute(rootName)));
		}
	}
}