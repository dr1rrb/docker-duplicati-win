using System;
using System.Linq;
using Newtonsoft.Json;

namespace Crawler.Client.GitHub
{
	public class Asset
	{
		[JsonProperty("browser_download_url")]
		public string Url { get; set; }
	}
}