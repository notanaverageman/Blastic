using System;

namespace Blastic.Forms.Sample.UserInterface.Downloads
{
	public class DownloadStatusListener
	{
		public Action? Queued { get; }
		public Action? Succeeded { get; }
		public Action? Cancelled { get; }
		public Action<Exception>? ThrewException { get; }

		public DownloadStatusListener(
			Action? queued = null,
			Action? succeeded = null,
			Action? cancelled = null,
			Action<Exception>? threwException = null)
		{
			Queued = queued;
			Succeeded = succeeded;
			Cancelled = cancelled;
			ThrewException = threwException;
		}
	}
}