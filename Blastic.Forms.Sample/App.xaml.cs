using Blastic.LifetimeManagement;
using Xamarin.Forms;

namespace Blastic.Forms.Sample
{
	public partial class App
	{
		public App()
		{
			InitializeComponent();

			UserAppTheme = OSAppTheme.Light;
		}

		protected override async void OnStart()
		{
			if (!(MainPage.BindingContext is IHasLifetime hasLifetime))
			{
				return;
			}

			await hasLifetime.Lifetime.Activate();
		}
	}
}