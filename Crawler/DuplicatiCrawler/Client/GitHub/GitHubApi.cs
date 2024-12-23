using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Crawler.Client.GitHub;

internal sealed class GitHubApi : IDisposable
{
	private static readonly Regex _releaseVersion = new(@"[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+_(?<channel>[a-zA-Z]+)_[0-9]{4}-[0-9]{2}-[0-9]{2}");

	private readonly HttpClient _client = new()
	{
		DefaultRequestHeaders =
		{
			{ "User-Agent", "DuplicatiReleaseCrawler"}
		}
	};

	public async Task<Dictionary<string, (string version, Release data)>> GetDuplicatiReleases(CancellationToken ct)
	{
		var releases = await _client.GetFromJsonAsync<Release[]>(new Uri("https://api.github.com/repos/duplicati/duplicati/releases"), ct);
		
		return (releases ?? [])
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

	/// <inheritdoc />
	public void Dispose()
		=> _client.Dispose();
}