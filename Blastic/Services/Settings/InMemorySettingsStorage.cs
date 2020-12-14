using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Blastic.Services.Settings
{
	/// <summary>
	/// Default implementation of <see cref="ISettingsStorage"/> that uses an in-memory
	/// <see cref="Dictionary{TKey,TValue}"/> to store the values.
	/// </summary>
	public class InMemorySettingsStorage : ISettingsStorage
	{
		private readonly Dictionary<string, object?> _settings;

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public InMemorySettingsStorage()
		{
			_settings = new Dictionary<string, object?>();
		}

		/// <inheritdoc />
		public Task<bool> Contains(string key, CancellationToken cancellationToken = default)
		{
			bool contains = _settings.ContainsKey(key);
			return Task.FromResult(contains);
		}

		/// <inheritdoc />
		public Task<T> Get<T>(string key, T defaultValue = default, CancellationToken cancellationToken = default)
		{
			if (!_settings.TryGetValue(key, out object? value) || !(value is T t))
			{
				return Task.FromResult(defaultValue);
			}

			return Task.FromResult(t);
		}

		/// <inheritdoc />
		public Task Put<T>(string key, T value, CancellationToken cancellationToken = default)
		{
			_settings[key] = value;
			return Task.CompletedTask;
		}

		/// <inheritdoc />
		public Task Delete(string key, CancellationToken cancellationToken = default)
		{
			_settings.Remove(key);
			return Task.CompletedTask;
		}
	}
}