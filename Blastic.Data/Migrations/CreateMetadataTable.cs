using Blastic.Ordering;

namespace Blastic.Data.Migrations;

public class CreateMetadataTable : MigrationBase
{
	public static readonly Version StaticVersion = new(0, 0, 0, 0);

	public override Version Version => StaticVersion;

	public CreateMetadataTable(Connection connection) : base(connection)
	{
	}

	public override void MigrateUp()
	{
		using Command command = Connection.CreateCommand();

		command.CommandText = "CREATE TABLE Metadata(Version NVARCHAR(255) PRIMARY KEY)";
		command.ExecuteNonQuery();

		command.CommandText = "INSERT INTO Metadata (Version) VALUES (@Version)";
		command.AddParameterWithValue("@Version", Version.ToString());

		command.ExecuteNonQuery();
	}

	public override void MigrateDown()
	{
		using Command command = Connection.CreateCommand();

		command.CommandText = "DROP TABLE Metadata";
		command.ExecuteNonQuery();
	}
}