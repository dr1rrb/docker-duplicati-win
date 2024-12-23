using System.Text.Json.Serialization;

namespace Crawler.Client.AzureDevOps;

internal sealed record BuildDefinition([property: JsonPropertyName("id")] int Id);