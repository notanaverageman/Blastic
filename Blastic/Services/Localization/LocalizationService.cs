using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Blastic.Services.Localization
{
	public class LocalizationService : ILocalizationService
	{
		public event EventHandler<CultureChangedEventArgs>? CultureChanged;

		private readonly ILocalizationSource[] _sources;

		private CultureInfo _culture;

		public CultureInfo Culture
		{
			get => _culture;
			set
			{
				_culture = value;
				CultureChanged?.Invoke(this, new CultureChangedEventArgs(_culture));
			}
		}

		public LocalizationService(IEnumerable<ILocalizationSource> sources)
		{
			_culture = CultureInfo.InvariantCulture;

			_sources = sources
				.OrderBy(x => x.Order)
				.ToArray();
		}

		public string? GetValue(string key)
		{
			string? result = null;

			foreach (ILocalizationSource source in _sources)
			{
				result = source.GetValue(key, _culture);

				if (!string.IsNullOrEmpty(result))
				{
					break;
				}
			}

			return result;
		}
	}
}