using System;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Crawler.Client.AzureDevOps;

internal sealed record VariableGroup(
	[property: JsonPropertyName("id")] uint Id,
	[property: JsonPropertyName("name")] string Name,
	[property: JsonPropertyName("variables")] ImmutableDictionary<string, Variable> Variables)
{
	[JsonIgnore]
	public string this[string key] => Variables[Name.Replace('-', '.') + '.' + key].Value;

	public VariableGroup With(string key, string value)
	{
		var variableName = Name.Replace('-', '.') + '.' + key;

		return Variables.TryGetValue(variableName, out var current)
			&& current.Value.Equals(value, StringComparison.Ordinal)
			? this 
			: this with { Variables = Variables.SetItem(variableName, new(value)) };
	}
}