using System;
using System.Threading;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Blastic.Initialization;
using Blastic.LifetimeManagement;
using Blastic.LifetimeManagement.Contexts;
using Microsoft.Extensions.DependencyInjection;

namespace Blastic.Uno
{
	public class BlasticApp : Application
	{
		private readonly IServiceProvider _serviceProvider;

		public BlasticApp(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		protected override void OnLaunched(LaunchActivatedEventArgs e)
		{
			Frame rootFrame = Window.Current.Content as Frame;

			if (rootFrame == null)
			{
				rootFrame = new Frame();

				rootFrame.NavigationFailed += OnNavigationFailed;

				if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
				{
					//TODO: Load state from previously suspended application
				}

				Window.Current.Content = rootFrame;
			}

			if (e.PrelaunchActivated == false)
			{
				if (rootFrame.Content == null)
				{
					SetMainView(rootFrame);
				}

				Window.Current.Activate();
			}
		}

		private async void SetMainView(Frame rootFrame)
		{
			Type mainViewModelType = _serviceProvider.GetRequiredService<MainViewModelDescriptor>().MainViewModelType;
			object mainViewModel = _serviceProvider.GetRequiredService(mainViewModelType);

			UIElement view = BlasticApplication.ViewLocator.Locate(mainViewModel);
			rootFrame.Content = view;

			if (mainViewModel is IHasLifetime hasLifetime)
			{
				ActivationContext context = new ActivationContext(CancellationToken.None);
				await hasLifetime.Lifetime.Activate.Execute(context);
			}
		}

		void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
		{
			throw new Exception($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
		}
	}

	internal class MainViewModelDescriptor
	{
		public Type MainViewModelType { get; }

		public MainViewModelDescriptor(Type mainViewModelType)
		{
			MainViewModelType = mainViewModelType;
		}
	}
}
