using System;
using AiForms.Renderers.iOS;
using Blastic.Forms.Sample.Initialization.Extensions;
using Blastic.Forms.Sample.iOS.Media;
using Blastic.Forms.Sample.Media;
using Foundation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace Blastic.Forms.Sample.iOS
{
	[Register(nameof(AppDelegate))]
	public class AppDelegate : FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			Xamarin.Forms.Forms.Init();
			SQLitePCL.Batteries_V2.Init();
			SettingsViewInit.Init();

			IHost host = new HostBuilder()
				.UseContentRoot(Environment.GetFolderPath(Environment.SpecialFolder.Personal))
				.ConfigureServices(
					x =>
					{
						x.AddSingleton<IAudioPlayer, AudioPlayer>();
					})
				.Initialize(LoadApplication)
				.Build();

			host.StartAsync().Wait();

			return base.FinishedLaunching(app, options);
		}
	}
}
