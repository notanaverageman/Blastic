using Blastic.Forms.Initialization;
using Blastic.Forms.Sample.UserInterface;
using Foundation;
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

            new BlasticApplication(LoadApplication)
	            .RegisterViewAssembly<MainView>()
	            .Run<App, MainViewModel>();

            return base.FinishedLaunching(app, options);
        }
    }
}
