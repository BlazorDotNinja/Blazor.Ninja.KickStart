using Foundation;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace Blazor.Ninja.KickStart.MAUI
{
	[Register("AppDelegate")]
	public class AppDelegate : MauiUIApplicationDelegate
	{
		protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
	}
}