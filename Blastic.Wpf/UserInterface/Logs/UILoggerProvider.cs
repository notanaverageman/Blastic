using Microsoft.Extensions.Logging;

namespace Blastic.Wpf.UserInterface.Logs
{
	public class UILoggerProvider : ILoggerProvider
	{
		private readonly UILogger _uiLogger;

		public UILoggerProvider(UILogger uiLogger)
		{
			_uiLogger = uiLogger;
		}

		public void Dispose()
		{
		}

		public ILogger CreateLogger(string categoryName)
		{
			return _uiLogger;
		}
	}
}