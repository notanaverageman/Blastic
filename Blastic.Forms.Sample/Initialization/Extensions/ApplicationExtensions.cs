using System;
using System.Net.Http;
using Blastic.Forms.Initialization.Extensions;
using Blastic.Forms.Sample.Localization;
using Blastic.Forms.Sample.UserInterface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xamarin.Forms;

namespace Blastic.Forms.Sample.Initialization.Extensions
{
	public static class ApplicationExtensions
	{
		public static IHostBuilder Initialize(this IHostBuilder hostBuilder, Action<Application> applicationRunner)
		{
			hostBuilder
				.ConfigureBlasticApplication(
					applicationBuilder =>
					{
						applicationBuilder
							.UseApplication<App>()
							.UseApplicationRunner(applicationRunner)
							.AddLocalizationSource(Properties.Resources.ResourceManager)
							.AddShellTab<HomeViewModel>()
							.AddShellTab<SearchViewModel>()
							.AddShellTab<LibraryViewModel>()
							.UseMainViewModel<MainViewModel>();
					})
				.ConfigureServices(
					(x, y) =>
					{
						y.AddSingleton<Labels>();
						y.AddSingleton(new HttpClient());
					});

			return hostBuilder;
		}
	}
}