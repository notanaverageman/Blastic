using System.Collections.Generic;

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
		public bool Contains(string key)
		{
			return _settings.ContainsKey(key);
		}

		/// <inheritdoc />
		public T? Get<T>(string key, T? defaultValue = default)
		{
			if (!_settings.TryGetValue(key, out object? value) || value is not T t)
			{
				return defaultValue;
			}

			return t;
		}

		/// <inheritdoc />
		public void Put<T>(string key, T value)
		{
			_settings[key] = value;
		}

		/// <inheritdoc />
		public void Delete(string key)
		{
			_settings.Remove(key);
		}
	}
}