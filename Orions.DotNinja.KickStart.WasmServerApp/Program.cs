using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using Orions.DotNinja.Client.Http;
using Orions.DotNinja.Common.Client;
using Orions.DotNinja.Common.Data.System;
using Orions.DotNinja.Common.Factories;
using Orions.DotNinja.Common.Meta;

namespace Orions.DotNinja.KickStart.WasmServerApp
{
	public class Program
	{
		public static IConfigurationProxy ConfigurationProxy { get; private set; }

		public static IProxyFactory ProxyFactory { get; private set; }

		public static async Task Main(string[] args)
		{
			var apiUrl = "https://api.blazor.ninja";

			// TODO Paste your app token below
			var appToken = "eyJ0eXAiOiJDU1AiLCAiZW5jIjoiUlNBX09BRVAifQ.AKlw3OJSfuEPNSzGT72CL0QQWhUQe2iLyMEv7F-q6j9M6_LLcXYNgA5b5Xl2p-6lOQUq-GAF3peBSyXR_f5ANpHinydE_MKROfrz39xiOXDO9duhVNJHUiQ6BoPzrop-nTATO5O331HDepU-pd1EzaLkUbtj8YRg2CIRhHKydaM.Q9pYG1eESeStPAq3.t1PGIguzLp7_j76FaTgR5bO4RUvslSk5eJYcGbxvLn9yj5sWqpAWYBOA_-26Xw.sJ9wUdLl8vS6uXwStXdbgA";

			ProxyFactory = new HttpProxyFactory(apiUrl, appToken);

			ConfigurationProxy = new HttpConfigurationProxy(apiUrl, appToken);
			await Task.WhenAll(new List<Task>
			{
				Task.Run(async () => await ConfigurationProxy.GetNamespaceAsync(UserNamespace.Label)),
				Task.Run(async () => await ConfigurationProxy.GetFeatureAsync<OnboardingFeature>()),
				Task.Run(async () => await ConfigurationProxy.GetFeatureAsync<PostboardingFeature>()),
				Task.Run(async () => await ConfigurationProxy.GetFeatureAsync<ThemeFeature>())
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
