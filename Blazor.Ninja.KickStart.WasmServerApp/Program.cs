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
			var appToken = "eyJ0eXAiOiJDU1AiLCAiZW5jIjoiUlNBX09BRVAifQ.TmEud0Nny4EKUQfgNusnmVeDdciaAD5ZpJF-nXwU-rvoVWUCNdzDBgpJT9W4xOBTLZf76BgqDNaf4zrrW-BEukzyjyn-XkhHhK14Zkcq0joQJfWx0VL90KWEswO-p-WSsZS5kFxp3Tm6rGZY1zucg97B1CTb_cm5V8PrlSHUJ_A.Q9pYG1eESeStPAq3.t1PGIguzLp7_j76FaTgR5bO4RUvslSk5eJYcGbxvLn9yj5sWqpAWYBOA_-26Xw.6TL9tC-cf-pqOiIeUfFrsg";

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
