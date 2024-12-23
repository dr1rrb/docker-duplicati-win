using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Crawler.Extensions;

namespace Crawler.Client.AzureDevOps
{
	public class AzureDevOpsApi : IDisposable
	{
		private readonly HttpClient _client;

		public AzureDevOpsApi(string auth)
		{
			_client = new HttpClient
			{
				BaseAddress = new Uri("https://dev.azure.com/dr1rrb/docker-duplicati-win/_apis/"),
				DefaultRequestHeaders =
				{
					{ "Authorization", auth}
				}
			};
		}

		public async Task<Dictionary<string, VariableGroup>> GetBuildVariables(CancellationToken ct)
		{
			using (var response = await _client.GetAsync("distributedtask/variablegroups?api-version=5.0-preview.1", ct))
			{
				var raw = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
				var groups = JsonSerializer.Deserialize<GetVariableGroupsResponse>(raw).Groups;

				return groups.ToDictionary(g => g.Name.TrimStart("duplicati-", StringComparison.OrdinalIgnoreCase));
			}
		}

		public async Task UpdateBuildVariables(VariableGroup group, CancellationToken ct)
		{
			var body = new StringContent(JsonSerializer.Serialize(group), Encoding.UTF8, "application/json");

			using (var response = await _client.PutAsync($"distributedtask/variablegroups/{@group.Id}?api-version=5.0-preview.1", body, ct))
			{
				response.EnsureSuccessStatusCode();
			}
		}

		public async Task QueueBuild(string channel, CancellationToken ct)
		{
			var parameters = new Dictionary<string, string>
			{
				{ "duplicati.channel", channel }
			};
			var request = new QueueBuildRequest
			{
				Definition = new BuildDefinition
				{
					Id = 1
				},
				Parameters = JsonSerializer.Serialize(parameters)
			};
			var body = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

			using (var response = await _client.PostAsync("build/builds?api-version=5.0", body, ct))
			{
				response.EnsureSuccessStatusCode();
			}
		}

		/// <inheritdoc />
		public void Dispose()
			=> _client.Dispose();

	}
}