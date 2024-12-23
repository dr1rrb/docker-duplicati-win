using Crawler.Client.AzureDevOps;
using Crawler.Client.GitHub;
using Crawler.Client.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var host = new HostBuilder();

host
	.ConfigureFunctionsWebApplication()
	.ConfigureAppConfiguration(config => config
		.AddJsonFile("host.json", optional: true, reloadOnChange: true)
		.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
		.AddEnvironmentVariables()
	)
	.ConfigureServices((ctx, svc) => svc
		.AddSingleton<HealthchecksApi>()
		.AddSingleton<GitHubApi>()
		.AddSingleton<AzureDevOpsApi>(_ => new AzureDevOpsApi(ctx.Configuration["AZURE_AUTH"] ?? throw new InvalidOperationException("No azure auth token configured.")))
	);

host.Build().Run();