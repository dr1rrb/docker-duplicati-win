using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace Crawler.Client.GitHub
{
	internal sealed class Asset
	{
		[JsonPropertyName("browser_download_url")]
		public required string Url { get; set; }
	}
}