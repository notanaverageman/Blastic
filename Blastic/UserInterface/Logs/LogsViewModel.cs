using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Blastic.UserInterface.Logs.Settings;
using Caliburn.Micro;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Serilog.Events;

namespace Blastic.UserInterface.Logs
{
	public sealed class LogsViewModel : IViewAware
	{
		private readonly IWindowManager _windowManager;
		private readonly LogSettingsViewModel _logSettingsViewModel;

		private Window _activeWindow;

		public IReactiveProperty<LogEventLevel> MinimumLogLevel { get; set; }

		public LogSink LogSink { get; }
		public IEnumerable<LogEventLevel> LogLevels { get; }

		public ReactiveCommand Clear { get; }

		public LogsViewModel(
			IWindowManager windowManager,
			LogSettingsViewModel logSettingsViewModel,
			LogSink logSink)
		{
			_windowManager = windowManager;
			_logSettingsViewModel = logSettingsViewModel;
			LogSink = logSink;

			MinimumLogLevel = new ReactiveProperty<LogEventLevel>();
			MinimumLogLevel.Subscribe(OnMinimumLogLevelChanged);

			LogLevels = new []
			{
				LogEventLevel.Fatal,
				LogEventLevel.Error,
				LogEventLevel.Warning,
				LogEventLevel.Information,
				LogEventLevel.Debug
			};

			MinimumLogLevel.Value = LogEventLevel.Debug;

			LogSink.Logs.CollectionChangedAsObservable().Subscribe(LogsChanged);

			Clear = new ReactiveCommand().WithSubscribe(() => LogSink.Logs.Clear());
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
			if (_activeWindow != null && PresentationSource.FromVisual(_activeWindow) != null)
			{
				_activeWindow.Activate();
				return;
			}

			dynamic settings = new ExpandoObject();
			settings.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			settings.Owner = Application.Current.MainWindow;

			await _windowManager.ShowWindowAsync(this, null, settings);
		}

		private void OnMinimumLogLevelChanged(LogEventLevel level)
		{
			ICollectionView collectionView = CollectionViewSource.GetDefaultView(LogSink.Logs);
			collectionView.Filter = o => ((Log)o).Level >= level;
		}

		public void AttachView(object view, object context = null)
		{
			_activeWindow = view as Window;

			ViewAttached?.Invoke(this, new ViewAttachedEventArgs
			{
				View = view
			});
		}

		public object GetView(object context = null)
		{
			return _activeWindow;
		}

		public event EventHandler<ViewAttachedEventArgs> ViewAttached;
	}
}