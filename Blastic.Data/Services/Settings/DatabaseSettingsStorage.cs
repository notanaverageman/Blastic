using System.Text.Json;
using Blastic.Services.Settings;

namespace Blastic.Data.Services.Settings
{
	public class DatabaseSettingsStorage : ISettingsStorage
	{
		private readonly SettingsTable _settingsTable;

		public DatabaseSettingsStorage(SettingsTable settingsTable)
		{
			_settingsTable = settingsTable;
		}

		public bool Contains(string key)
		{
			return _settingsTable.Contains(key);
		}

		public T? Get<T>(string key, T? defaultValue)
		{
			string? serializedData = _settingsTable.Get(key);

			if (serializedData == null)
			{
				return defaultValue;
			}

			return JsonSerializer.Deserialize<T>(serializedData);
		}

		public void Put<T>(string key, T value)
		{
			string serializedData = JsonSerializer.Serialize(value);
			_settingsTable.Put(key, serializedData);
		}

		public void Delete(string key)
		{
			_settingsTable.Delete(key);
		}
	}
}