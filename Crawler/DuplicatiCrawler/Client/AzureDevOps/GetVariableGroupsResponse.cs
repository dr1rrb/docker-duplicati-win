using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace Crawler.Client.AzureDevOps
{
	internal class GetVariableGroupsResponse
	{
		[JsonPropertyName("value")]
		public VariableGroup[] Groups { get; set; }
	}
}