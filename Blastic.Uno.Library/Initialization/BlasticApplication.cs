using System;
using System.Collections.Generic;
using System.Reflection;
using Windows.UI.Xaml;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Blastic.Controls.DynamicControls;
using Blastic.Initialization.Extensions;
using Blastic.Uno;
using Blastic.Uno.Shared.Controls.DynamicControls;
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
		public static IViewLocator ViewLocator { get; private set; }

		private readonly ConfigurationBuilder _configurationBuilder;
		private readonly LoggerConfiguration _loggerConfiguration;
		private readonly ContainerBuilder _containerBuilder;
		private readonly ServiceCollection _serviceCollection;

		private readonly HashSet<Assembly> _viewAssemblies;

		public BlasticApplication()
		{
			_configurationBuilder = new ConfigurationBuilder();
			_loggerConfiguration = new LoggerConfiguration();
			_containerBuilder = new ContainerBuilder();
			_serviceCollection = new ServiceCollection();

			_viewAssemblies = new HashSet<Assembly>();

			this.AddDefaults();
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

		public BlasticApplication Configure(Action<LoggerConfiguration> action)
		{
			action(_loggerConfiguration);
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
					.WithTypeMapper<DynamicModel, DynamicControl>()
					.WithTypeMapper(new SuffixTypeMapper(viewAssemblies, "View", "ViewModel"));

				builder
					.RegisterInstance(viewLocator)
					.AsImplementedInterfaces()
					.AsSelf()
					.SingleInstance();
			});
		}

		public void Run<TApp, TMainViewModel>()
		{
			IConfiguration configuration = _configurationBuilder.Build();

			_loggerConfiguration.ReadFrom.Configuration(configuration);

			RegisterViewAssembly<BlasticApplication>();
			AddViewLocator(_viewAssemblies);

			Configure(x => x.RegisterInstance(configuration));
			Configure(x => x.RegisterType<TApp>().SingleInstance());
			Configure(x => x.RegisterType<TMainViewModel>().SingleInstance());
			Configure(x => x.RegisterInstance(new MainViewModelDescriptor(typeof(TMainViewModel))));

			_containerBuilder.Populate(_serviceCollection);
			IContainer container = _containerBuilder.Build();

			ViewLocator = container.Resolve<IViewLocator>();

			LogSink logSink = container.ResolveOptional<LogSink>();

			if (logSink != null)
			{
				_loggerConfiguration.WriteTo.Sink(logSink);
			}

			Log.Logger = _loggerConfiguration.CreateLogger();
			
			try
			{
				Application.Start(x =>
				{
					container.Resolve<TApp>();
				});
			}
			catch (Exception exception)
			{
				Log.Error(exception, exception.Message);
				throw;
			}
		}
	}
}