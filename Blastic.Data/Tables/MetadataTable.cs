using Blastic.Ordering;

namespace Blastic.Data.Tables;

public class MetadataTable : TableBase
{
	public MetadataTable(Connection connection) : base(connection)
	{
	}

	public Version? GetVersion()
	{
		if (!Exists())
		{
			return null;
		}

		using Command command = Connection.CreateCommand();
		command.CommandText = "SELECT Version FROM Metadata";

		string? versionAsString = command.ExecuteScalar<string>();

		return string.IsNullOrEmpty(versionAsString)
			? null
			: Version.Parse(versionAsString!);
	}

	public void SetVersion(Version version)
	{
		using Command command = Connection.CreateCommand();

		command.CommandText = """
			UPDATE Metadata SET Version=@Version
			""";

		command.AddParameterWithValue("@Version", version.ToString());
		command.ExecuteNonQuery();
	}

	private bool Exists()
	{
		using Command command = Connection.CreateCommand();
		command.CommandText = """
			SELECT 1 FROM sqlite_master
			WHERE type='table' AND name='Metadata'
			""";

		int? count = command.ExecuteScalar<int>();

		return count > 0;
	}
}