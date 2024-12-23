using System;
using System.Text.Json.Serialization;

namespace Crawler.Client.GitHub;

internal sealed record Release(
	[property: JsonPropertyName("html_url")] string Url,
	[property: JsonPropertyName("name")] string Version,
	[property: JsonPropertyName("body")] string Notes,
	[property: JsonPropertyName("assets")] Asset[] Assets,
	[property: JsonPropertyName("published_at")] DateTimeOffset PublicationDate);