using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Crawler.Client.HealthChecks;

internal sealed partial class HealthchecksApi(IConfiguration config, ILogger<HealthchecksApi> log) : IDisposable
{
	#region Logs
	[LoggerMessage(0, LogLevel.Error, "Failed to ping healthchecks.io")]
	private static partial void LogHealthcheckError(ILogger logger, Exception exception); 
	#endregion

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

			using var response = await _client.GetAsync(new Uri(check + method, UriKind.Relative), ct);
			response.EnsureSuccessStatusCode();
		}
		catch (Exception e)
		{
			LogHealthcheckError(log, e);
		}
	}

	/// <inheritdoc />
	public void Dispose()
		=> _client.Dispose();
}
