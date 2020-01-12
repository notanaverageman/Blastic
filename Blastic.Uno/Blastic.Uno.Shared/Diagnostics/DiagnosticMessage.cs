using System;
using System.Threading.Tasks;
using Blastic.Commanding;
using Blastic.Common.Diagnostics;

namespace Blastic.Diagnostics
{
	public class DiagnosticMessage
	{
		public Severity Severity { get; }
		public string Message { get; }

		public Func<Task> Action { get; }
		public string ActionLabel { get; }

		public AsyncCommand ActionCommand { get; }

		public DiagnosticMessage(Severity severity, string message)
			:
			this(severity, message, null, null)
		{
			ActionCommand = new AsyncCommand().WithSubscribe(async x =>
			{
				if (Action != null)
				{
					await Action();
				}
			});
		}

		public DiagnosticMessage(
			Severity severity,
			string message,
			Func<Task> action,
			string actionLabel)
		{
			Severity = severity;
			Message = message;
			Action = action;
			ActionLabel = actionLabel;
		}
	}
}