using System;

namespace Blastic.Commanding.ErrorHandling;

public class UnhandledExceptionEventArgs : EventArgs
{
	public UnhandledExceptionSource Source { get; }
	public Exception Exception { get; }

	public bool Rethrow { get; private set; }

	public UnhandledExceptionEventArgs(UnhandledExceptionSource source, Exception exception)
	{
		Source = source;
		Exception = exception;

		Rethrow = true;
	}

	public void Swallow()
	{
		Rethrow = false;
	}
}