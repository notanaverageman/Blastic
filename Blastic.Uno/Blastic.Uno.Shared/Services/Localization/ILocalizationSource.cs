using System.Globalization;
using Blastic.Common;

namespace Blastic.Services.Localization
{
	public interface ILocalizationSource
	{
		Order Order { get; }

		string GetValue(string key, CultureInfo culture);
	}
}