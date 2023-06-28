using Blastic.Ordering;

namespace Blastic.Data.Tables;

public class MetadataTable : TableBase
{
	public string TableName { get; }

	public MetadataTable(Connection connection, string tableName) : base(connection)
	{
		TableName = tableName;
	}

	public Version? GetVersion()
	{
		if (!Exists())
		{
			return null;
		}

		using Command command = Connection.CreateCommand();
		command.CommandText = $"SELECT Version FROM {TableName}";

		string? versionAsString = command.ExecuteScalar<string>();

		return string.IsNullOrEmpty(versionAsString)
			? null
			: Version.Parse(versionAsString!);
	}

	public void SetVersion(Version version)
	{
		using Command command = Connection.CreateCommand();

		command.CommandText = $"""
			UPDATE {TableName} SET Version=@Version
			""";

		command.AddParameterWithValue("@Version", version.ToString());
		command.ExecuteNonQuery();
	}

	private bool Exists()
	{
		using Command command = Connection.CreateCommand();
		command.CommandText = $"""
			SELECT 1 FROM sqlite_master
			WHERE type='table' AND name='{TableName}'
			""";

		int? count = command.ExecuteScalar<int>();

		return count > 0;
	}
}