using System.Threading;
using System.Threading.Tasks;
using Blastic.Common;

namespace Blastic.Data.Migrations
{
	public class CreateDatabaseInformationTable : MigrationBase
	{
		public static readonly Version StaticVersion = new Version(0, 0, 0);

		public override Version Version => StaticVersion;

		public override async Task MigrateUp(Connection connection, CancellationToken cancellationToken)
		{
			using Command command = connection.CreateCommand();

			command.CommandText = "CREATE TABLE DatabaseInformation(Version NVARCHAR(255) PRIMARY KEY)";
			await command.ExecuteNonQuery(cancellationToken);

			command.CommandText = "INSERT INTO DatabaseInformation (Version) VALUES (@Version)";
			command.AddParameterWithValue("@Version", Version.ToString());

			await command.ExecuteNonQuery(cancellationToken);
		}

		public override async Task MigrateDown(Connection connection, CancellationToken cancellationToken)
		{
			using Command command = connection.CreateCommand();

			command.CommandText = "DROP TABLE DatabaseInformation";
			await command.ExecuteNonQuery(cancellationToken);
		}
	}
}