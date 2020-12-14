using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Diagnostics;
using Blastic.DynamicControls;
using Blastic.LifetimeManagement;
using Blastic.LifetimeManagement.Contexts;
using Blastic.Ordering;
using Blastic.Reactive;
using Blastic.Services.Settings;
using Blastic.Settings;

namespace Blastic.UserInterface.Settings
{
	public abstract class SettingsSectionViewModel : ConductorAllActive<Setting>, ISettingsSectionViewModel
	{
		private readonly IPresenterSource _presenterSource;

		public ReactiveCollection<Setting> SettingsToShow { get; }

		public abstract string SectionName { get; }
		public ISettingsStorage SettingsStorage { get; }

		public IsExpandedSetting IsExpanded { get; private set; }

		protected SettingsSectionViewModel(
			ISettingsStorage settingsStorage,
			IPresenterSource presenterSource)
		{
			_presenterSource = presenterSource;
			SettingsStorage = settingsStorage;

			SettingsToShow = new ReactiveCollection<Setting>();

			Lifetime.Initialization.Subscribe(OnInitialize, new Order(int.MinValue));
		}

		private async Task OnInitialize(
			InitializationContext context,
			CancellationToken cancellationToken)
		{
			IsExpanded = new IsExpandedSetting(SettingsStorage, _presenterSource, SectionName);

			List<Setting> settings = GetType()
				.GetProperties()
				.Where(x => typeof(Setting).IsAssignableFrom(x.PropertyType))
				.Select(x => (Setting)x.GetValue(this))
				.ToList();

			Items.Clear();
			SettingsToShow.Clear();

			Items.AddRange(settings);

			foreach (Setting setting in settings)
			{
				setting.ShowOnUI.Subscribe(x =>
				{
					if (x)
					{
						SettingsToShow.Add(setting);
					}
					else
					{
						SettingsToShow.Remove(setting);
					}
				});

				if (cancellationToken.IsCancellationRequested)
				{
					break;
				}

				await setting.Lifetime.Initialize(cancellationToken, context);
			}
		}

		public virtual Task<IEnumerable<DiagnosticMessage>> GetDiagnosticMessages(CancellationToken cancellationToken)
		{
			List<DiagnosticMessage> diagnosticMessages = new List<DiagnosticMessage>();

			foreach (Setting setting in Items)
			{
				setting.PopulateDiagnosticMessages();
				ReactiveCollection<DiagnosticMessage> messages = setting.DiagnosticMessages;

				if (messages?.Any() != true)
				{
					continue;
				}

				diagnosticMessages.AddRange(messages);
			}

			return Task.FromResult((IEnumerable<DiagnosticMessage>)diagnosticMessages);
		}
	}
}