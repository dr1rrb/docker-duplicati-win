using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Crawler.Client.AzureDevOps;
using Crawler.Client.GitHub;
using Crawler.Client.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Crawler;

internal sealed class ReleaseCrawler(HealthchecksApi hc, GitHubApi gitHub, AzureDevOpsApi azure, ILogger<ReleaseCrawler> log)
{
#if DEBUG
	private static readonly string[] _channels = ["canary"];
#else
	private static readonly string[] _channels = ["stable", "beta", "experimental", "canary"];
#endif

	[Function("ScheduledCrawl")]
	public async Task RunScheduledCrawl(
		[TimerTrigger("0 0 3 * * *")] TimerInfo myTimer,
		CancellationToken ct)
	{
		log.LogInformation($"Launching crawler ({DateTime.Now})");
		try
		{
			await CrawlReleases(_channels, ct);
		}
		catch (Exception e)
		{
			log.LogError(e, "Failed to crawl releases.");
		}
	}

	[Function("ImmediateCrawl")]
	public async Task<IActionResult> RunImmediateCrawl(
		[HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "crawl/{channel?}")] HttpRequest req,
		[FromQuery] string channel,
		CancellationToken ct)
	{
		try
		{
			var channels = channel is { Length: > 0 } ? [channel] : _channels;

			return new OkObjectResult(await CrawlReleases(channels, ct));
		}
		catch (Exception e)
		{
			log.LogError(e, "Failed to crawl releases.");

			throw;
		}
	}

	private async Task<UpdateResult[]> CrawlReleases(string[] channels, CancellationToken ct)
	{
		var releases = await gitHub.GetDuplicatiReleases(ct);
		var variables = await azure.GetBuildVariables(ct);

		var @default = variables["default"];

		return await Task.WhenAll(channels.Select(TryUpdateChannel));

		async Task<UpdateResult> TryUpdateChannel(string channel)
		{
			var status = "parsing inputs";
			try
			{
				status = "searching release";
				if (!releases.TryGetValue(channel, out var release))
				{
					throw new InvalidOperationException($"Cannot find release for {channel}");
				}

				status = "reporting start to healthchecks";
				await hc.Start(channel, ct); // we report to HC only if we found a release. This prevent issue with the still awaited "stable" version :)

				status = "searching variable group";
				if (!variables.TryGetValue(channel, out var group))
				{
					throw new InvalidOperationException($"Cannot find build variables for {channel}");
				}

				status = "analyzing build config vs. found release";
				var install = release.data.Assets.FirstOrDefault(a => a.Url.EndsWith(release.version + "-win-x64-gui.zip", StringComparison.OrdinalIgnoreCase))?.Url;
				var version = release.version;
				if (install is not { Length: > 0 } || version is not { Length: > 0 })
				{
					throw new InvalidOperationException($"The found release is invalid for {channel} (Failed to get required values 'install' and 'version')");
				}

				var updated = group
					.With("install", install)
					.With("version", version)
					.With("url", release.data.Url)
					.With("notes", release.data.Notes);

				if (updated != group)
				{
					status = "updating build variables";
					await azure.UpdateBuildVariables(updated, ct);

					if (@default["channel"] == channel)
					{
						status = "updating **default** build variables";

						@default
							.With("install", install)
							.With("version", version)
							.With("url", release.data.Url)
							.With("notes", release.data.Notes);
								
						await azure.UpdateBuildVariables(@default, ct);

						status = "queuing new **default** build";
						await azure.QueueBuild("default", ct); // So the image is tagged as 'latest'
					}
					else
					{
						status = "queuing new build";
						await azure.QueueBuild(channel, ct);
					}

					status = "reporting success to healthchecks";
					await hc.Report(channel, ct);

					return UpdateResult.Succeeded(channel);
				}
				else
				{
					status = "reporting success (not changed) to healthchecks";
					await hc.Report(channel, ct);

					return UpdateResult.NotChanged(channel);
				}
			}
			catch (Exception e)
			{
				await hc.Failed(channel, ct); // cannot fail

				return UpdateResult.Failed(channel, status, e);
			}
		}
	}
}