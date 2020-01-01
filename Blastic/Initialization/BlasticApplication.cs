using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Blastic.Initialization.Extensions;
using Blastic.Services.Windowing;
using Blastic.UserInterface.Logs;
using Blastic.UserInterface.Settings;
using Blastic.ViewManagement;
using Blastic.ViewManagement.TypeMappers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Log = Serilog.Log;

namespace Blastic.Initialization
{
	public class BlasticApplication
	{
		internal static IViewLocator ViewLocator;

		private readonly ConfigurationBuilder _configurationBuilder;
		private readonly ContainerBuilder _containerBuilder;
		private readonly ServiceCollection _serviceCollection;

		private readonly HashSet<Assembly> _viewAssemblies;

		private readonly App _application;

		public BlasticApplication()
		{
			_configurationBuilder = new ConfigurationBuilder();
			_containerBuilder = new ContainerBuilder();
			_serviceCollection = new ServiceCollection();

			_viewAssemblies = new HashSet<Assembly>();

			_application = new App();

			this.AddDefaults();
		}

		public BlasticApplication OnUnhandledException(Func<DispatcherUnhandledExceptionEventArgs, Task> func)
		{
			_application.DispatcherUnhandledException += async (sender, args) =>
			{
				await func(args);
			};

			return this;
		}

		public BlasticApplication Configure(Action<ConfigurationBuilder> action)
		{
			action(_configurationBuilder);
			return this;
		}

		public BlasticApplication Configure(Action<ContainerBuilder> action)
		{
			action(_containerBuilder);
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
			Configure(builder =>
			{
				ViewLocator viewLocator = new ViewLocator()
					.WithTypeMapper<ISettingsSectionViewModel, FormSettingSectionView>()
					.WithTypeMapper(new SuffixTypeMapper(viewAssemblies, "View", "ViewModel"));

				builder
					.RegisterInstance(viewLocator)
					.AsImplementedInterfaces()
					.AsSelf()
					.SingleInstance();
			});
		}

		public void Run<TMainViewModel>()
		{
			IConfiguration configuration = _configurationBuilder.Build();

			LoggerConfiguration loggerConfiguration = new LoggerConfiguration()
				.ReadFrom.Configuration(configuration);

			RegisterViewAssembly<BlasticApplication>();
			AddViewLocator(_viewAssemblies);

			Configure(x => x.RegisterInstance(configuration));
			Configure(x => x.RegisterType<TMainViewModel>().SingleInstance());

			_containerBuilder.Populate(_serviceCollection);
			IContainer container = _containerBuilder.Build();

			ViewLocator = container.Resolve<IViewLocator>();

			LogSink logSink = container.ResolveOptional<LogSink>();

			if (logSink != null)
			{
				loggerConfiguration.WriteTo.Sink(logSink);
			}

			Log.Logger = loggerConfiguration.CreateLogger();
			
			SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());
			
			try
			{
				TMainViewModel viewModel = container.Resolve<TMainViewModel>();
				IWindowManager windowManager = container.Resolve<IWindowManager>();

				// Do not await this method as it will freeze the UI.
				windowManager.ShowWindow(viewModel);
				_application.Run();
			}
			catch (Exception exception)
			{
				Log.Error(exception, exception.Message);
				throw;
			}
		}
	}
}