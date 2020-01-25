using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Blastic.Platform;
using Blastic.Services.Windowing;
using Blastic.UserInterface.Settings;
using Blastic.ViewManagement;
using Blastic.ViewManagement.TypeMappers;
using Blastic.Wpf.Initialization.Extensions;
using Blastic.Wpf.Platform;
using Blastic.Wpf.UserInterface.Settings;
using Blastic.Wpf.ViewManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blastic.Wpf.Initialization
{
	public class BlasticApplication
	{
		internal static IViewLocator<FrameworkElement> ViewLocator;

		private readonly ConfigurationBuilder _configurationBuilder;
		private readonly ServiceCollection _serviceCollection;

		private readonly HashSet<Assembly> _viewAssemblies;
		private readonly ViewLocator _viewLocator;

		private readonly List<Func<DispatcherUnhandledExceptionEventArgs, Task>> _unhandledExceptionHandlers;

		public BlasticApplication()
		{
			_configurationBuilder = new ConfigurationBuilder();
			_serviceCollection = new ServiceCollection();

			_viewAssemblies = new HashSet<Assembly>();
			_viewLocator = new ViewLocator();

			_unhandledExceptionHandlers = new List<Func<DispatcherUnhandledExceptionEventArgs, Task>>();

			this.AddDefaults();
		}

		public BlasticApplication OnUnhandledException(Func<DispatcherUnhandledExceptionEventArgs, Task> func)
		{
			_unhandledExceptionHandlers.Add(func);
			return this;
		}

		public BlasticApplication Configure(Action<ConfigurationBuilder> action)
		{
			action(_configurationBuilder);
			return this;
		}

		public BlasticApplication Configure(Action<IServiceCollection> action)
		{
			action(_serviceCollection);
			return this;
		}

		public BlasticApplication Configure(Action<ViewLocator> action)
		{
			action(_viewLocator);
			return this;
		}

		public BlasticApplication RegisterViewAssembly<T>()
		{
			_viewAssemblies.Add(typeof(T).Assembly);
			return this;
		}

		private void AddViewLocator(IEnumerable<Assembly> viewAssemblies)
		{
			Configure(x =>
			{
				_viewLocator
					.WithTypeMapper<ISettingsSectionViewModel, FormSettingSectionView>()
					.WithTypeMapper<SettingsViewModel, SettingsView>()
					.WithTypeMapper(new SuffixTypeMapper(viewAssemblies, "View", "ViewModel"));

				x.AddSingleton<IViewLocator<FrameworkElement>>(y => _viewLocator);
			});
		}

		public void Run<TApp, TMainViewModel>() where TApp : Application where TMainViewModel : class
		{
			IConfiguration configuration = _configurationBuilder.Build();

			RegisterViewAssembly<BlasticApplication>();
			AddViewLocator(_viewAssemblies);

			Configure(x => x.AddSingleton(configuration));
			Configure(x => x.AddSingleton<TMainViewModel>());
			Configure(x => x.AddSingleton<TApp>());

			ServiceProvider serviceProvider = _serviceCollection.BuildServiceProvider();

			ViewLocator = serviceProvider.GetRequiredService<IViewLocator<FrameworkElement>>();

			ILogger logger = serviceProvider.GetService<ILogger>();

			SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());
			
			try
			{
				TApp application = serviceProvider.GetRequiredService<TApp>();

				PlatformSpecifics.Current = new WpfPlatformSpecifics(application.Dispatcher);

				foreach (Func<DispatcherUnhandledExceptionEventArgs, Task> handler in _unhandledExceptionHandlers)
				{
					application.DispatcherUnhandledException += async (sender, args) =>
					{
						await handler(args);
					};
				}

				TMainViewModel viewModel = serviceProvider.GetRequiredService<TMainViewModel>();
				IWindowManager windowManager = serviceProvider.GetRequiredService<IWindowManager>();

				// Do not await this method as it will freeze the UI.
				windowManager.ShowWindow(viewModel);
				application.Run();
			}
			catch (Exception exception)
			{
				logger?.LogError(exception, exception.Message);
				throw;
			}
		}
	}
}