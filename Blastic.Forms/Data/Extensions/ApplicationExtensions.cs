using Blastic.Data;
using Blastic.Forms.Data.ProgramData;
using Blastic.Forms.Data.Steps;
using Blastic.Forms.Initialization;
using Blastic.Forms.Initialization.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Blastic.Forms.Data.Extensions
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

					x.AddLogging();
				})
				.AddInitializationStep<MigrateProgramDatabaseStep>();
		}
	}
}