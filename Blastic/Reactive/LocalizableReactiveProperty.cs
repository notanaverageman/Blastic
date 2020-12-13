using System.Diagnostics;
using Blastic.Services.Localization;

namespace Blastic.Reactive
{
	/// <summary>
	/// An implementation of <see cref="IReadOnlyReactiveProperty{T}"/> that emits a new value
	/// when <see cref="ILocalizationService.Culture"/> changes.
	/// </summary>
	[DebuggerDisplay("{" + nameof(Value) + "}")]
	public class LocalizableReactiveProperty : ReactivePropertyBase<string?>, IReadOnlyReactiveProperty<string?>
	{
		private readonly ILocalizationService _localizationService;
		private readonly string _key;

		/// <inheritdoc />
		public string? Value => GetValue();

		/// <inheritdoc />
		object? IReadOnlyReactiveProperty.Value => Value;

		/// <summary>
		/// Creates a new instance that listens to the changes in given localization service's
		/// culture and emits a localized value that corresponds to the given key.
		/// </summary>
		/// <param name="localizationService">The localization servie to listen to for changes in culture.</param>
		/// <param name="key">Key that is used for getting localized values from the localization service.</param>
		public LocalizableReactiveProperty(
			ILocalizationService localizationService,
			string key)
			:
			base(localizationService.GetValue(key), null)
		{
			_localizationService = localizationService;
			_key = key;

			localizationService.CultureChanged += OnNext;
		}

		private void OnNext(object sender, CultureChangedEventArgs args)
		{
			string? value = _localizationService.GetValue(_key);
			SetValue(value);
		}

		/// <inheritdoc />
		public override void Dispose()
		{
			_localizationService.CultureChanged -= OnNext;
			base.Dispose();
		}
	}
}