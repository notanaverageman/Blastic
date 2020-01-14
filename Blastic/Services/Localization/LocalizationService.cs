using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Subjects;

namespace Blastic.Services.Localization
{
	public class LocalizationService : ILocalizationService
	{
		private readonly ILocalizationSource[] _sources;
		private readonly Subject<CultureInfo> _cultureSource;

		private CultureInfo _currentCulture;

		public IObservable<CultureInfo> Culture => _cultureSource;

		public LocalizationService(IEnumerable<ILocalizationSource> sources)
		{
			_sources = sources
				.OrderBy(x => x.Order)
				.ToArray();

			_cultureSource = new Subject<CultureInfo>();
		}

		public string GetValue(string key)
		{
			string result = null;

			foreach (ILocalizationSource source in _sources)
			{
				result = source.GetValue(key, _currentCulture);

				if (!string.IsNullOrEmpty(result))
				{
					break;
				}
			}

			return result;
		}

		public void SetCulture(CultureInfo culture)
		{
			_currentCulture = culture;
			_cultureSource.OnNext(culture);
		}
	}
}