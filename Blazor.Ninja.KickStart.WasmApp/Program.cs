using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

using Blazor.Ninja.Client.Http;
using Blazor.Ninja.Common.Data.System;
using Blazor.Ninja.Common.Factories;
using Blazor.Ninja.Common.Meta;
using Syncfusion.Blazor;

namespace Blazor.Ninja.KickStart.WasmApp
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var apiUrl = "https://api.blazor.ninja";

			// TODO Paste your app token below
			var appToken = "eyJ0eXAiOiJDU1AiLCAiZW5jIjoiUlNBX09BRVAifQ.TmEud0Nny4EKUQfgNusnmVeDdciaAD5ZpJF-nXwU-rvoVWUCNdzDBgpJT9W4xOBTLZf76BgqDNaf4zrrW-BEukzyjyn-XkhHhK14Zkcq0joQJfWx0VL90KWEswO-p-WSsZS5kFxp3Tm6rGZY1zucg97B1CTb_cm5V8PrlSHUJ_A.Q9pYG1eESeStPAq3.t1PGIguzLp7_j76FaTgR5bO4RUvslSk5eJYcGbxvLn9yj5sWqpAWYBOA_-26Xw.6TL9tC-cf-pqOiIeUfFrsg";

			var builder = WebAssemblyHostBuilder.CreateDefault(args);
			builder.RootComponents.Add<App>("app");

			builder.Services.AddScoped(
				sp => new HttpClient {BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)});

			var proxyFactory = new HttpProxyFactory(apiUrl, appToken);
			builder.Services.AddSingleton<IProxyFactory>(proxyFactory);

			await Task.WhenAll(new List<Task>
			{
				Task.Run(async () => await proxyFactory.GetConfigurationProxy().GetNamespaceAsync(UserNamespace.Label)),
				Task.Run(async () => await proxyFactory.GetConfigurationProxy().GetFeatureAsync<OnboardingFeature>()),
				Task.Run(async () => await proxyFactory.GetConfigurationProxy().GetFeatureAsync<OneTimePasswordFeature>()),
				Task.Run(async () => await proxyFactory.GetConfigurationProxy().GetFeatureAsync<PostboardingFeature>()),
				Task.Run(async () => await proxyFactory.GetConfigurationProxy().GetFeatureAsync<ThemeFeature>()),
				Task.Run(async () => await proxyFactory.GetConfigurationProxy().GetFeatureAsync<TicketFeature>())
			});
			
			builder.Services.AddSyncfusionBlazor();

			await builder.Build().RunAsync();
		}
	}
}
