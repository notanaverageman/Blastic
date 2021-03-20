using Blastic.Data.Migrations;
using Blastic.Data.Services.Settings;
using Blastic.Services.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Blastic.Data
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddDatabase<TDatabase>(
			this IServiceCollection serviceCollection,
			DatabaseProvider databaseProvider,
			string connectionString)
			where TDatabase : DatabaseBase
		{
			DatabaseConfiguration databaseConfiguration = new(databaseProvider, connectionString);

			serviceCollection.AddSingleton(_ => databaseConfiguration);
			serviceCollection.AddSingleton<ConnectionFactory>();
			serviceCollection.AddSingleton<TDatabase>();

			return serviceCollection;
		}
		
		public static IServiceCollection AddDatabaseSettingsStorage(this IServiceCollection serviceCollection)
		{
			serviceCollection.AddSingleton<SettingsTable>();
			serviceCollection.AddSingleton<MigrationBase, CreateSettingsTable>();
			serviceCollection.AddSingleton<ISettingsStorage, DatabaseSettingsStorage>();

			return serviceCollection;
		}
	}
}