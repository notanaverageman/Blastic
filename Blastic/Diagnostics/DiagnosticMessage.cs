using Blastic.Commanding;
using Blastic.Reactive;

namespace Blastic.Diagnostics
{
	public class DiagnosticMessage
	{
		public IReadOnlyReactiveProperty<Severity> Severity { get; }
		public IReadOnlyReactiveProperty<string> Message { get; }

		public Command? ActionCommand { get; }
		public IReadOnlyReactiveProperty<string>? ActionLabel { get; }

		public DiagnosticMessage(
			IReadOnlyReactiveProperty<Severity> severity,
			IReadOnlyReactiveProperty<string> message)
			:
			this(severity, message, null, null)
		{
		}

		public DiagnosticMessage(
			IReadOnlyReactiveProperty<Severity> severity,
			IReadOnlyReactiveProperty<string> message,
			Command? actionCommand,
			IReadOnlyReactiveProperty<string>? actionLabel)
		{
			Severity = severity;
			Message = message;
			ActionCommand = actionCommand;
			ActionLabel = actionLabel;
		}

		public DiagnosticMessage(
			Severity severity,
			string message)
			:
			this(severity, message, null, null)
		{
		}

		public DiagnosticMessage(
			Severity severity,
			string message,
			Command? actionCommand,
			string? actionLabel)
			:
			this(
				new ReactiveProperty<Severity>(severity),
				new ReactiveProperty<string>(message),
				actionCommand,
				actionLabel == null
					? null
					: new ReactiveProperty<string>(actionLabel))
		{
		}
	}
}