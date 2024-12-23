using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace Crawler.Client.AzureDevOps
{
	public class QueueBuildRequest
	{
		[JsonPropertyName("definition")]
		public BuildDefinition Definition { get; set; }

		[JsonPropertyName("parameters")]
		public string Parameters { get; set; }
	}
}