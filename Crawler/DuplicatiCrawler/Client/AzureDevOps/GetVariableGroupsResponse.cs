using System.Text.Json.Serialization;

namespace Crawler.Client.AzureDevOps;

internal sealed record GetVariableGroupsResponse([property: JsonPropertyName("value")] VariableGroup[] Groups);