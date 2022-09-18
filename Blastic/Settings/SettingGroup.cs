using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Diagnostics;
using Blastic.LifetimeManagement;
using Blastic.Services.Settings;
using DynamicData;

namespace Blastic.Settings
{
	public class SettingGroup : ConductorAllActive<Setting>
	{
		public ISettingsStorage SettingsStorage { get; }
		public ReadOnlyObservableCollection<Setting> SettingsToShow { get; }
		
		protected SettingGroup(ISettingsStorage settingsStorage)
		{
			SettingsStorage = settingsStorage;

			ItemsSource
				.Connect()
				.FilterOnObservable(x => x.ShowOnUI)
				.Bind(out ReadOnlyObservableCollection<Setting> settingsToShow)
				.DisposeMany()
				.Subscribe();

			SettingsToShow = settingsToShow;
		}

		public virtual Task<IEnumerable<DiagnosticMessage>> GetDiagnosticMessages(CancellationToken cancellationToken)
		{
			List<DiagnosticMessage> diagnosticMessages = new();

			foreach (Setting setting in Items)
			{
				setting.PopulateDiagnosticMessages();
				ReadOnlyObservableCollection<DiagnosticMessage> messages = setting.DiagnosticMessages;

				if (messages.Count == 0)
				{
					continue;
				}

				diagnosticMessages.AddRange(messages);
			}

			return Task.FromResult((IEnumerable<DiagnosticMessage>)diagnosticMessages);
		}
	}
}