using System.Text.Json.Serialization;

namespace Crawler.Client.AzureDevOps;

internal sealed record QueueBuildRequest(
	[property: JsonPropertyName("definition")] BuildDefinition Definition,
	[property: JsonPropertyName("parameters")] string Parameters);