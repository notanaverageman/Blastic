using Microsoft.Data.Sqlite;

namespace Blastic.Maui.Sample.Data;

public class GameDatabaseOptions
{
	public SqliteConnectionStringBuilder ConnectionStringBuilder { get; }

	public GameDatabaseOptions()
	{
		ConnectionStringBuilder = new SqliteConnectionStringBuilder
		{
			DataSource = ":memory:"
		};
	}
}