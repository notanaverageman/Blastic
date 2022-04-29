using System;
using Blastic.Services.Settings;
using Xamarin.Essentials;

namespace Blastic.Forms.Services.Settings
{
	public class SettingsStorage : ISettingsStorage
	{
		public bool Contains(string key)
		{
			return Preferences.ContainsKey(key);
		}

		public T Get<T>(string key, T defaultValue)
		{
			bool contains = Contains(key);

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

		public void Put<T>(string key, T value)
		{
			Type type = value.GetType();

			if (type.IsEnum)
			{
				Preferences.Set(key, Enum.GetName(type, value));
				return;
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
		}

		public void Delete(string key)
		{
			Preferences.Remove(key);
		}
	}
}