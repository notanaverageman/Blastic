using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Blastic.Initialization.Extensions;
using Blastic.Services.Windowing;
using Blastic.UserInterface.Settings;
using Blastic.ViewManagement;
using Blastic.ViewManagement.TypeMappers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blastic.Initialization
{
	public class BlasticApplication
	{
		internal static IViewLocator ViewLocator;

		private readonly ConfigurationBuilder _configurationBuilder;
		private readonly ServiceCollection _serviceCollection;

		private readonly HashSet<Assembly> _viewAssemblies;

		private readonly List<Func<DispatcherUnhandledExceptionEventArgs, Task>> _unhandledExceptionHandlers;

		public BlasticApplication()
		{
			_configurationBuilder = new ConfigurationBuilder();
			_serviceCollection = new ServiceCollection();

			_viewAssemblies = new HashSet<Assembly>();

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

		public BlasticApplication RegisterViewAssembly<T>()
		{
			_viewAssemblies.Add(typeof(T).Assembly);
			return this;
		}

		private void AddViewLocator(IEnumerable<Assembly> viewAssemblies)
		{
			Configure(x =>
			{
				ViewLocator viewLocator = new ViewLocator()
					.WithTypeMapper<ISettingsSectionViewModel, FormSettingSectionView>()
					.WithTypeMapper(new SuffixTypeMapper(viewAssemblies, "View", "ViewModel"));

				x.AddSingleton<IViewLocator, ViewLocator>(y => viewLocator);
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

			ViewLocator = serviceProvider.GetRequiredService<IViewLocator>();

			ILogger logger = serviceProvider.GetService<ILogger>();

			SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());
			
			try
			{
				TApp application = serviceProvider.GetRequiredService<TApp>();

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