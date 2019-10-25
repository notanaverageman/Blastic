using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Caliburn;
using Blastic.Diagnostics;
using Blastic.Execution;
using Blastic.Services.Settings;
using Blastic.Settings;
using Caliburn.Micro;

namespace Blastic.UserInterface.Settings
{
	public abstract class SettingsSectionViewModel : ConductorAllActiveBase<object>, ISettingsSectionViewModel
	{
		private Dictionary<string, SettingInfo> _settings;

		public abstract string SectionName { get; }
		public ISettingsService SettingsService { get; }

		public IsExpandedSetting IsExpanded { get; private set; }

		protected SettingsSectionViewModel(
			ExecutionContextFactory executionContextFactory,
			ISettingsService settingsService)
			:
			base(executionContextFactory)
		{
			SettingsService = settingsService;
		}

		protected override Task OnInitializeAsync(CancellationToken cancellationToken)
		{
			IsExpanded = new IsExpandedSetting(SettingsService, SectionName);

			_settings = GetType()
				.GetProperties()
				.Where(x => IsAssignableToGenericType(x.PropertyType, typeof(Setting<>)))
				.ToDictionary(
					x => x.Name,
					x => new SettingInfo(x, x.GetValue(this)));

			return Task.CompletedTask;
		}

		public async Task Save(CancellationToken cancellationToken)
		{
			foreach (SettingInfo info in _settings.Values)
			{
				await (Task)info.SaveMethod.Invoke(info.Setting, new object[] { cancellationToken });
			}
		}

		public async Task ReadSettings(CancellationToken cancellationToken)
		{
			foreach (SettingInfo info in _settings.Values)
			{
				await (Task)info.ReadMethod.Invoke(info.Setting, new object[] { cancellationToken });
			}
		}

		public void Revert()
		{
			foreach (SettingInfo info in _settings.Values)
			{
				info.RevertMethod.Invoke(info.Setting, Array.Empty<object>());
			}
		}

		public virtual Task<IEnumerable<DiagnosticMessage>> GetDiagnosticMessages(CancellationToken cancellationToken)
		{
			List<DiagnosticMessage> diagnosticMessages = new List<DiagnosticMessage>();

			foreach (SettingInfo info in _settings.Values)
			{
				IObservableCollection<DiagnosticMessage> messages = (IObservableCollection<DiagnosticMessage>)info.DiagnosticMessagesProperty.GetValue(info.Setting);

				if (messages?.Any() != true)
				{
					continue;
				}

				diagnosticMessages.AddRange(messages);
			}

			return Task.FromResult((IEnumerable<DiagnosticMessage>)diagnosticMessages);
		}

		private static bool IsAssignableToGenericType(Type givenType, Type genericType)
		{
			Type[] interfaceTypes = givenType.GetInterfaces();

			if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
			{
				return true;
			}

			foreach (Type it in interfaceTypes)
			{
				if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
				{
					return true;
				}
			}

			Type baseType = givenType.BaseType;

			if (baseType == null)
			{
				return false;
			}

			return IsAssignableToGenericType(baseType, genericType);
		}

		private class SettingInfo
		{
			public object Setting { get; }

			public MethodInfo SaveMethod { get; }
			public MethodInfo ReadMethod { get; }
			public MethodInfo RevertMethod { get; }
			public PropertyInfo DiagnosticMessagesProperty { get; }

			public SettingInfo(PropertyInfo propertyInfo, object setting)
			{
				Setting = setting;

				SaveMethod = GetMethodInfo(nameof(Setting<object>.Save), propertyInfo.PropertyType);
				ReadMethod = GetMethodInfo(nameof(Setting<object>.Read), propertyInfo.PropertyType);
				RevertMethod = GetMethodInfo(nameof(Setting<object>.Revert), propertyInfo.PropertyType);
				DiagnosticMessagesProperty = GetPropertyInfo(nameof(Setting<object>.DiagnosticMessages), propertyInfo.PropertyType);
			}

			private MethodInfo GetMethodInfo(
				string methodName,
				Type propertyType)
			{
				MethodInfo methodInfo = propertyType.GetMethod(methodName);

				if (methodInfo == null)
				{
					throw new InvalidOperationException($"{methodName} method is not found on {propertyType}!");
				}

				return methodInfo;
			}

			private PropertyInfo GetPropertyInfo(
				string propertyName,
				Type propertyType)
			{
				PropertyInfo propertyInfo = propertyType.GetProperty(propertyName);

				if (propertyInfo == null)
				{
					throw new InvalidOperationException($"{propertyName} property is not found on {propertyType}!");
				}

				return propertyInfo;
			}
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