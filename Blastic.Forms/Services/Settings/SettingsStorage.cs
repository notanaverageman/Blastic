using System;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Services.Settings;
using Xamarin.Essentials;

namespace Blastic.Forms.Services.Settings
{
	public class SettingsStorage : ISettingsStorage
	{
		public Task<bool> Contains(string key, CancellationToken cancellationToken)
		{
			return Task.FromResult(Preferences.ContainsKey(key));
		}

		public async Task<T> Get<T>(string key, T defaultValue, CancellationToken cancellationToken)
		{
			bool contains = await Contains(key, cancellationToken);

			if (!contains)
			{
				return defaultValue;
			}

			Type type = defaultValue.GetType();

			if (type.IsEnum)
			{
				string s = Preferences.Get(key, Enum.GetName(type, defaultValue));
				return (T)Enum.Parse(type, s);
			}

			object value = defaultValue switch
			{
				bool x => Preferences.Get(key, x),
				int x => Preferences.Get(key, x),
				long x => Preferences.Get(key, x),
				float x => Preferences.Get(key, x),
				double x => Preferences.Get(key, x),
				string x => Preferences.Get(key, x),
				DateTime x => Preferences.Get(key, x),
				_ => throw new ArgumentException($"{type} is not supported.")
			};

			return (T)value;
		}

		public Task Put<T>(string key, T value, CancellationToken cancellationToken)
		{
			Type type = value.GetType();

			if (type.IsEnum)
			{
				Preferences.Set(key, Enum.GetName(type, value));
				return Task.CompletedTask;
			}
			
			switch (value)
			{
				case bool x:
					Preferences.Set(key, x);
					break;
				case int x:
					Preferences.Set(key, x);
					break;
				case long x:
					Preferences.Set(key, x);
					break;
				case float x:
					Preferences.Set(key, x);
					break;
				case double x:
					Preferences.Set(key, x);
					break;
				case string x:
					Preferences.Set(key, x);
					break;
				case DateTime x:
					Preferences.Set(key, x);
					break;
				default:
					throw new ArgumentException($"{type} is not supported.");
			}

			return Task.CompletedTask;
		}

		public Task Delete(string key, CancellationToken cancellationToken)
		{
			Preferences.Remove(key);
			return Task.CompletedTask;
		}
	}
}