using System;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Diagnostics;
using Blastic.DynamicControls;
using Blastic.LifetimeManagement;
using Blastic.Reactive;
using Blastic.Services.Settings;

namespace Blastic.Settings
{
	/// <summary>
	/// An individual setting.
	/// </summary>
	public abstract class Setting : IHasLifetime
	{
		private readonly IPresenterSource _presenterSource;

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
		public ReactiveCollection<DiagnosticMessage> DiagnosticMessages { get; }

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

			SettingsStorage = settingsStorage;
			Key = key;

			Lifetime = new Lifetime();
			DiagnosticMessages = new ReactiveCollection<DiagnosticMessage>();
			ShowOnUI = new ReactiveProperty<bool>(true);

			Lifetime.Initialization.Subscribe(Read);

			Lifetime.Closure.Subscribe(async (x, y) =>
			{
				if (x.Result == true)
				{
					await Save(y);
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
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A task to be awaited.</returns>
		public abstract Task Read(CancellationToken cancellationToken);

		/// <summary>
		/// Write the value of this setting to store.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A task to be awaited.</returns>
		public abstract Task Save(CancellationToken cancellationToken);

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
				DiagnosticMessages.Add(new DiagnosticMessage(Severity.Error, error!));
			}

			IReadOnlyReactiveProperty<string>? errorProperty = CheckErrorReactive();

			if (errorProperty != null)
			{
				DiagnosticMessages.Add(new DiagnosticMessage(Severity.Error, errorProperty));
			}
		}
	}

	/// <summary>
	/// An individual setting.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	public abstract class Setting<T> : Setting
	{
		private IDisposable? _isEnabledSubscription;

		/// <summary>
		/// Default value to be used when key does not exist in store.
		/// </summary>
		public T DefaultValue { get; }

		/// <summary>
		/// This property will be bound to the setting UI. Use this property while
		/// checking for errors.
		/// </summary>
		public ReactiveProperty<T> ReactiveSettingValue { get; set; }

		/// <summary>
		/// This property will be bound to the setting UI. Use this property while
		/// checking for errors.
		/// </summary>
		public T SettingValue => ReactiveSettingValue.Value;

		/// <summary>
		/// Use this property to check for the effective value of the setting.
		/// </summary>
		public ReactiveProperty<T> ReactiveValue { get; set; }

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
		public override async Task Read(CancellationToken cancellationToken)
		{
			_isEnabledSubscription?.Dispose();
			_isEnabledSubscription = Element.IsEnabled.Subscribe(_ => ReactiveSettingValue.TriggerValidation());

			object defaultValue = await GetValueBeforeSave(DefaultValue, cancellationToken);
			object storageValue = (await SettingsStorage.Get(Key, defaultValue, cancellationToken))!;

			T value = await GetValueAfterRead(storageValue, cancellationToken);

			ReactiveValue.Value = value;
			ReactiveSettingValue.Value = value;
		}

		/// <inheritdoc />
		public override async Task Save(CancellationToken cancellationToken)
		{
			object value = await GetValueBeforeSave(SettingValue, cancellationToken);

			await SettingsStorage.Put(Key, value, cancellationToken);
			ReactiveValue.Value = SettingValue;
		}

		/// <summary>
		/// Return the setting value corresponding to the value read from store.
		/// </summary>
		/// <remarks>
		/// Override <see cref="GetValueBeforeSave"/> to implement the forward conversion.
		/// By default the sent value is returned.
		/// </remarks>
		/// <param name="value">Value read from store.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>Return the same object or the setting value constructed from sent value.</returns>
		protected virtual Task<T> GetValueAfterRead(object value, CancellationToken cancellationToken)
		{
			return Task.FromResult((T) value);
		}

		/// <summary>
		/// Return an object to save to the storage.
		/// </summary>
		/// <remarks>
		/// If the returned value is not equal to the sent value, override <see cref="GetValueAfterRead"/> to implement
		/// the back conversion. By default the sent value is returned.
		/// </remarks>
		/// <param name="value">Value to write to store.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>Return the same object or the value represents the setting value.</returns>
		protected virtual Task<object> GetValueBeforeSave(T value, CancellationToken cancellationToken)
		{
			return Task.FromResult((object) value!);
		}

		/// <inheritdoc />
		public override void Revert()
		{
			ReactiveSettingValue.Value = Value;
		}

		private void OnSettingValueChanged()
		{
			DiagnosticMessages.Clear();

			if (Element.IsEnabled.Value != true)
			{
				return;
			}

			PopulateDiagnosticMessages();
		}
	}
}