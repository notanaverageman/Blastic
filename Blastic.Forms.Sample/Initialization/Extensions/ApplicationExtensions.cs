using System;
using System.IO;
using Blastic.Data;
using Blastic.Forms.Data.Extensions;
using Blastic.Forms.Data.ProgramData.Migrations;
using Blastic.Forms.Initialization;
using Blastic.Forms.Initialization.Extensions;
using Blastic.Forms.Sample.Data;
using Blastic.Forms.Sample.Data.Migrations;
using Blastic.Forms.Sample.Localization;
using Blastic.Forms.Sample.UserInterface;
using Blastic.Initialization.Steps;
using Microsoft.Extensions.DependencyInjection;

namespace Blastic.Forms.Sample.Initialization.Extensions
{
	public static class ApplicationExtensions
	{
		public static BlasticApplication Initialize(this BlasticApplication application)
		{
			string databasePath = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				"Settings.sqlite");

			application
				.AddLocalizationSource(Properties.Resources.ResourceManager)
				.AddShellTab<HomeViewModel>()
				.AddProgramDatabase<ProgramDatabase>(DatabaseProvider.SQLite, $"Data Source={databasePath};")
				.AddSettingsService()
                .AddMigrations()
				.Configure(x => x.AddSingleton<Labels>());

			return application;
		}

		public static BlasticApplication AddMigrations(this BlasticApplication application)
        {
            application.Configure(x =>
            {
                void AddMigration<T>() where T : ProgramDatabaseMigrationBase
                {
                    x.AddSingleton<ProgramDatabaseMigrationBase, T>();
                }

                AddMigration<CreateMachinesTable>();
                AddMigration<CreateJobsTable>();
            });

			return application;
		}
	}
}