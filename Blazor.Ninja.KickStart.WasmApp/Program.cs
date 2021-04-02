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

namespace Blazor.Ninja.KickStart.WasmApp
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var apiUrl = "https://api.blazor.ninja";

			// TODO Paste your app token below
			var appToken = "eyJ0eXAiOiJDU1AiLCAiZW5jIjoiUlNBX09BRVAifQ.AKlw3OJSfuEPNSzGT72CL0QQWhUQe2iLyMEv7F-q6j9M6_LLcXYNgA5b5Xl2p-6lOQUq-GAF3peBSyXR_f5ANpHinydE_MKROfrz39xiOXDO9duhVNJHUiQ6BoPzrop-nTATO5O331HDepU-pd1EzaLkUbtj8YRg2CIRhHKydaM.Q9pYG1eESeStPAq3.t1PGIguzLp7_j76FaTgR5bO4RUvslSk5eJYcGbxvLn9yj5sWqpAWYBOA_-26Xw.sJ9wUdLl8vS6uXwStXdbgA";

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
			
			await builder.Build().RunAsync();
		}
	}
}
