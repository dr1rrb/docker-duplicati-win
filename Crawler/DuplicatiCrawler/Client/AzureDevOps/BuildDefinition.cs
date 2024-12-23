using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace Crawler.Client.AzureDevOps
{
	public class BuildDefinition
	{
		[JsonPropertyName("id")]
		public int Id { get; set; }
	}
}