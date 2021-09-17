using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using Blazor.Ninja.Client.Http;
using Blazor.Ninja.Common.Data.System;
using Blazor.Ninja.Common.Factories;
using Blazor.Ninja.Common.Meta;

namespace Blazor.Ninja.KickStart.WasmServerApp
{
	public class Program
	{
		public static IProxyFactory ProxyFactory { get; private set; }

		public static async Task Main(string[] args)
		{
			var apiUrl = "https://api.blazor.ninja";

			// TODO Paste your app token below
			var appToken = "[AppToken]";

			ProxyFactory = new HttpProxyFactory(apiUrl, appToken);

			await Task.WhenAll(new List<Task>
			{
				Task.Run(async () => await ProxyFactory.GetConfigurationProxy().GetNamespaceAsync(UserNamespace.Label)),
				Task.Run(async () => await ProxyFactory.GetConfigurationProxy().GetFeatureAsync<OnboardingFeature>()),
				Task.Run(async () => await ProxyFactory.GetConfigurationProxy().GetFeatureAsync<OneTimePasswordFeature>()),
				Task.Run(async () => await ProxyFactory.GetConfigurationProxy().GetFeatureAsync<PostboardingFeature>()),
				Task.Run(async () => await ProxyFactory.GetConfigurationProxy().GetFeatureAsync<ThemeFeature>()),
				Task.Run(async () => await ProxyFactory.GetConfigurationProxy().GetFeatureAsync<TicketFeature>())
			});

			await CreateHostBuilder(args).Build().RunAsync();
		}


		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}
