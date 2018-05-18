using DLC.Scientific.Core.Configuration.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace DLC.Scientific.Core.Configuration
{
	public class ConfigurationFactory
	{
		private static readonly JsonConverter[] _jsonConverters = new[] { new IPAddressConverter() };

		public RootConfiguration<TAgent, TModule> LoadFromFile<TAgent, TModule>(string path)
			where TAgent : AgentConfiguration
			where TModule : ModuleConfiguration
		{
			if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
			return Load<TAgent, TModule>(File.ReadAllText(path));
		}

		public RootConfiguration<TAgent, TModule> LoadFromReader<TAgent, TModule>(StreamReader reader)
			where TAgent : AgentConfiguration
			where TModule : ModuleConfiguration
		{
			if (reader == null) throw new ArgumentNullException("reader");
			return Load<TAgent, TModule>(reader.ReadToEnd());
		}

		public RootConfiguration<TAgent, TModule> Load<TAgent, TModule>(string configuration)
			where TAgent : AgentConfiguration
			where TModule : ModuleConfiguration
		{
			if (string.IsNullOrEmpty("configuration")) throw new ArgumentNullException("configuration");

			var serializer = new JsonSerializer();
			foreach (var converter in _jsonConverters)
				serializer.Converters.Add(converter);

			var jRoot = (JObject) JsonConvert.DeserializeObject(configuration);
			var root = jRoot.ToObject<RootConfiguration<TAgent, TModule>>(serializer);

			LoadCore<TAgent, TModule>(root, jRoot, serializer);

			root.Validate();

			return root;
		}

		protected virtual void LoadCore<TAgent, TModule>(RootConfiguration<TAgent, TModule> root, JObject jRoot, JsonSerializer serializer)
			where TAgent : AgentConfiguration
			where TModule : ModuleConfiguration
		{
		}

		public void Update(string path, string propertyJsonPath, object value)
		{
			if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
			if (string.IsNullOrEmpty(propertyJsonPath)) throw new ArgumentNullException("propertyJsonPath");

			string result;

			using (var input = new StreamReader(path))
			using (var output = new StringWriter())
			using (var reader = new JsonTextReader(input))
			using (var writer = new JsonTextWriter(output))
			{
				writer.Formatting = Formatting.Indented;

				while (reader.Path != propertyJsonPath && reader.Read())
					writer.WriteToken(reader, false);

				if (!reader.Read())
					throw new ArgumentException(string.Format("Le path JSON '{0}' est invalide.", propertyJsonPath), "propertyJsonPath");

				writer.WriteRawValue(JsonConvert.SerializeObject(value, _jsonConverters));

				if (reader.TokenType == JsonToken.StartArray)
					while (reader.TokenType != JsonToken.EndArray && reader.Read()) { }
				else if (reader.TokenType == JsonToken.StartObject)
					while (reader.TokenType != JsonToken.EndObject && reader.Read()) { }

				while (reader.Read())
					writer.WriteToken(reader);

				result = output.ToString();
			}

			File.WriteAllText(path, result);
		}
	}
}