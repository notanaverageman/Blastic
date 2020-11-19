using System;
using Blastic.Forms.Sample.Initialization.Extensions;
using Foundation;
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
			Xamarin.Forms.Forms.SetFlags("SwipeView_Experimental");
			Xamarin.Forms.Forms.Init();

			Sharpnado.Tabs.iOS.Preserver.Preserve();

			IHost host = new HostBuilder()
				.UseContentRoot(Environment.GetFolderPath(Environment.SpecialFolder.Personal))
				.Initialize(LoadApplication)
				.Build();

			host.StartAsync().Wait();

			return base.FinishedLaunching(app, options);
		}
	}
}
