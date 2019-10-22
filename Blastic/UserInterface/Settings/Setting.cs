using System.Threading;
using System.Threading.Tasks;
using Blastic.Controls.DynamicControls.Elements;
using Blastic.Diagnostics;
using Blastic.Services.Settings;
using Caliburn.Micro;
using Reactive.Bindings;

namespace Blastic.UserInterface.Settings
{
	/// <summary>
	/// An individual setting.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	public abstract class Setting<T>
	{
		private readonly ISettingsService _settingsService;

		/// <summary>
		/// Element instance that will be used when setting is shown on UI.
		/// </summary>
		public abstract IElement Element { get; }

		/// <summary>
		/// Presenter that will be shown on UI.
		/// </summary>
		public Presenter Presenter => Element.ToPresenter();

		/// <summary>
		/// Key to be used when writing to the database.
		/// </summary>
		public string Key { get; }

		/// <summary>
		/// Default value to be returned when key does not exist in database.
		/// </summary>
		public T DefaultValue { get; }

		/// <summary>
		/// This property will be bound to the setting UI. Use this property while
		/// checking for errors.
		/// </summary>
		public IReactiveProperty<T> ReactiveSettingValue { get; set; }

		/// <summary>
		/// This property will be bound to the setting UI. Use this property while
		/// checking for errors.
		/// </summary>
		public T SettingValue => ReactiveSettingValue.Value;

		/// <summary>
		/// Use this property to check for the effective value of the setting.
		/// </summary>
		public IReactiveProperty<T> ReactiveValue { get; set; }

		/// <summary>
		/// Use this property to check for the effective value of the setting.
		/// </summary>
		public T Value => ReactiveValue.Value;

		public IObservableCollection<DiagnosticMessage> DiagnosticMessages { get; }

		public Setting(
			ISettingsService settingsService,
			string key,
			T defaultValue)
		{
			_settingsService = settingsService;

			Key = key;
			DefaultValue = defaultValue;

			DiagnosticMessages = new BindableCollection<DiagnosticMessage>();

			ReactiveValue = new ReactiveProperty<T>(DefaultValue);
			ReactiveSettingValue = new ReactiveProperty<T>(DefaultValue);
		}

		public async Task Read(CancellationToken cancellationToken)
		{
			T value = await _settingsService.Get(Key, DefaultValue, cancellationToken);

			await AfterRead(value, cancellationToken);

			ReactiveValue.Value = value;
			ReactiveSettingValue.Value = value;
		}

		public async Task Save(CancellationToken cancellationToken)
		{
			T value = await BeforeSave(SettingValue, cancellationToken);

			await _settingsService.Put(Key, value, cancellationToken);
			ReactiveValue.Value = SettingValue;
		}

		protected virtual Task<T> AfterRead(T value, CancellationToken cancellationToken)
		{
			return Task.FromResult(value);
		}

		protected virtual Task<T> BeforeSave(T value, CancellationToken cancellationToken)
		{
			return Task.FromResult(value);
		}

		public void Revert()
		{
			ReactiveSettingValue.Value = Value;
		}

		public virtual string CheckError()
		{
			return null;
		}

		public virtual void PopulateDiagnosticMessages()
		{
		}

		// Will be called by Fody.
		private void OnSettingValueChanged()
		{
			DiagnosticMessages.Clear();
			PopulateDiagnosticMessages();
		}
	}
}