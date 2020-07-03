using System;
using System.Linq;
using System.Threading;
using Blastic.Commanding;
using Blastic.DynamicControls;
using Blastic.Forms.DynamicControls;
using Blastic.Forms.Initialization.Extensions;
using Blastic.Forms.Platform;
using Blastic.Forms.ViewManagement;
using Blastic.LifetimeManagement;
using Blastic.LifetimeManagement.Contexts;
using Blastic.Platform;
using Blastic.ViewManagement;
using Blastic.ViewManagement.TypeMappers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xamarin.Forms;

namespace Blastic.Forms.Initialization
{
	public class BlasticApplication
	{
		internal static IViewLocator<VisualElement> ViewLocator;

		private readonly Action<Application> _applicationRunner;

		private readonly ConfigurationBuilder _configurationBuilder;
		private readonly ServiceCollection _serviceCollection;

		private readonly ViewLocator _viewLocator;

		public BlasticApplication(Action<Application> applicationRunner)
		{
			_applicationRunner = applicationRunner;

			_configurationBuilder = new ConfigurationBuilder();
			_serviceCollection = new ServiceCollection();

			_viewLocator = new ViewLocator(Enumerable.Empty<ITypeMapper>());

			this.AddDefaults();
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

		private void AddViewLocator()
		{
			Configure(x =>
			{
				_viewLocator
					.WithTypeMapper(new SuffixTypeMapper("View", "ViewModel"))
					.WithTypeMapper(new InheritanceTypeMapper(typeof(DynamicModel), typeof(DynamicControl)));

				x.AddSingleton<IViewLocator<VisualElement>>(y => _viewLocator);
				x.AddSingleton(y => _viewLocator);
			});
		}

		public void Run<TApp, TMainViewModel>() where TApp : Application where TMainViewModel : class
		{
			IConfiguration configuration = _configurationBuilder.Build();

			AddViewLocator();

			Configure(x => x.AddSingleton(configuration));
			Configure(x => x.AddSingleton<TMainViewModel>());
			Configure(x => x.AddSingleton<TApp>());

			ServiceProvider serviceProvider = _serviceCollection.BuildServiceProvider();

			ViewLocator = serviceProvider.GetRequiredService<IViewLocator<VisualElement>>();

			ILogger logger = serviceProvider.GetService<ILogger>();

			SynchronizationContext synchronizationContext = SynchronizationContext.Current;

			try
			{
				PlatformSpecifics.Current = new FormsPlatformSpecifics(synchronizationContext);

				TApp application = serviceProvider.GetRequiredService<TApp>();
				TMainViewModel viewModel = serviceProvider.GetRequiredService<TMainViewModel>();

				application.MainPage = ViewLocator.Locate(viewModel) as Page;

				PlatformSpecifics.Current.OnUIThread(async () =>
				{
					if (viewModel is IHasLifetime hasLifetime)
					{
						CommandContext<ActivationContext> commandContext = new CommandContext<ActivationContext>(
							new ActivationContext(),
							CancellationToken.None);

						await hasLifetime.Lifetime.Activate.Execute(commandContext);
					}
				});

				_applicationRunner(application);
			}
			catch (Exception exception)
			{
				logger?.LogError(exception, exception.Message);
				throw;
			}
		}
	}
}