using System.Globalization;
using System.Resources;
using Blastic.Ordering;

namespace Blastic.Services.Localization
{
	/// <summary>
	/// An implementation of <see cref="ILocalizationSource"/> that looks the keys up
	/// in given <see cref="ResourceManager"/>.
	/// </summary>
	public class ResourceManagerLocalizationSource : ILocalizationSource
	{
		private readonly ResourceManager _resourceManager;

		/// <inheritdoc />
		public Order Order { get; }

		/// <summary>
		/// Creates a new instance with given resource manager and an optional order.
		/// </summary>
		/// <param name="resourceManager">The resource manager to look keys up in.</param>
		/// <param name="order">Order of this source among others.</param>
		public ResourceManagerLocalizationSource(
			ResourceManager resourceManager,
			Order? order = null)
		{
			_resourceManager = resourceManager;
			Order = order ?? new Order();
		}

		/// <inheritdoc />
		public string? GetValue(string key, CultureInfo culture)
		{
			return _resourceManager.GetString(key, culture);
		}
	}
}