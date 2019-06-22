using System;
using System.Linq;
using Newtonsoft.Json;

namespace Crawler.Client.GitHub
{
	public class Release
	{
		[JsonProperty("html_url")]
		public string Url { get; set; }

		[JsonProperty("name")]
		public string Version { get; set; }

		[JsonProperty("body")]
		public string Notes { get; set; }

		public Asset[] Assets { get; set; }

		[JsonProperty("published_at")]
		public DateTimeOffset PublicationDate { get; set; }
	}
}