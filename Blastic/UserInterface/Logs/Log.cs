using Microsoft.Extensions.Logging;

namespace Blastic.UserInterface.Logs
{
	public class Log
	{
		public string Date { get; set; }
		public LogLevel Level { get; set; }
		public string Message { get; set; }
	}
}