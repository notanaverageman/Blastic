using System;
using System.Threading.Tasks;
using Reactive.Bindings;

namespace Blastic.Diagnostics
{
	public class DiagnosticMessage
	{
		public Severity Severity { get; }
		public string Message { get; }

		public Func<Task> Action { get; }
		public string ActionLabel { get; }

		public AsyncReactiveCommand ActionCommand { get; set; }

		public DiagnosticMessage(Severity severity, string message)
			:
			this(severity, message, null, null)
		{
			ActionCommand = new AsyncReactiveCommand().WithSubscribe(async () => await Action());
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