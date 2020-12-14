using System;
using System.Globalization;
using Blastic.Reactive;

namespace Blastic.Services.Localization
{
	/// <summary>
	/// Provides localized strings for given keys according to the current culture.
	/// </summary>
	public interface ILocalizationService
	{
		/// <summary>
		/// Event that is raised when culture changes.
		/// </summary>
		event EventHandler<CultureChangedEventArgs>? CultureChanged;

		/// <summary>
		/// An observable property that holds the current culture.
		/// </summary>
		IReactiveProperty<CultureInfo> Culture { get; }

		/// <summary>
		/// Get localized string for given key.
		/// </summary>
		/// <param name="key">Key for localized string.</param>
		/// <returns>The localized string or null if key is not found.</returns>
		string? GetValue(string key);
	}
}