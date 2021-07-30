using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Blazor.Extensions.Storage;

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
			var appToken = "[AppToken]";

			var builder = WebAssemblyHostBuilder.CreateDefault(args);
			builder.RootComponents.Add<App>("app");

			builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

			builder.Services.AddStorage();

			var proxyFactory = new HttpProxyFactory(apiUrl, appToken);
			builder.Services.AddSingleton<IProxyFactory>(proxyFactory);

			await Task.WhenAll(new List<Task>
			{
				Task.Run(async () => await proxyFactory.GetConfigurationProxy().GetNamespaceAsync(UserNamespace.Label)),
				Task.Run(async () => await proxyFactory.GetConfigurationProxy().GetFeatureAsync<OnboardingFeature>()),
				Task.Run(async () => await proxyFactory.GetConfigurationProxy().GetFeatureAsync<PostboardingFeature>()),
				Task.Run(async () => await proxyFactory.GetConfigurationProxy().GetFeatureAsync<ThemeFeature>()),
				Task.Run(async () => await proxyFactory.GetConfigurationProxy().GetFeatureAsync<TicketFeature>())
			});
			
			builder.Services.AddSyncfusionBlazor();

			await builder.Build().RunAsync();
		}
	}
}
