using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace Crawler.Client.AzureDevOps
{
	internal sealed class QueueBuildRequest
	{
		[JsonPropertyName("definition")]
		public required BuildDefinition Definition { get; set; }

		[JsonPropertyName("parameters")]
		public required string Parameters { get; set; }
	}
}