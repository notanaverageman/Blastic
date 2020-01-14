using System;
using Blastic.Reactive;
using Microsoft.Extensions.Logging;

namespace Blastic.Wpf.UserInterface.Logs
{
	internal class UILogger : ILogger
	{
		public static readonly UILogger Instance = new UILogger();

		public ReactiveCollection<Log> Logs { get; }
		public LogLevel MinimumLogLevel { get; set; }

		private UILogger()
		{
			Logs = new ReactiveCollection<Log>();
		}

		public void Log<TState>(
			LogLevel logLevel,
			EventId eventId,
			TState state,
			Exception exception,
			Func<TState, Exception, string> formatter)
		{
			if (!IsEnabled(logLevel))
			{
				return;
			}

			Log log = new Log
			{
				Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
				Level = logLevel,
				Message = formatter(state, exception)
			};

			// ReactiveCollection should be thread safe as it modifies the internal collection
			// only on UI thread.
			Logs.Add(log);
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			return logLevel >= MinimumLogLevel;
		}

		public IDisposable BeginScope<TState>(TState state)
		{
			return null;
		}
	}

	internal class UILoggerProvider : ILoggerProvider
	{
		public void Dispose()
		{
		}

		public ILogger CreateLogger(string categoryName)
		{
			return UILogger.Instance;
		}
	}
}