using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Services.Settings;
using Blastic.Wpf.Data.ProgramData;

namespace Blastic.Wpf.Services.Settings
{
	public class DatabaseSettingsService : ISettingsService
	{
		private readonly ProgramDatabase _database;

		public DatabaseSettingsService(ProgramDatabase database)
		{
			_database = database;
		}

		public async Task<bool> Contains(string key, CancellationToken cancellationToken)
		{
			return await _database.SettingsTable.Contains(key, cancellationToken);
		}

		public async Task<T> Get<T>(string key, T defaultValue, CancellationToken cancellationToken)
		{
			string serializedData = await _database.SettingsTable.Get(key, cancellationToken);

			if (serializedData == null)
			{
				return defaultValue;
			}

			return JsonSerializer.Deserialize<T>(serializedData);
		}

		public async Task Put<T>(string key, T value, CancellationToken cancellationToken)
		{
			string serializedData = JsonSerializer.Serialize(value);
			await _database.SettingsTable.Put(key, serializedData, cancellationToken);
		}

		public async Task Delete(string key, CancellationToken cancellationToken)
		{
			await _database.SettingsTable.Delete(key, cancellationToken);
		}
	}
}