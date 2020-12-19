using System.Threading;
using System.Threading.Tasks;
using Blastic.Data;
using Blastic.Data.ProviderSpecific;
using Blastic.Ordering;

namespace Blastic.Forms.Sample.Data.Migrations
{
	public class CreateBooksTable : ProgramDatabaseMigrationBase
	{
		public override Version Version { get; } = new Version(1, 0, 0);

		public override async Task MigrateUp(Connection connection, CancellationToken cancellationToken)
		{
			using Command command = connection.CreateCommand();

			ProviderSpecifics providerSpecifics = connection.ProviderSpecifics;

			command.CommandText = $@"CREATE TABLE Books (
                                        Id              INTEGER PRIMARY KEY {providerSpecifics.IdentityColumn},
                                        ArchiveOrgId    {providerSpecifics.NVarCharMaxColumn},
                                        Title           {providerSpecifics.NVarCharMaxColumn},
                                        Description     {providerSpecifics.NVarCharMaxColumn}
                                    );";

			await command.ExecuteNonQuery(cancellationToken);
		}

		public override async Task MigrateDown(Connection connection, CancellationToken cancellationToken)
		{
			using Command command = connection.CreateCommand();

			command.CommandText = "DROP TABLE Books";
			await command.ExecuteNonQuery(cancellationToken);
		}
	}
}