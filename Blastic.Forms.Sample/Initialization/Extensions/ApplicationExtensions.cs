using System;
using System.IO;
using System.Net.Http;
using Blastic.Data.Migrations;
using Blastic.Forms.Initialization.Extensions;
using Blastic.Forms.Sample.Data;
using Blastic.Forms.Sample.Data.Migrations;
using Blastic.Forms.Sample.Services;
using Blastic.Forms.Sample.UserInterface.Chapters;
using Blastic.Forms.Sample.UserInterface.Downloads;
using Blastic.Forms.Sample.UserInterface.Home;
using Blastic.Forms.Sample.UserInterface.Library;
using Blastic.Forms.Sample.UserInterface.Main;
using Blastic.Forms.Sample.UserInterface.MediaPlayer;
using Blastic.Forms.Sample.UserInterface.Notifications;
using Blastic.Forms.Sample.UserInterface.Search;
using Blastic.Forms.Sample.UserInterface.Settings;
using Blastic.Forms.Sample.UserInterface.Settings.Languages;
using Blastic.Forms.Sample.UserInterface.Settings.Themes;
using Blastic.Ordering;
using Blastic.Services.Localization;
using Blastic.Services.Notifications;
using Microsoft.Data.Sqlite;
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
							.AddTypeMapper<Notification, NotificationView>()
							.AddSettingsStorage();
					})
				.AddProgramDatabase(new SqliteConnectionStringBuilder
				{
					DataSource = databasePath
				})
				.ConfigureServices(
					(_, y) =>
					{
						y.AddSingleton(new HttpClient());
						y.AddSingleton<ArchiveOrgService>();
						y.AddSingleton<DownloadService>();
						y.AddSingleton<ILocalizationSource>(new Resources.LocalizationSource(Order.AbsoluteMaximum));
						y.AddSingleton<Resources.LocalizableProperties>();

						y.AddSingleton<NotificationsViewModel>();
						y.AddSingleton<DownloadsViewModel>();
						y.AddSingleton<MediaPlayerViewModel>();
						y.AddSingleton<ChapterDetailsViewModel>();

						y.AddSingleton<ThemeSettingsSection>();
						y.AddSingleton<LanguageSettingsSection>();
					});

			return hostBuilder;
		}

		public static IHostBuilder AddProgramDatabase(
			this IHostBuilder hostBuilder,
			SqliteConnectionStringBuilder connectionStringBuilder)
		{
			hostBuilder.ConfigureServices((_, x) =>
			{
				x.AddSingleton(connectionStringBuilder);
				x.AddSingleton<ProgramDatabase>();
				x.AddSingleton<MigrationBase, CreateBooksTable>();
			});

			return hostBuilder;
		}
	}
}