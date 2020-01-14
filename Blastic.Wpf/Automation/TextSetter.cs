using System;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Reactive;

namespace Blastic.Wpf.Automation
{
	public static partial class AutomationExtensions
	{
		public static async Task SetText(
			this IReactiveProperty<string> property,
			string text,
			TimeSpan? duration = null)
		{
			await Task.Run(() =>
			{
				duration ??= text.Length * TimeSpan.FromMilliseconds(100);

				double waitDurationMilliseconds = duration.Value.TotalMilliseconds / Math.Max(text.Length, 1);

				for (int i = 0; i < text.Length; i++)
				{
					property.Value = text.Substring(0, i + 1);

					if (waitDurationMilliseconds > 0)
					{
						Thread.Sleep((int)waitDurationMilliseconds);
					}
				}
			});
		}
	}
}