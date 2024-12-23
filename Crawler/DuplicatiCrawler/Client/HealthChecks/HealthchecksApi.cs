using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Crawler.Client.HealthChecks;

internal sealed class HealthchecksApi(IConfiguration config, ILogger<HealthchecksApi> log) : IDisposable
{
	private readonly HttpClient _client = new()
	{
		BaseAddress = new Uri("https://hc-ping.com/")
	};

	public Task Report(string identifier, CancellationToken ct) => ReportCore(identifier, string.Empty, ct);
	public Task Start(string identifier, CancellationToken ct) => ReportCore(identifier, "/start", ct);
	public Task Failed(string identifier, CancellationToken ct) => ReportCore(identifier, "/failed", ct);

	private async Task ReportCore(string identifier, string method, CancellationToken ct)
	{
		try
		{
			var check = config["HC_" + identifier];
			if (check is not { Length: > 0 })
			{
				return;
			}

			using var response = await _client.GetAsync(check + method, ct);
			response.EnsureSuccessStatusCode();
		}
		catch (Exception e)
		{
			log.LogError(e, "Failed to ping healthchecks.io");
		}
	}

	/// <inheritdoc />
	public void Dispose()
		=> _client.Dispose();
}