using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Blastic.Services.Localization
{
	/// <summary>
	/// Default implementation of <see cref="ILocalizationService"/> that uses an ordered
	/// list of <see cref="ILocalizationSource"/>s to provide localized strings.
	/// </summary>
	public class LocalizationService : ILocalizationService
	{
		/// <inheritdoc />
		public event EventHandler<CultureChangedEventArgs>? CultureChanged;

		private readonly ILocalizationSource[] _sources;

		private CultureInfo _culture;

		/// <inheritdoc />
		public CultureInfo Culture
		{
			get => _culture;
			set
			{
				_culture = value;
				CultureChanged?.Invoke(this, new CultureChangedEventArgs(_culture));
			}
		}

		/// <summary>
		/// Creates a new instance with given localization sources.
		/// </summary>
		/// <param name="sources">Localization sources that provide localized strings.</param>
		public LocalizationService(IEnumerable<ILocalizationSource> sources)
		{
			_culture = CultureInfo.InvariantCulture;

			_sources = sources
				.OrderBy(x => x.Order)
				.ToArray();
		}

		/// <inheritdoc />
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