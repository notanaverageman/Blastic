using System.Threading;
using System.Threading.Tasks;
using Blastic.Common;
using Blastic.Data.ProviderSpecific;

namespace Blastic.Data.ProgramData.Migrations
{
	public class CreateSettingsTable : ProgramDatabaseMigrationBase
	{
		public override Version Version { get; } = new Version(0, 0, 0);

		public override async Task MigrateUp(Connection connection, CancellationToken cancellationToken)
		{
			ProviderSpecifics providerSpecifics = connection.ProviderSpecifics;

			using Command command = connection.CreateCommand();

			command.CommandText = $@"CREATE TABLE Settings (
                                        Key   {providerSpecifics.NVarCharMaxColumn} PRIMARY KEY,
                                        Value {providerSpecifics.NVarCharMaxColumn}
                                    );";

			await command.ExecuteNonQuery(cancellationToken);
		}

		public override async Task MigrateDown(Connection connection, CancellationToken cancellationToken)
		{
			using Command command = connection.CreateCommand();

			command.CommandText = "DROP TABLE Settings";
			await command.ExecuteNonQuery(cancellationToken);
		}
	}
}