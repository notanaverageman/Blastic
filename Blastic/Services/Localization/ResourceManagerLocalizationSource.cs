using System.Globalization;
using System.Resources;
using Blastic.Ordering;

namespace Blastic.Services.Localization
{
	public class ResourceManagerLocalizationSource : ILocalizationSource
	{
		private readonly ResourceManager _resourceManager;
		
		public Order Order { get; }

		public ResourceManagerLocalizationSource(
			ResourceManager resourceManager,
			Order? order = null)
		{
			_resourceManager = resourceManager;
			Order = order ?? new Order();
		}

		public string? GetValue(string key, CultureInfo culture)
		{
			return _resourceManager.GetString(key, culture);
		}
	}
}