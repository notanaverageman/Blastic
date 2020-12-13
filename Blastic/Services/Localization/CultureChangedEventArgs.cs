using System;
using System.Globalization;

namespace Blastic.Services.Localization
{
	/// <summary>
	/// Args for <see cref="ILocalizationService.CultureChanged"/> event.
	/// </summary>
	public class CultureChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Current culture.
		/// </summary>
		public CultureInfo Culture { get; }

		/// <summary>
		/// Create a new instance of <see cref="CultureChangedEventArgs"/> with given culture.
		/// </summary>
		/// <param name="culture">Current culture.</param>
		public CultureChangedEventArgs(CultureInfo culture)
		{
			Culture = culture;
		}
	}
}