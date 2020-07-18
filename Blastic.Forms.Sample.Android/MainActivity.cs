using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Blastic.Forms.Sample.Initialization.Extensions;
using Microsoft.Extensions.Hosting;
using Xamarin.Forms.Platform.Android;
using Environment = System.Environment;

namespace Blastic.Forms.Sample.Droid
{
	[Activity(
		Label = "Blastic.Forms.Android",
		Icon = "@mipmap/icon",
		Theme = "@style/MainTheme",
		MainLauncher = true,
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : FormsAppCompatActivity
	{
		protected override async void OnCreate(Bundle savedInstanceState)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(savedInstanceState);

			Xamarin.Forms.Forms.SetFlags("SwipeView_Experimental");

			Xamarin.Essentials.Platform.Init(this, savedInstanceState);
			Xamarin.Forms.Forms.Init(this, savedInstanceState);

			IHost host = new HostBuilder()
				.UseContentRoot(Environment.GetFolderPath(Environment.SpecialFolder.Personal))
				.Initialize(LoadApplication)
				.Build();

			await host.RunAsync();
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
		{
			Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}
	}
}