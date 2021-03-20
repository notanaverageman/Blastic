using System.Threading;
using System.Threading.Tasks;
using Blastic.Data.Migrations;
using Blastic.Data.ProviderSpecific;
using Blastic.Ordering;

namespace Blastic.Data.Services.Settings
{
	public class CreateSettingsTable : MigrationBase
	{
		public override Version Version { get; } = new(int.MinValue, 1, 0);

		public override async Task MigrateUp(Connection connection, CancellationToken cancellationToken)
		{
			ProviderSpecifics providerSpecifics = connection.ProviderSpecifics;

			using Command command = connection.CreateCommand();

			command.CommandText = $@"CREATE TABLE Settings (
                                        Setting NVARCHAR(255) PRIMARY KEY,
                                        Value   {providerSpecifics.NVarCharMaxColumn}
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