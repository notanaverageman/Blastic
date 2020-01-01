using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Blastic.Commanding;
using Blastic.Reactive;
using Blastic.Services.Localization;
using Blastic.Services.Windowing;
using Blastic.UserInterface.Logs.Settings;
using Blastic.ViewManagement;
using Serilog.Events;

namespace Blastic.UserInterface.Logs
{
	public sealed class LogsViewModel : IViewAware
	{
		private readonly IWindowManager _windowManager;
		private readonly LogSettingsViewModel _logSettingsViewModel;

		public IReactiveProperty<string> Title { get; }

		public IReactiveProperty<UIElement> View { get; }

		public IReactiveProperty<LogEventLevel> MinimumLogLevel { get; }

		public LogSink LogSink { get; }
		public IEnumerable<LogEventLevel> LogLevels { get; }

		public Command Clear { get; }

		public LogsViewModel(
			IWindowManager windowManager,
			ILocalizationService localizationService,
			LogSettingsViewModel logSettingsViewModel,
			LogSink logSink)
		{
			_windowManager = windowManager;
			_logSettingsViewModel = logSettingsViewModel;
			LogSink = logSink;

			Title = new LocalizableReactiveProperty(localizationService, "Blastic.Logs.Window.Title");

			MinimumLogLevel = new ReactiveProperty<LogEventLevel>();
			MinimumLogLevel.Subscribe(OnMinimumLogLevelChanged);
			MinimumLogLevel.Value = LogEventLevel.Debug;

			LogLevels = new []
			{
				LogEventLevel.Fatal,
				LogEventLevel.Error,
				LogEventLevel.Warning,
				LogEventLevel.Information,
				LogEventLevel.Debug
			};

			View = new ReactiveProperty<UIElement>();

			LogSink.Logs.CollectionChangedAsObservable().Subscribe(LogsChanged);

			Clear = new Command(() => LogSink.Logs.Clear());
		}

		private async void LogsChanged(NotifyCollectionChangedEventArgs e)
		{
			bool? hasErrorLog = e.NewItems?.Cast<Log>().Any(x => x.Level >= LogEventLevel.Error);

			if (hasErrorLog == true && _logSettingsViewModel.OpenWindowOnErrorSetting.Value)
			{
				await Show();
			}
		}

		public async Task Show()
		{
			await _windowManager.ShowWindow(this, x =>
			{
				x.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				x.Owner = Application.Current.MainWindow;
			});
		}

		private void OnMinimumLogLevelChanged(LogEventLevel level)
		{
			ICollectionView collectionView = CollectionViewSource.GetDefaultView(LogSink.Logs);
			collectionView.Filter = o => ((Log)o).Level >= level;
		}
	}
}