using Newtonsoft.Json;
using System;
using System.Net;

namespace DLC.Scientific.Core.Configuration.Converters
{
	internal class IPAddressConverter
		: JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(IPAddress);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return IPAddress.Parse((string) reader.Value);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteRawValue(value == null ? "" : value.ToString());
		}
	}
}