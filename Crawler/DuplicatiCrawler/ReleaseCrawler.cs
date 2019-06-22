using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Crawler.Client.AzureDevOps;
using Crawler.Client.GitHub;
using Crawler.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Crawler
{
	public static class ReleaseCrawler
	{
		[FunctionName("ReleaseCrawler")]
		public static async void Run([TimerTrigger("0 3 * * *")] TimerInfo myTimer, ILogger log, CancellationToken ct)
		{
			log.LogInformation($"Launching crawler ({DateTime.Now})");
			try
			{
				await Crawl(ct);
			}
			catch (Exception e)
			{
				log.LogError(e, "Failed to crawl releases.");
			}
		}

		[FunctionName("Crawl")]
		public static async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "crawl")]
			HttpRequest req,
			ILogger log,
			CancellationToken ct)
		{
			try
			{
				return new OkObjectResult(await Crawl(ct));
			}
			catch (Exception e)
			{
				log.LogError(e, "Failed to crawl releases.");

				throw;
			}
		}

		private static async Task<UpdateResult[]> Crawl(CancellationToken ct)
		{
			var config = new ConfigurationBuilder()
				.AddJsonFile("host.json", optional: true, reloadOnChange: true)
				.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables()
				.Build();

			var clientId = config["ClientId"];
			var clientSecret = config["ClientSecret"];
			var aadDomain = config["AADDomain"];

#if DEBUG
			var channels = new[] { "canary" };
#else
			var channels = new[] { "stable", "beta", "experimental", "canary" };
#endif

			using (var gitHub = new GitHubApi())
			using (var azure = new AzureDevOpsApi(config["azureDevOpsAuth"]))
			{
				var releases = await gitHub.GetDuplicatiReleases(ct);
				var variables = await azure.GetBuildVariables(ct);

				return await Task.WhenAll(channels.Select(TryUpdateChannel));

				async Task<UpdateResult> TryUpdateChannel(string channel)
				{
					var status = "parsing inputs";
					try
					{
						if (!releases.TryGetValue(channel, out var release))
						{
							throw new Exception($"Cannot find release for {channel}");
						}

						if (!variables.TryGetValue(channel, out var group))
						{
							throw new Exception($"Cannot find build variables for {channel}");
						}

						var install = release.data.Assets.FirstOrDefault(a => a.Url.EndsWith(release.version + ".zip", StringComparison.OrdinalIgnoreCase))?.Url;
						var version = release.version;
						if (install.IsNullOrWhiteSpace() || version.IsNullOrWhiteSpace())
						{
							throw new Exception($"The found release is invalid for {channel} (Failed to get required values 'install' and 'version')");
						}

						var hasChanged = false;
						hasChanged |= group.TryUpdate("install", install);
						hasChanged |= group.TryUpdate("version", version);
						hasChanged |= group.TryUpdate("url", release.data.Url);
						hasChanged |= group.TryUpdate("notes", release.data.Notes);

						if (hasChanged)
						{
							status = "updating build variables";
							await azure.UpdateBuildVariables(group, ct);

							status = "queuing new build";
							await azure.QueueBuild(channel, ct);

							return UpdateResult.Succeeded(channel);
						}
						else
						{
							return UpdateResult.NotChanged(channel);
						}
					}
					catch (Exception e)
					{
						return UpdateResult.Failed(channel, status, e);
					}
				}
			}
		}
	}
}
