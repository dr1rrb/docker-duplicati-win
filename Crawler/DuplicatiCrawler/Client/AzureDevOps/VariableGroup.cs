using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Crawler.Client.AzureDevOps
{
	public class VariableGroup
	{
		[JsonPropertyName("id")]
		public uint Id { get; set; }

		[JsonPropertyName("name")]
		public string Name { get; set; }

		[JsonPropertyName("variables")]
		public Dictionary<string, Variable> Variables { get; set; }

		// Yes its ugly ... it a mutable entity
		[JsonIgnore]
		public string this[string key] => Variables[Name.Replace('-', '.') + '.' + key].Value;

		public bool TryUpdate(string key, string value)
		{
			var variable = Variables[Name.Replace('-', '.') + '.' + key];

			if (variable.Value.Equals(value, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			else
			{
				variable.Value = value;
				return true;
			}
		}
	}
}