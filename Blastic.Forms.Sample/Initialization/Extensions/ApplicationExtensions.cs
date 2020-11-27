using System;
using System.IO;
using System.Net.Http;
using Blastic.Data;
using Blastic.Forms.Data.ProgramData.Migrations;
using Blastic.Forms.Initialization.Extensions;
using Blastic.Forms.Sample.Data;
using Blastic.Forms.Sample.Data.Migrations;
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
				"Database.sqlite");

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
							.UseMainViewModel<MainViewModel>()
							.AddProgramDatabase<ProgramDatabase>(DatabaseProvider.SQLite, $"Data Source={databasePath};");
					})
				.AddMigrations()
				.ConfigureServices(
					(x, y) =>
					{
						y.AddSingleton<Labels>();
						y.AddSingleton(new HttpClient());
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