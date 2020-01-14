using Blastic.Data;
using Blastic.Services.Settings;
using Blastic.Wpf.Data.Initialization.Steps;
using Blastic.Wpf.Data.ProgramData;
using Blastic.Wpf.Initialization;
using Blastic.Wpf.Initialization.Extensions;
using Blastic.Wpf.Services.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Blastic.Wpf.Data
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