using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace Crawler.Client.GitHub
{
	internal sealed class Release
	{
		[JsonPropertyName("html_url")]
		public required string Url { get; set; }

		[JsonPropertyName("name")]
		public required string Version { get; set; }

		[JsonPropertyName("body")]
		public required string Notes { get; set; }

		public required Asset[] Assets { get; set; }

		[JsonPropertyName("published_at")]
		public required DateTimeOffset PublicationDate { get; set; }
	}
}