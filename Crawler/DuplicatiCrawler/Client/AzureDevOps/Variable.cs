using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace Crawler.Client.AzureDevOps
{
	public class Variable
	{
		[JsonPropertyName("value")]
		public string Value { get; set; }
	}
}