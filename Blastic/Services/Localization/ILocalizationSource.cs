using System.Globalization;
using Blastic.Ordering;

namespace Blastic.Services.Localization
{
	/// <summary>
	/// Provides localized strings for given key and culture. Used by <see cref="LocalizationService"/>
	/// to get localized strings.
	/// </summary>
	public interface ILocalizationSource
	{
		/// <summary>
		/// The order of this localization source among others.
		/// </summary>
		Order Order { get; }

		/// <summary>
		/// Return a localized string in given culture for given key.
		/// </summary>
		/// <param name="key">Key for the localized string.</param>
		/// <param name="culture">The culture.</param>
		/// <returns></returns>
		string? GetValue(string key, CultureInfo culture);
	}
}