using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using Blastic.Commanding;
using Blastic.Reactive;
using Blastic.Services.Localization;
using Blastic.Services.Windowing;
using Blastic.UserInterface.Logs.Settings;
using Blastic.ViewManagement;
using Microsoft.Extensions.Logging;

namespace Blastic.UserInterface.Logs
{
	public sealed class LogsViewModel : IViewAware
	{
		private readonly IWindowManager _windowManager;
		private readonly LogSettingsViewModel _logSettingsViewModel;

		public IReactiveProperty<string> Title { get; }

		public IReactiveProperty<object> View { get; }

		public IReactiveProperty<LogLevel> MinimumLogLevel { get; }

		public ReactiveCollection<Log> Logs { get; }
		public IEnumerable<LogLevel> LogLevels { get; }

		public Command Clear { get; }

		public LogsViewModel(
			IWindowManager windowManager,
			ILocalizationService localizationService,
			LogSettingsViewModel logSettingsViewModel)
		{
			_windowManager = windowManager;
			_logSettingsViewModel = logSettingsViewModel;

			Title = new LocalizableReactiveProperty(localizationService, "Blastic.Logs.Window.Title");

			Logs = UILogger.Instance.Logs;
			Logs.CollectionChangedAsObservable().Subscribe(LogsChanged);

			MinimumLogLevel = new ReactiveProperty<LogLevel>();
			MinimumLogLevel.Subscribe(OnMinimumLogLevelChanged);
			MinimumLogLevel.Value = LogLevel.Debug;

			LogLevels = new []
			{
				LogLevel.Critical,
				LogLevel.Error,
				LogLevel.Warning,
				LogLevel.Information,
				LogLevel.Debug,
				LogLevel.Trace
			};

			View = new ReactiveProperty<object>();

			Clear = new Command(() => Logs.Clear());
		}

		private async void LogsChanged(NotifyCollectionChangedEventArgs e)
		{
			bool? hasErrorLog = e.NewItems?.Cast<Log>().Any(x => x.Level >= LogLevel.Error);

			if (hasErrorLog == true && _logSettingsViewModel.OpenWindowOnErrorSetting.Value)
			{
				await Show();
			}
		}

		public async Task Show()
		{
			await _windowManager.ShowWindow(this);
		}

		private void OnMinimumLogLevelChanged(LogLevel level)
		{
			UILogger.Instance.MinimumLogLevel = level;

			ICollectionView collectionView = CollectionViewSource.GetDefaultView(Logs);
			collectionView.Filter = o => ((Log)o).Level >= level;
		}
	}
}