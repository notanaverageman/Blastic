using System.Threading;
using System.Threading.Tasks;
using Blastic.Data;
using Blastic.Data.ProviderSpecific;
using Blastic.Forms.Data.ProgramData.Migrations;
using Blastic.Ordering;

namespace Blastic.Forms.Sample.Data.Migrations
{
	public class CreateMachinesTable : ProgramDatabaseMigrationBase
	{
		public override Version Version { get; } = new Version(1, 0, 0);

		public override async Task MigrateUp(Connection connection, CancellationToken cancellationToken)
		{
			using Command command = connection.CreateCommand();

			ProviderSpecifics providerSpecifics = connection.ProviderSpecifics;

			command.CommandText = $@"CREATE TABLE Machines (
                                        Id              INTEGER PRIMARY KEY {providerSpecifics.IdentityColumn},
                                        Name            NVARCHAR(255),
                                        SecondsPerFrame INTEGER
                                    );";

			await command.ExecuteNonQuery(cancellationToken);
		}

		public override async Task MigrateDown(Connection connection, CancellationToken cancellationToken)
		{
			using Command command = connection.CreateCommand();

			command.CommandText = "DROP TABLE Machines";
			await command.ExecuteNonQuery(cancellationToken);
		}
	}
}