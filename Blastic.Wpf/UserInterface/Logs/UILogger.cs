using System;
using System.Collections.ObjectModel;
using Blastic.Platform;
using DynamicData;
using Microsoft.Extensions.Logging;

namespace Blastic.Wpf.UserInterface.Logs
{
	public class UILogger : ILogger
	{
		private readonly SourceList<Log> _logs;

		public ReadOnlyObservableCollection<Log> Logs { get; }
		public LogLevel MinimumLogLevel { get; set; }

		public UILogger(IPlatformSpecifics platformSpecifics)
		{
			_logs = new SourceList<Log>();

			_logs
				.Connect()
				.ObserveOnUI(platformSpecifics)
				.Bind(out ReadOnlyObservableCollection<Log> logs)
				.Subscribe();
			
			Logs = logs;
		}

		public void Log<TState>(
			LogLevel logLevel,
			EventId eventId,
			TState state,
			Exception? exception,
			Func<TState, Exception?, string> formatter)
		{
			if (!IsEnabled(logLevel))
			{
				return;
			}

			Log log = new()
			{
				Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
				Level = logLevel,
				Message = formatter(state, exception)
			};
			
			_logs.Add(log);
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			return logLevel >= MinimumLogLevel;
		}

		public IDisposable? BeginScope<TState>(TState state) where TState : notnull
		{
			return null;
		}

		public void Clear()
		{
			_logs.Clear();
		}
	}
}