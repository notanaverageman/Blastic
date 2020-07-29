using System;
using System.IO;
using System.Net.Http;
using Blastic.Data;
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
			string databasePath = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				"Settings.sqlite");

			hostBuilder
				.ConfigureBlasticApplication(
					applicationBuilder =>
					{
						applicationBuilder
							.UseApplication<App>()
							.UseApplicationRunner(applicationRunner)
							.AddLocalizationSource(Properties.Resources.ResourceManager)
							.AddShellTab<HomeViewModel>()
							.UseMainViewModel<MainViewModel>()
							.AddProgramDatabase(DatabaseProvider.SQLite, $"Data Source={databasePath};")
							.AddSettingsService();
					})
				.ConfigureServices(
					(x, y) =>
					{
						y.AddSingleton<Labels>();
						y.AddSingleton<HttpClient>();
					});

			return hostBuilder;
		}
	}
}