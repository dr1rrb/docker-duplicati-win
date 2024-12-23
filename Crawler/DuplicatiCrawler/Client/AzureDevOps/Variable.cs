using System.Text.Json.Serialization;

namespace Crawler.Client.AzureDevOps;

internal sealed record Variable([property: JsonPropertyName("value")] string Value);