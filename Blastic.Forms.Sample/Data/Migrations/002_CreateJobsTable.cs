using System.Threading;
using System.Threading.Tasks;
using Blastic.Data;
using Blastic.Data.ProviderSpecific;
using Blastic.Forms.Data.ProgramData.Migrations;
using Blastic.Ordering;

namespace Blastic.Forms.Sample.Data.Migrations
{
	public class CreateJobsTable : ProgramDatabaseMigrationBase
	{
		public override Version Version { get; } = new Version(1, 0, 1);

		public override async Task MigrateUp(Connection connection, CancellationToken cancellationToken)
		{
			using Command command = connection.CreateCommand();

			ProviderSpecifics providerSpecifics = connection.ProviderSpecifics;

			command.CommandText = $@"CREATE TABLE Jobs (
                                        Id         INTEGER PRIMARY KEY {providerSpecifics.IdentityColumn},
                                        MachineId  INTEGER,
										Name       {providerSpecifics.NVarCharMaxColumn},
										IsStarted  INTEGER,
										QueueDate  DATETIME,
										StartDate  DATETIME,
										StartFrame INTEGER,
										EndFrame   INTEGER
                                    );";

			await command.ExecuteNonQuery(cancellationToken);
		}

		public override async Task MigrateDown(Connection connection, CancellationToken cancellationToken)
		{
			using Command command = connection.CreateCommand();

			command.CommandText = "DROP TABLE Jobs";
			await command.ExecuteNonQuery(cancellationToken);
		}
	}
}