using System;
using System.Globalization;

namespace Blastic.Services.Localization
{
	public class CultureChangedEventArgs : EventArgs
	{
		public CultureInfo Culture { get; }

		public CultureChangedEventArgs(CultureInfo culture)
		{
			Culture = culture;
		}
	}
}