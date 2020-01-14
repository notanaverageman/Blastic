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

		protected ISettingsService SettingsService { get; }

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
		/// Key to be used when writing to the database.
		/// </summary>
		public string Key { get; }

		public ReactiveCollection<DiagnosticMessage> DiagnosticMessages { get; }

		public Setting(
			ISettingsService settingsService,
			IPresenterSource presenterSource,
			string key)
		{
			_presenterSource = presenterSource;

			SettingsService = settingsService;
			Key = key;

			Lifetime = new Lifetime();
			DiagnosticMessages = new ReactiveCollection<DiagnosticMessage>();
			ShowOnUI = new ReactiveProperty<bool>(true);

			Lifetime.Initialize.Subscribe(x => Read(x.Parameter.CancellationToken));

			Lifetime.Close.Subscribe(async x =>
			{
				if (x.Parameter.DialogResult == true)
				{
					await Save(x.Parameter.CancellationToken);
				}
				else
				{
					Revert();
				}
			});
		}

		public abstract Task Read(CancellationToken cancellationToken);
		public abstract Task Save(CancellationToken cancellationToken);
		public abstract void Revert();

		public virtual string CheckError()
		{
			return null;
		}

		public virtual void PopulateDiagnosticMessages()
		{
			string error = CheckError();

			if (!string.IsNullOrEmpty(error))
			{
				DiagnosticMessages.Add(new DiagnosticMessage(Severity.Error, error));
			}
		}
	}

	/// <summary>
	/// An individual setting.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	public abstract class Setting<T> : Setting
	{
		private IDisposable _isEnabledSubscription;

		/// <summary>
		/// Default value to be returned when key does not exist in database.
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

		public Setting(
			ISettingsService settingsService,
			IPresenterSource presenterSource,
			string key,
			T defaultValue)
			:
			base(settingsService, presenterSource, key)
		{
			DefaultValue = defaultValue;

			ReactiveValue = new ReactiveProperty<T>(DefaultValue);
			ReactiveSettingValue = new ReactiveProperty<T>(DefaultValue);

			ReactiveSettingValue.AddValidator(_ => Element?.IsEnabled.Value == true ? CheckError() : null);
			ReactiveSettingValue.Subscribe(_ => OnSettingValueChanged());
		}

		public override async Task Read(CancellationToken cancellationToken)
		{
			_isEnabledSubscription?.Dispose();
			_isEnabledSubscription = Element.IsEnabled.Subscribe(x => ReactiveSettingValue.TriggerValidation());

			T value = await SettingsService.Get(Key, DefaultValue, cancellationToken);

			value = await AfterRead(value, cancellationToken);

			ReactiveValue.Value = value;
			ReactiveSettingValue.Value = value;
		}

		public override async Task Save(CancellationToken cancellationToken)
		{
			T value = await BeforeSave(SettingValue, cancellationToken);

			await SettingsService.Put(Key, value, cancellationToken);
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

		public override void Revert()
		{
			ReactiveSettingValue.Value = Value;
		}

		private void OnSettingValueChanged()
		{
			DiagnosticMessages.Clear();

			if (Element?.IsEnabled.Value != true)
			{
				return;
			}

			PopulateDiagnosticMessages();
		}
	}
}