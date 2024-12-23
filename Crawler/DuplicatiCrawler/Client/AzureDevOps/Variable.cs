using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace Crawler.Client.AzureDevOps
{
	internal sealed class Variable
	{
		[JsonPropertyName("value")]
		public required string Value { get; set; }
	}
}