using Blastic.Commanding;
using Blastic.Reactive;

namespace Blastic.Diagnostics
{
	/// <summary>
	/// A class that has a <see cref="Severity"/> and a <see cref="Message"/>.
	/// Optionally can have a <see cref="Command"/> and a label for that command.
	/// </summary>
	public class DiagnosticMessage
	{
		/// <summary>
		/// An observable property that holds the severity values.
		/// </summary>
		public IReadOnlyReactiveProperty<Severity> Severity { get; }

		/// <summary>
		/// An observable property that holds the message.
		/// </summary>
		public IReadOnlyReactiveProperty<string> Message { get; }

		/// <summary>
		/// Optional command to be executed.
		/// </summary>
		public Command? ActionCommand { get; }

		/// <summary>
		/// An observable property as the label of the <see cref="ActionCommand"/>.
		/// </summary>
		public IReadOnlyReactiveProperty<string>? ActionLabel { get; }

		/// <summary>
		/// Creates a new instance with given observable properties for severity and message.
		/// </summary>
		/// <param name="severity">An observable property for severity.</param>
		/// <param name="message">An observable property for message.</param>
		public DiagnosticMessage(
			IReadOnlyReactiveProperty<Severity> severity,
			IReadOnlyReactiveProperty<string> message)
			:
			this(severity, message, null, null)
		{
		}

		/// <summary>
		/// Creates a new instance with given severity and message.
		/// </summary>
		/// <param name="severity">The constant severity value.</param>
		/// <param name="message">An observable property for message.</param>
		public DiagnosticMessage(
			Severity severity,
			IReadOnlyReactiveProperty<string> message)
			:
			this(new ReactiveProperty<Severity>(severity), message, null, null)
		{
		}

		/// <summary>
		/// Creates a new instance with given observable properties for severity, message, action, and label.
		/// </summary>
		/// <param name="severity">An observable property for severity.</param>
		/// <param name="message">An observable property for message.</param>
		/// <param name="actionCommand">A command to execute.</param>
		/// <param name="actionLabel">An observable property as command's label.</param>
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

		/// <summary>
		/// Creates a new instance with given observable properties for severity and message.
		/// </summary>
		/// <param name="severity">The constant severity value.</param>
		/// <param name="message">The constant message value.</param>
		public DiagnosticMessage(
			Severity severity,
			string message)
			:
			this(severity, message, null, null)
		{
		}

		/// <summary>
		/// Creates a new instance with given observable properties for severity, message, action, and label.
		/// </summary>
		/// <param name="severity">The constant severity value.</param>
		/// <param name="message">The constant message value.</param>
		/// <param name="actionCommand">A command to execute.</param>
		/// <param name="actionLabel">The constant label value for command.</param>
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