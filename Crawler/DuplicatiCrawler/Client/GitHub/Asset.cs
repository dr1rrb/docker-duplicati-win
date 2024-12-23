using System.Text.Json.Serialization;

namespace Crawler.Client.GitHub;

internal sealed record Asset([property: JsonPropertyName("browser_download_url")] string Url);