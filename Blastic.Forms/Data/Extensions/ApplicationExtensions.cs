using Blastic.Data;
using Blastic.Forms.Data.ProgramData;
using Blastic.Forms.Data.ProgramData.Migrations;
using Blastic.Forms.Data.Steps;
using Blastic.Forms.Initialization;
using Blastic.Forms.Initialization.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Blastic.Forms.Data.Extensions
{
	public static class ApplicationExtensions
	{
		public static BlasticApplication AddProgramDatabase<T>(
			this BlasticApplication application,
			DatabaseProvider databaseProvider,
			string connectionString)
			where T : ProgramDatabase
		{
			return application
				.Configure(x =>
				{
					DatabaseConfiguration databaseConfiguration = new DatabaseConfiguration(databaseProvider, connectionString);

					x.AddSingleton(y => databaseConfiguration);
					x.AddSingleton<ConnectionFactory>();
					x.AddSingleton<T>();
					x.AddSingleton<ProgramDatabase>(y => y.GetService<T>());

                    x.AddSingleton<ProgramDatabaseMigrationBase, CreateSettingsTable>();

					x.AddLogging();
				})
				.AddInitializationStep<MigrateProgramDatabaseStep>();
		}
	}
}