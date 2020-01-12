using System;
using System.Globalization;

namespace Blastic.Services.Localization
{
	public interface ILocalizationService
	{
		IObservable<CultureInfo> Culture { get; }

		string GetValue(string key);
		void SetCulture(CultureInfo culture);
	}
}