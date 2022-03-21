using Blazor.Ninja.Client.Http;
using Blazor.Ninja.Common.Data.System;
using Blazor.Ninja.Common.Factories;
using Blazor.Ninja.Common.Meta;

using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blazor.Ninja.KickStart.MAUI
{
	public static class MauiProgram
	{
		private static IProxyFactory _proxyFactory;

		public static MauiApp CreateMauiApp()
		{
			var apiUrl = "https://api.blazor.ninja";

			// TODO Paste your app token below
			var appToken = "eyJ0eXAiOiJDU1AiLCAiZW5jIjoiUlNBX09BRVAifQ.TmEud0Nny4EKUQfgNusnmVeDdciaAD5ZpJF-nXwU-rvoVWUCNdzDBgpJT9W4xOBTLZf76BgqDNaf4zrrW-BEukzyjyn-XkhHhK14Zkcq0joQJfWx0VL90KWEswO-p-WSsZS5kFxp3Tm6rGZY1zucg97B1CTb_cm5V8PrlSHUJ_A.Q9pYG1eESeStPAq3.t1PGIguzLp7_j76FaTgR5bO4RUvslSk5eJYcGbxvLn9yj5sWqpAWYBOA_-26Xw.6TL9tC-cf-pqOiIeUfFrsg";

			var builder = MauiApp.CreateBuilder();
			builder
				.RegisterBlazorMauiWebView()
				.UseMauiApp<App>()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				});

			builder.Services.AddBlazorWebView();
			//builder.Services.AddSingleton<WeatherForecastService>();

			_proxyFactory = new HttpProxyFactory(apiUrl, appToken);
			Task.WhenAll(new List<Task>
			{
				Task.Run(async () => await _proxyFactory.GetConfigurationProxy().GetNamespaceAsync(UserNamespace.Label)),
				Task.Run(async () => await _proxyFactory.GetConfigurationProxy().GetFeatureAsync<OnboardingFeature>()),
				Task.Run(async () => await _proxyFactory.GetConfigurationProxy().GetFeatureAsync<OneTimePasswordFeature>()),
				Task.Run(async () => await _proxyFactory.GetConfigurationProxy().GetFeatureAsync<PostboardingFeature>()),
				Task.Run(async () => await _proxyFactory.GetConfigurationProxy().GetFeatureAsync<ThemeFeature>()),
				Task.Run(async () => await _proxyFactory.GetConfigurationProxy().GetFeatureAsync<TicketFeature>())
			}).Wait();

			builder.Services.AddSingleton(_proxyFactory);

			return builder.Build();
		}
	}
}