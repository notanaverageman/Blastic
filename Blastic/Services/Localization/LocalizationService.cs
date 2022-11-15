using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Blastic.Commanding;
using Blastic.Reactive;

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

		private readonly IReactiveProperty<CultureInfo> _culture;
		private readonly ILocalizationSource[] _sources;

		/// <inheritdoc />
		public IReadOnlyReactiveProperty<CultureInfo> Culture => _culture;

		/// <inheritdoc />
		public Command<string> ChangeCultureCommand { get; }

		/// <summary>
		/// Creates a new instance with given localization sources.
		/// </summary>
		/// <param name="sources">Localization sources that provide localized strings.</param>
		/// <param name="currentCulture">Initial value of current culture.</param>
		public LocalizationService(
			IEnumerable<ILocalizationSource> sources,
			CultureInfo? currentCulture = null)
		{
			currentCulture ??= CultureInfo.InvariantCulture;

			_sources = sources
				.OrderBy(x => x.Order)
				.ToArray();

			_culture = new ReactiveProperty<CultureInfo>(currentCulture);
			_culture.Subscribe(x => CultureChanged?.Invoke(this, new CultureChangedEventArgs(x)), false);

			ChangeCultureCommand = new Command<string>(x =>
			{
				if (string.IsNullOrEmpty(x))
				{
					return;
				}

				CultureInfo cultureInfo = CultureInfo.GetCultureInfo(x);

				CultureInfo.CurrentCulture = cultureInfo;
				CultureInfo.CurrentUICulture = cultureInfo;
				CultureInfo.DefaultThreadCurrentCulture = currentCulture;
				CultureInfo.DefaultThreadCurrentUICulture = currentCulture;

				_culture.Value = cultureInfo;
			});
		}

		/// <inheritdoc />
		public string GetValue(string key)
		{
			string? result = null;
			CultureInfo culture = Culture.Value;

			foreach (ILocalizationSource source in _sources)
			{
				result = source.GetValue(key, culture);

				if (!string.IsNullOrEmpty(result))
				{
					break;
				}
			}

			if (result == null)
			{
				throw new ArgumentException($"Can't find localized string for key {key}", nameof(key));
			}

			return result;
		}
	}
}