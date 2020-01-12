using System.Globalization;
using Windows.ApplicationModel.Resources;
using Blastic.Common;

namespace Blastic.Services.Localization
{
	public class ResourceLoaderLocalizationSource : ILocalizationSource
	{
		private readonly ResourceLoader _resourceLoader;
		
		public Order Order { get; }

		public ResourceLoaderLocalizationSource(
			ResourceLoader resourceLoader,
			Order order = null)
		{
			_resourceLoader = resourceLoader;
			Order = order ?? new Order();
		}

		public string GetValue(string key, CultureInfo culture)
		{
			key = key.Replace(".", "/");
			return _resourceLoader.GetString(key);
		}
	}
}