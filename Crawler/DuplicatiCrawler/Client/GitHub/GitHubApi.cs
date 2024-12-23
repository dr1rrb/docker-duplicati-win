using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Crawler.Client.GitHub
{
	public class GitHubApi : IDisposable
	{
		private static readonly Regex _releaseVersion = new Regex(@"[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+_(?<channel>[a-zA-Z]+)_[0-9]{4}-[0-9]{2}-[0-9]{2}");

		private readonly HttpClient _client;

		public GitHubApi()
		{
			_client = new HttpClient
			{
				DefaultRequestHeaders =
				{
					{ "User-Agent", "DuplicatiReleaseCrawler"}
				}
			};
		}

		public async Task<Dictionary<string, (string version, Release data)>> GetDuplicatiReleases(CancellationToken ct)
		{
			using (var response = await _client.GetAsync(new Uri("https://api.github.com/repos/duplicati/duplicati/releases"), ct))
			{
				var raw = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
				return JsonSerializer
					.Deserialize<Release[]>(raw)
					.Select(r => new
					{
						release = r,
						version = _releaseVersion.Match(r.Version)
					})
					.Where(x => x.version.Success)
					.GroupBy(x => x.version.Groups["channel"].Value)
					.ToDictionary(
						g => g.Key,
						g =>
						{
							var last = g.OrderBy(x => x.release.PublicationDate).Last();
							var version = last.version.Value;

							return (version, last.release);
						});
			}
		}

		/// <inheritdoc />
		public void Dispose()
			=> _client.Dispose();
	}
}