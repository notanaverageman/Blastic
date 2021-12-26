using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Blastic.Commanding;
using Blastic.Reactive;
using Blastic.Services.Localization;
using Blastic.ViewManagement;
using Blastic.Wpf.Services.Windowing;
using Microsoft.Extensions.Logging;

namespace Blastic.Wpf.UserInterface.Logs
{
	public sealed class LogsViewModel : IViewAware
	{
		private readonly UILogger _uiLogger;
		private readonly IWindowManager _windowManager;

		public IReadOnlyReactiveProperty<string> Title { get; }

		public IReactiveProperty<object?> View { get; }

		public IReactiveProperty<LogLevel> MinimumLogLevel { get; }

		public ReadOnlyObservableCollection<Log> Logs => _uiLogger.Logs;
		public IEnumerable<LogLevel> LogLevels { get; }

		public AsyncCommand Clear { get; }

		public LogsViewModel(
			UILogger uiLogger,
			IWindowManager windowManager,
			ILocalizationService localizationService)
		{
			_uiLogger = uiLogger;
			_windowManager = windowManager;

			Title = new LocalizableReactiveProperty(localizationService, "Blastic.Logs.Window.Title");

			MinimumLogLevel = new ReactiveProperty<LogLevel>(LogLevel.Error);
			MinimumLogLevel.Subscribe(OnMinimumLogLevelChanged);

			LogLevels = new[]
			{
				LogLevel.Critical,
				LogLevel.Error,
				LogLevel.Warning,
				LogLevel.Information,
				LogLevel.Debug,
				LogLevel.Trace
			};

			View = new ReactiveProperty<object?>();

			Clear = new AsyncCommand(() => _uiLogger.Clear());
		}

		private void OnMinimumLogLevelChanged(LogLevel level)
		{
			_uiLogger.MinimumLogLevel = level;

			ICollectionView collectionView = CollectionViewSource.GetDefaultView(Logs);
			collectionView.Filter = o => ((Log)o).Level >= level;
		}
	}
}