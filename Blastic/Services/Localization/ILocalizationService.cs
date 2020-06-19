using System;
using System.Globalization;

namespace Blastic.Services.Localization
{
	public interface ILocalizationService
	{
		event EventHandler<CultureChangedEventArgs> CultureChanged;

		CultureInfo Culture { get; set; }

		string GetValue(string key);
	}
}