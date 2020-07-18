using System;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Forms.Platform;
using Blastic.Forms.ViewManagement;
using Blastic.Platform;
using Blastic.ViewManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xamarin.Forms;

namespace Blastic.Forms.Initialization
{
	public class FormsHostedService : IHostedService
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly BlasticApplicationBuilder _builder;

		public FormsHostedService(
			IServiceProvider serviceProvider,
			BlasticApplicationBuilder builder)
		{
			_serviceProvider = serviceProvider;
			_builder = builder;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			SynchronizationContext synchronizationContext = SynchronizationContext.Current;
			SynchronizationContext.SetSynchronizationContext(synchronizationContext);

			object mainViewModel = _serviceProvider.GetRequiredService(_builder.MainViewModelType);
			Application application = _serviceProvider.GetRequiredService<Application>();

			PlatformSpecifics.Current = new FormsPlatformSpecifics(synchronizationContext);
			ViewLocator.Current = _serviceProvider.GetRequiredService<IViewLocator<VisualElement>>();

			application.MainPage = ViewLocator.Current.Locate(mainViewModel) as Page;

			_builder.ApplicationRunner.Invoke(application);

			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			Application application = _serviceProvider.GetRequiredService<Application>();
			application.Dispatcher.BeginInvokeOnMainThread(() => application.Quit());

			return Task.CompletedTask;
		}
	}
}