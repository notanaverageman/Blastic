using Blastic.Data.Tables;

namespace Blastic.Data.Services.Settings
{
	public class SettingsTable : TableBase
	{
		public SettingsTable(Connection connection) : base(connection)
		{
		}

		public bool Contains(string key)
		{
			using Command command = Connection.CreateCommand();

			command.CommandText = "SELECT COUNT(*) FROM Settings WHERE Key=@Key";
			command.AddParameterWithValue("@Key", key);

			return command.ExecuteScalar<int>() > 0;
		}

		public string? Get(string key)
		{
			using Command command = Connection.CreateCommand();

			command.CommandText = "SELECT * FROM Settings WHERE Key=@Key";
			command.AddParameterWithValue("@Key", key);

			using DataReader reader = command.ExecuteReader();

			if (!reader.Read())
			{
				return null;
			}

			return reader.Get<string>("Value");
		}

		public void Put(string key, string value)
		{
			using Command command = Connection.CreateCommand();

			command.CommandText = @"INSERT OR REPLACE INTO Settings (Key, Value) VALUES (@Key, @Value)";

			command.AddParameterWithValue("@Key", key);
			command.AddParameterWithValue("@Value", value);

			command.ExecuteNonQuery();
		}

		public void Delete(string key)
		{
			using Command command = Connection.CreateCommand();

			command.CommandText = @"DELETE FROM Settings WHERE Key=@Key";
			command.AddParameterWithValue("@Key", key);

			command.ExecuteNonQuery();
		}
	}
}