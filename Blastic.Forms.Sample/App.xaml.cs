using Blastic.LifetimeManagement;
using Blastic.LifetimeManagement.Contexts;

namespace Blastic.Forms.Sample
{
	public partial class App
	{
		public App()
		{
			InitializeComponent();
		}

		protected override async void OnStart()
		{
			if (!(MainPage.BindingContext is IHasLifetime hasLifetime))
			{
				return;
			}

			await hasLifetime.Lifetime.Activate.Execute(new ActivationContext());
		}
	}
}