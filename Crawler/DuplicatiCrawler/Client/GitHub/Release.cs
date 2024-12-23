using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace Crawler.Client.GitHub
{
	public class Release
	{
		[JsonPropertyName("html_url")]
		public string Url { get; set; }

		[JsonPropertyName("name")]
		public string Version { get; set; }

		[JsonPropertyName("body")]
		public string Notes { get; set; }

		public Asset[] Assets { get; set; }

		[JsonPropertyName("published_at")]
		public DateTimeOffset PublicationDate { get; set; }
	}
}