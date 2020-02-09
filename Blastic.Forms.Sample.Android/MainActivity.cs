using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Blastic.Forms.Initialization;
using Blastic.Forms.Sample.Initialization.Extensions;
using Blastic.Forms.Sample.UserInterface;
using Xamarin.Forms.Platform.Android;

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
		protected override void OnCreate(Bundle savedInstanceState)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(savedInstanceState);

			Xamarin.Forms.Forms.SetFlags("SwipeView_Experimental");

			Xamarin.Essentials.Platform.Init(this, savedInstanceState);
			Xamarin.Forms.Forms.Init(this, savedInstanceState);

			new BlasticApplication(LoadApplication)
				.RegisterViewAssembly<MainView>()
				.Initialize()
				.Run<App, MainViewModel>();
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
		{
			Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}
	}
}