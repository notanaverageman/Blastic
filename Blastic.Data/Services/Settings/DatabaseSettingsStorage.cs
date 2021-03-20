using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
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

		public async Task<bool> Contains(string key, CancellationToken cancellationToken)
		{
			return await _settingsTable.Contains(key, cancellationToken);
		}

		public async Task<T> Get<T>(string key, T defaultValue, CancellationToken cancellationToken)
		{
			string? serializedData = await _settingsTable.Get(key, cancellationToken);

			if (serializedData == null)
			{
				return defaultValue;
			}

			return JsonSerializer.Deserialize<T>(serializedData);
		}

		public async Task Put<T>(string key, T value, CancellationToken cancellationToken)
		{
			string serializedData = JsonSerializer.Serialize(value);
			await _settingsTable.Put(key, serializedData, cancellationToken);
		}

		public async Task Delete(string key, CancellationToken cancellationToken)
		{
			await _settingsTable.Delete(key, cancellationToken);
		}
	}
}