using System;
using System.IO;
using System.Net.Http;
using Blastic.Data;
using Blastic.Forms.Data.ProgramData.Migrations;
using Blastic.Forms.Initialization.Extensions;
using Blastic.Forms.Sample.Data;
using Blastic.Forms.Sample.Data.Migrations;
using Blastic.Forms.Sample.Services;
using Blastic.Forms.Sample.UserInterface;
using Blastic.Forms.Sample.UserInterface.MediaPlayer;
using Blastic.Forms.Sample.UserInterface.Settings;
using Blastic.Forms.Sample.UserInterface.Settings.Languages;
using Blastic.Forms.Sample.UserInterface.Settings.Themes;
using Blastic.Initialization.Steps;
using Blastic.Ordering;
using Blastic.Services.Localization;
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
				"Database.sqlite");

			hostBuilder
				.ConfigureBlasticApplication(
					applicationBuilder =>
					{
						applicationBuilder
							.UseApplication<App>()
							.UseApplicationRunner(applicationRunner)
							.AddShellTab<HomeViewModel>()
							.AddShellTab<SearchViewModel>()
							.AddShellTab<LibraryViewModel>()
							.AddShellTab<SettingsViewModel>()
							.UseMainViewModel<MainViewModel>()
							.AddSettingsService()
							.AddProgramDatabase<ProgramDatabase>(DatabaseProvider.SQLite, $"Data Source={databasePath};");
					})
				.AddMigrations()
				.ConfigureServices(
					(x, y) =>
					{
						y.AddSingleton(new HttpClient());
						y.AddSingleton<MediaPlayerViewModel>();
						y.AddSingleton<ArchiveOrgService>();
						y.AddSingleton<DownloadService>();
						y.AddSingleton<ILocalizationSource>(new Resources.LocalizationSource(Order.AbsoluteMaximum));
						y.AddSingleton<Resources.LocalizableProperties>();
						y.AddSingleton<IInitializationStep, ReadSettingsStep>();
						
						y.AddSingleton<ThemeSettingsSection>();
						y.AddSingleton<LanguageSettingsSection>();
					});

			return hostBuilder;
		}

		public static IHostBuilder AddMigrations(this IHostBuilder hostBuilder)
		{
			hostBuilder.ConfigureServices((x, y) =>
			{
				void AddMigration<T>() where T : ProgramDatabaseMigrationBase
				{
					y.AddSingleton<ProgramDatabaseMigrationBase, T>();
				}

				AddMigration<CreateBooksTable>();
			});

			return hostBuilder;
		}
	}
}