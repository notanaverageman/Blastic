using Blastic.Ordering;

namespace Blastic.Data.Migrations;

public class CreateMetadataTable : MigrationBase
{
	private readonly string _tableName;

	public static readonly Version StaticVersion = new(0, 0, 0, 0);

	public override Version Version => StaticVersion;

	public CreateMetadataTable(Connection connection, string tableName) : base(connection)
	{
		_tableName = tableName;
	}

	public override void MigrateUp()
	{
		using Command command = Connection.CreateCommand();

		command.CommandText = $"""
			CREATE TABLE {_tableName}(
				Version NVARCHAR(255) PRIMARY KEY  
			)
			""";
		command.ExecuteNonQuery();

		command.CommandText = $"""
			INSERT INTO {_tableName} (Version)
			VALUES (@Version)
			""";

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