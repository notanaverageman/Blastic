using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Blastic.Services.Settings
{
	public class InMemorySettingsService : ISettingsService
	{
		private readonly Dictionary<string, object?> _settings;

		public InMemorySettingsService()
		{
			_settings = new Dictionary<string, object?>();
		}

		public Task<bool> Contains(string key, CancellationToken cancellationToken = default)
		{
			bool contains = _settings.ContainsKey(key);
			return Task.FromResult(contains);
		}

		public Task<T> Get<T>(string key, T defaultValue = default, CancellationToken cancellationToken = default)
		{
			if (!_settings.TryGetValue(key, out object? value) || !(value is T t))
			{
				return Task.FromResult(defaultValue);
			}

			return Task.FromResult(t);
		}

		public Task Put<T>(string key, T value, CancellationToken cancellationToken = default)
		{
			_settings[key] = value;
			return Task.CompletedTask;
		}

		public Task Delete(string key, CancellationToken cancellationToken = default)
		{
			_settings.Remove(key);
			return Task.CompletedTask;
		}
	}
}