using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace Crawler.Client.AzureDevOps
{
	internal sealed class BuildDefinition
	{
		[JsonPropertyName("id")]
		public int Id { get; set; }
	}
}