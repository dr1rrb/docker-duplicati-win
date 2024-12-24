using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Crawler.Extensions;

namespace Crawler.Client.AzureDevOps;

internal sealed class AzureDevOpsApi(string auth) : IDisposable
{
	private readonly HttpClient _client = new()
	{
		BaseAddress = new Uri("https://dev.azure.com/dr1rrb/docker-duplicati-win/_apis/"),
		DefaultRequestHeaders =
		{
			{ "Authorization", auth}
		}
	};

	public async Task<Dictionary<string, VariableGroup>> GetBuildVariables(CancellationToken ct)
	{
		var response = await _client.GetFromJsonAsync<GetVariableGroupsResponse>(new Uri("distributedtask/variablegroups?api-version=5.0-preview.1", UriKind.Relative), ct)
			?? throw new InvalidOperationException("Failed to get build variables.");

		return response.Groups.ToDictionary(g => g.Name.TrimStart("duplicati-", StringComparison.OrdinalIgnoreCase));
	}

	public async Task UpdateBuildVariables(VariableGroup group, CancellationToken ct)
	{
		using var body = JsonContent.Create(group);
		using var response = await _client.PutAsync(new Uri($"distributedtask/variablegroups/{group.Id}?api-version=5.0-preview.1", UriKind.Relative), body, ct);
		response.EnsureSuccessStatusCode();
	}

	public async Task QueueBuild(string channel, CancellationToken ct)
	{
		var parameters = new Dictionary<string, string>
		{
			{ "duplicati.channel", channel }
		};

		using var body = JsonContent.Create(new QueueBuildRequest(new BuildDefinition(1), JsonSerializer.Serialize(parameters)));
		using var response = await _client.PostAsync(new Uri("build/builds?api-version=5.0", UriKind.Relative), body, ct);
		response.EnsureSuccessStatusCode();
	}

	/// <inheritdoc />
	public void Dispose()
		=> _client.Dispose();

}