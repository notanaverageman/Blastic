using Blastic.Ordering;

namespace Blastic.Data.Tables;

public class MetadataTable : TableBase
{
	private readonly string _tableName;

	public MetadataTable(Connection connection, string tableName) : base(connection)
	{
		_tableName = tableName;
	}

	public Version? GetVersion()
	{
		if (!Exists())
		{
			return null;
		}

		using Command command = Connection.CreateCommand();
		command.CommandText = $"SELECT Version FROM {_tableName}";

		string? versionAsString = command.ExecuteScalar<string>();

		return string.IsNullOrEmpty(versionAsString)
			? null
			: Version.Parse(versionAsString!);
	}

	public void SetVersion(Version version)
	{
		using Command command = Connection.CreateCommand();

		command.CommandText = $"""
			UPDATE {_tableName} SET Version=@Version
			""";

		command.AddParameterWithValue("@Version", version.ToString());
		command.ExecuteNonQuery();
	}

	private bool Exists()
	{
		using Command command = Connection.CreateCommand();
		command.CommandText = $"""
			SELECT 1 FROM sqlite_master
			WHERE type='table' AND name='{_tableName}'
			""";

		int? count = command.ExecuteScalar<int>();

		return count > 0;
	}
}