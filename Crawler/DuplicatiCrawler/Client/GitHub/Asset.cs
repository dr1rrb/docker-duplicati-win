using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace Crawler.Client.GitHub
{
	public class Asset
	{
		[JsonPropertyName("browser_download_url")]
		public string Url { get; set; }
	}
}