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
				// One for initial value raised on construction and one for the value raised
				// when the setting is read.
				.Skip(2)
				.Subscribe(async _ => await setting.Save(CancellationToken.None));
		}
	}
}