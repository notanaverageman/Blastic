using Blastic.Data.Initialization.Steps;
using Blastic.Data.ProgramData;
using Blastic.Initialization;
using Blastic.Initialization.Extensions;
using Blastic.Services.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Blastic.Data
{
	public static class ApplicationExtensions
	{
		public static BlasticApplication AddProgramDatabase(
			this BlasticApplication application,
			DatabaseProvider databaseProvider,
			string connectionString)
		{
			return application
				.Configure(x =>
				{
					DatabaseConfiguration databaseConfiguration = new DatabaseConfiguration(databaseProvider, connectionString);

					x.AddSingleton(y => databaseConfiguration);
					x.AddSingleton<ConnectionFactory>();
					x.AddSingleton<ProgramDatabase>();
				})
				.AddInitializationStep<MigrateProgramDatabaseStep>();
		}

		public static BlasticApplication AddSettingsService(this BlasticApplication application)
		{
			return application.Configure(x =>
			{
				x.AddSingleton<ISettingsService, SettingsService>();
			});
		}
	}
}