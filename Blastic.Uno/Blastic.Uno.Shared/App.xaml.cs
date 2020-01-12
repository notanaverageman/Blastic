using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Blastic.Initialization;
using Blastic.LifetimeManagement;
using Blastic.LifetimeManagement.Contexts;
#if !__WASM__
// using FirstFloor.XamlSpy.Services;
#endif
using Microsoft.Extensions.DependencyInjection;

namespace Blastic.Uno
{
	public sealed partial class App
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly Type _mainViewModelType;

		public App(IServiceProvider serviceProvider, Type mainViewModelType)
		{
			_serviceProvider = serviceProvider;
			_mainViewModelType = mainViewModelType;

			ConfigureFilters(global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory);
			InitializeComponent();
		}

		protected override void OnLaunched(LaunchActivatedEventArgs e)
		{
#if !__WASM__
			// XamlSpyService.Current.Connect("localhost", 4530, "01345");
#endif

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
			object mainViewModel = _serviceProvider.GetRequiredService(_mainViewModelType);

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

		private static void ConfigureFilters(ILoggerFactory factory)
		{
			factory
				.WithFilter(new FilterLoggerSettings
					{
						{ "Uno", LogLevel.Warning },
						{ "Windows", LogLevel.Warning },

						// Debug JS interop
						// { "Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug },

						// Generic Xaml events
						// { "Windows.UI.Xaml", LogLevel.Debug },
						// { "Windows.UI.Xaml.VisualStateGroup", LogLevel.Debug },
						// { "Windows.UI.Xaml.StateTriggerBase", LogLevel.Debug },
						// { "Windows.UI.Xaml.UIElement", LogLevel.Debug },

						// Layouter specific messages
						// { "Windows.UI.Xaml.Controls", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.Layouter", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.Panel", LogLevel.Debug },
						// { "Windows.Storage", LogLevel.Debug },

						// Binding related messages
						// { "Windows.UI.Xaml.Data", LogLevel.Debug },

						// DependencyObject memory references tracking
						// { "ReferenceHolder", LogLevel.Debug },
					}
				)
#if DEBUG
				.AddConsole(LogLevel.Debug);
#else
				.AddConsole(LogLevel.Information);
#endif
		}
	}
}
