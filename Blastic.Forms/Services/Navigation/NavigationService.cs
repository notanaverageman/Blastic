using System;
using System.Threading.Tasks;
using Blastic.LifetimeManagement;
using Blastic.ViewManagement;
using Xamarin.Forms;

namespace Blastic.Forms.Services.Navigation
{
	public class NavigationService : INavigationService
	{
		private readonly IViewLocator<VisualElement> _viewLocator;

		public NavigationService(IViewLocator<VisualElement> viewLocator)
		{
			_viewLocator = viewLocator;
		}

		public async Task GoBack(IViewAware parent)
		{
			if (!(parent.View.Value is Page parentPage))
			{
				throw new ArgumentException("The view for parent does not exist or it is not a Page.", nameof(parent));
			}

			await parentPage.Navigation.PopAsync();
		}

		public async Task NavigateTo(IViewAware parent, object model)
		{
			if (!(parent.View.Value is Page parentPage))
			{
				throw new ArgumentException("The view for parent does not exist or it is not a Page.", nameof(parent));
			}

			VisualElement element = _viewLocator.Locate(model);

			Page page = element as Page;

			if (element is ContentView contentView)
			{
				page = new ContentPage
				{
					Content = contentView
				};
			}

			if (page == null)
			{
				throw new ArgumentException("The view for model is not a Page or ContentView.", nameof(model));
			}

			await parentPage.Navigation.PushAsync(page);

			if (model is IHasLifetime hasLifetime)
			{
				hasLifetime.Lifetime.Activate();
			}

			if (model is IHasAsyncLifetime hasAsyncLifetime)
			{
				await hasAsyncLifetime.Lifetime.Activate();
			}
		}
	}
}