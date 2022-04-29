using System;
using System.Collections.ObjectModel;
using Blastic.Diagnostics;
using Blastic.DynamicControls;
using Blastic.LifetimeManagement;
using Blastic.Reactive;
using Blastic.Services.Settings;
using DynamicData;

namespace Blastic.Settings
{
	/// <summary>
	/// An individual setting.
	/// </summary>
	public abstract class Setting : IHasLifetime
	{
		private readonly IPresenterSource _presenterSource;

		/// <summary>
		/// Items source for diagnostic messages that is populated when a validation occurs.
		/// </summary>
		protected SourceList<DiagnosticMessage> DiagnosticMessagesSource { get; }

		/// <summary>
		/// Settings storage that is used when reading or writing values.
		/// </summary>
		protected ISettingsStorage SettingsStorage { get; }

		/// <summary>
		/// An observable property that decides whether to show the setting on user interface.
		/// It is true by default.
		/// </summary>
		public IReactiveProperty<bool> ShowOnUI { get; }

		/// <summary>
		/// If set to true, setting will be saved to storage whenever its value changes.
		/// </summary>
		public bool SaveOnChange { get; set; }

		/// <summary>
		/// Lifetime object that initates the read and save operations.
		/// </summary>
		public ILifetime Lifetime { get; }

		/// <summary>
		/// Element instance that will be used when setting is shown on UI.
		/// </summary>
		public abstract IElement Element { get; }

		/// <summary>
		/// Presenter that will be shown on UI.
		/// </summary>
		public IPresenter Presenter => _presenterSource.CreatePresenter(Element);

		/// <summary>
		/// Key that is used when reading from or writing to the store.
		/// </summary>
		public string Key { get; }

		/// <summary>
		/// Collection of diagnostic messages that is populated when a validation occurs.
		/// </summary>
		public ReadOnlyObservableCollection<DiagnosticMessage> DiagnosticMessages { get; }

		/// <summary>
		/// Creates a new instance of <see cref="Setting"/>
		/// </summary>
		/// <param name="settingsStorage">The settings storage.</param>
		/// <param name="presenterSource">The presenter source.</param>
		/// <param name="key">Key that is used when reading from or writing to the store.</param>
		public Setting(
			ISettingsStorage settingsStorage,
			IPresenterSource presenterSource,
			string key)
		{
			_presenterSource = presenterSource;
			DiagnosticMessagesSource = new SourceList<DiagnosticMessage>();

			SettingsStorage = settingsStorage;
			Key = key;

			Lifetime = new Lifetime();
			ShowOnUI = new ReactiveProperty<bool>(true);

			DiagnosticMessagesSource
				.Connect()
				.Bind(out ReadOnlyObservableCollection<DiagnosticMessage> diagnosticMessages)
				.Subscribe();

			DiagnosticMessages = diagnosticMessages;

			Lifetime.Initialization.Subscribe(Read);

			Lifetime.Closure.Subscribe((x, _) =>
			{
				if (x?.Result == true)
				{
					Save();
				}
				else
				{
					Revert();
				}
			});
		}

		/// <summary>
		/// Read the value of this setting from store.
		/// </summary>
		/// <returns>A task to be awaited.</returns>
		public abstract void Read();

		/// <summary>
		/// Write the value of this setting to store.
		/// </summary>
		/// <returns>A task to be awaited.</returns>
		public abstract void Save();

		/// <summary>
		/// Sets the <see cref="Setting{T}.SettingValue"/> to the <see cref="Setting{T}.Value"/>.
		/// </summary>
		public abstract void Revert();

		/// <summary>
		/// Return a non empty error message if there is a validation error.
		/// </summary>
		/// <returns>A non empty error message if there is a validation error.</returns>
		public virtual string? CheckError()
		{
			return null;
		}

		/// <summary>
		/// Return a non null observable property for error message if there is a validation error.
		/// </summary>
		/// <returns>A non null observable property for error message if there is a validation error.</returns>
		public virtual IReadOnlyReactiveProperty<string>? CheckErrorReactive()
		{
			return null;
		}

		public virtual void PopulateDiagnosticMessages()
		{
			string? error = CheckError();

			if (!string.IsNullOrEmpty(error))
			{
				DiagnosticMessagesSource.Add(new DiagnosticMessage(Severity.Error, error!));
			}

			IReadOnlyReactiveProperty<string>? errorProperty = CheckErrorReactive();

			if (errorProperty != null)
			{
				DiagnosticMessagesSource.Add(new DiagnosticMessage(Severity.Error, errorProperty));
			}
		}
	}

	/// <summary>
	/// An individual setting.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <typeparam name="TStored">Type of the object to store.</typeparam>
	public abstract class Setting<T, TStored> : Setting
	{
		private IDisposable? _isEnabledSubscription;
		private bool _isReadingValue;

		/// <summary>
		/// Default value to be used when key does not exist in store.
		/// </summary>
		public T DefaultValue { get; }

		/// <summary>
		/// This property will be bound to the setting UI. Use this property while
		/// checking for errors.
		/// </summary>
		public ReactiveProperty<T> ReactiveSettingValue { get; }

		/// <summary>
		/// This property will be bound to the setting UI. Use this property while
		/// checking for errors.
		/// </summary>
		public T SettingValue => ReactiveSettingValue.Value;

		/// <summary>
		/// Use this property to check for the effective value of the setting.
		/// </summary>
		public ReactiveProperty<T> ReactiveValue { get; }

		/// <summary>
		/// Use this property to check for the effective value of the setting.
		/// </summary>
		public T Value => ReactiveValue.Value;

		/// <summary>
		/// Creates a new instance of <see cref="Setting"/>
		/// </summary>
		/// <param name="settingsStorage">The settings storage.</param>
		/// <param name="presenterSource">The presenter source.</param>
		/// <param name="key">Key that is used when reading from or writing to the store.</param>
		/// <param name="defaultValue">Default value to be used when key does not exist in store.</param>
		public Setting(
			ISettingsStorage settingsStorage,
			IPresenterSource presenterSource,
			string key,
			T defaultValue)
			:
			base(settingsStorage, presenterSource, key)
		{
			DefaultValue = defaultValue;

			ReactiveValue = new ReactiveProperty<T>(DefaultValue);
			ReactiveSettingValue = new ReactiveProperty<T>(DefaultValue);

			ReactiveSettingValue.AddValidator(_ => Element.IsEnabled.Value ? CheckError() : null);
			ReactiveSettingValue.AddValidator(_ => Element.IsEnabled.Value ? CheckErrorReactive() : null);

			ReactiveSettingValue.Subscribe(_ => OnSettingValueChanged(), false);
		}

		/// <inheritdoc />
		public override void Read()
		{
			_isReadingValue = true;
			
			_isEnabledSubscription?.Dispose();
			_isEnabledSubscription = Element.IsEnabled.Subscribe(_ => ReactiveSettingValue.TriggerValidation());

			TStored defaultValue = GetValueBeforeSave(DefaultValue);
			TStored storageValue = SettingsStorage.Get(Key, defaultValue);

			T value = GetValueAfterRead(storageValue);

			ReactiveValue.Value = value;
			ReactiveSettingValue.Value = value;

			_isReadingValue = false;
		}

		/// <inheritdoc />
		public override void Save()
		{
			TStored value = GetValueBeforeSave(SettingValue);

			SettingsStorage.Put(Key, value);
			ReactiveValue.Value = SettingValue;
		}

		/// <summary>
		/// Return the setting value corresponding to the value read from store.
		/// </summary>
		/// <param name="value">Value read from store.</param>
		/// <returns>Return the same object or the setting value constructed from sent value.</returns>
		protected abstract T GetValueAfterRead(TStored value);

		/// <summary>
		/// Return an object to save to the storage.
		/// </summary>
		/// <param name="value">Value to write to store.</param>
		/// <returns>Return the same object or the value represents the setting value.</returns>
		protected abstract TStored GetValueBeforeSave(T value);

		/// <inheritdoc />
		public override void Revert()
		{
			ReactiveSettingValue.Value = Value;
		}

		private void OnSettingValueChanged()
		{
			DiagnosticMessagesSource.Clear();

			if (Element.IsEnabled.Value != true)
			{
				return;
			}

			PopulateDiagnosticMessages();

			if (SaveOnChange && !_isReadingValue && DiagnosticMessages.Count == 0)
			{
				Save();
			}
		}
	}

	/// <summary>
	/// An individual setting.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	public abstract class Setting<T> : Setting<T, T>
	{
		/// <summary>
		/// Creates a new instance of <see cref="Setting"/>
		/// </summary>
		/// <param name="settingsStorage">The settings storage.</param>
		/// <param name="presenterSource">The presenter source.</param>
		/// <param name="key">Key that is used when reading from or writing to the store.</param>
		/// <param name="defaultValue">Default value to be used when key does not exist in store.</param>
		public Setting(
			ISettingsStorage settingsStorage,
			IPresenterSource presenterSource,
			string key,
			T defaultValue)
			:
			base(settingsStorage, presenterSource, key, defaultValue)
		{
		}

		/// <inheritdoc cref="Setting{T,TStored}.GetValueAfterRead"/>
		protected override T GetValueAfterRead(T value)
		{
			return value;
		}

		/// <inheritdoc cref="Setting{T,TStored}.GetValueAfterRead"/>
		protected override T GetValueBeforeSave(T value)
		{
			return value;
		}
	}
}