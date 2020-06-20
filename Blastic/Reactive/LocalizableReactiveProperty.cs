using Blastic.Services.Localization;

namespace Blastic.Reactive
{
	public class LocalizableReactiveProperty : ReactivePropertyBase<string>, IReadOnlyReactiveProperty<string>
	{
		private readonly ILocalizationService _localizationService;
		private readonly string _key;

		public string Value => GetValue();

		object IReadOnlyReactiveProperty.Value => Value;

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
			string value = _localizationService.GetValue(_key);
			SetValue(value);
		}

		public override void Dispose()
		{
			_localizationService.CultureChanged -= OnNext;
			base.Dispose();
		}
	}
}