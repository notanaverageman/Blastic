using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Diagnostics;
using Blastic.DynamicControls;
using Blastic.LifetimeManagement;
using Blastic.Ordering;
using Blastic.Reactive;
using Blastic.Services.Settings;
using Blastic.Settings;

namespace Blastic.UserInterface.Settings
{
	public abstract class SettingsSectionViewModel : ConductorAllActive<Setting>, ISettingsSectionViewModel
	{
		private readonly IPresenterSource _presenterSource;

		private Dictionary<string, Setting> _settings;

		public abstract string SectionName { get; }
		public ISettingsService SettingsService { get; }

		public IsExpandedSetting IsExpanded { get; private set; }

		protected SettingsSectionViewModel(
			ISettingsService settingsService,
			IPresenterSource presenterSource)
		{
			_presenterSource = presenterSource;
			SettingsService = settingsService;

			Lifetime.Initialize.Subscribe(x => OnInitialize(), new Order(int.MinValue));
		}

		private Task OnInitialize()
		{
			IsExpanded = new IsExpandedSetting(SettingsService, _presenterSource, SectionName);

			_settings = GetType()
				.GetProperties()
				.Where(x => typeof(Setting).IsAssignableFrom(x.PropertyType))
				.ToDictionary(
					x => x.Name,
					x => (Setting)x.GetValue(this));

			return Task.CompletedTask;
		}

		public virtual Task<IEnumerable<DiagnosticMessage>> GetDiagnosticMessages(CancellationToken cancellationToken)
		{
			List<DiagnosticMessage> diagnosticMessages = new List<DiagnosticMessage>();

			foreach (Setting setting in _settings.Values)
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

		protected void RegisterForUI<T>(Setting<T> setting)
		{
			Items.Add(setting);
		}

		protected void RegisterForUI<T1, T2>(Setting<T1> setting1, Setting<T2> setting2)
		{
			RegisterForUI(setting1);
			RegisterForUI(setting2);
		}

		protected void RegisterForUI<T1, T2, T3>(
			Setting<T1> setting1,
			Setting<T2> setting2,
			Setting<T3> setting3)
		{
			RegisterForUI(setting1, setting2);
			RegisterForUI(setting3);
		}

		protected void RegisterForUI<T1, T2, T3, T4>(
			Setting<T1> setting1,
			Setting<T2> setting2,
			Setting<T3> setting3,
			Setting<T4> setting4)
		{
			RegisterForUI(setting1, setting2, setting3);
			RegisterForUI(setting4);
		}

		protected void RegisterForUI<T1, T2, T3, T4, T5>(
			Setting<T1> setting1,
			Setting<T2> setting2,
			Setting<T3> setting3,
			Setting<T4> setting4,
			Setting<T5> setting5)
		{
			RegisterForUI(setting1, setting2, setting3, setting4);
			RegisterForUI(setting5);
		}
	}
}