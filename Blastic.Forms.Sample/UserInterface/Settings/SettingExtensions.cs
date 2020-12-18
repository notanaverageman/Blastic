using System;
using System.Reactive.Linq;
using System.Threading;
using Blastic.Settings;

namespace Blastic.Forms.Sample.UserInterface.Settings
{
	public static class SettingExtensions
	{
		public static void SaveOnChange<T>(this Setting<T> setting)
		{
			setting.ReactiveSettingValue
				// Skip the value raised when the setting is read.
				.Skip(1)
				.Subscribe(async _ => await setting.Save(CancellationToken.None));
		}
	}
}