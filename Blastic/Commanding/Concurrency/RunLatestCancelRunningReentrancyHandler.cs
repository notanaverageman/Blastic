using System;
using System.Threading;

namespace Blastic.Commanding.Concurrency;

/// <summary>
/// Cancels the running operation and runs the last request after the cancellation is
/// complete, ignoring the requests between.
/// </summary>
public class RunLatestCancelRunningReentrancyHandler : IReentrancyHandler
{
	private readonly SemaphoreSlim _semaphore = new(initialCount: 1, maxCount: 1);
	
	private CancellationTokenSource? _cancellationTokenSource;
	private bool _acquiredSemaphore;
	private long _executionNumber;

	/// <inheritdoc />
	public bool AllowConcurrentExecution => true;

	/// <inheritdoc />
	public PreExecuteResult PreExecute(CancellationToken cancellationToken)
	{
		long currentExecutionNumber = Interlocked.Increment(ref _executionNumber);

		try
		{
			// Cancel the execution that is already running.
			_cancellationTokenSource?.Cancel();

			// Wait our turn for execution.
			_semaphore.Wait(cancellationToken);
			_acquiredSemaphore = true;
		}
		catch (OperationCanceledException)
		{
			return new PreExecuteResult(false, cancellationToken);
		}

		if (currentExecutionNumber < _executionNumber)
		{
			// There are newer executions than this one, abort this execution.
			return new PreExecuteResult(false, cancellationToken);
		}

		// Update the cancellation token to be able to cancel execution if another execution comes
		// after this one.
		_cancellationTokenSource?.Dispose();
		_cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

		return new PreExecuteResult(true, _cancellationTokenSource.Token);
	}

	/// <inheritdoc />
	public void PostExecute()
	{
		_cancellationTokenSource?.Dispose();
		_cancellationTokenSource = null;

		if (_acquiredSemaphore)
		{
			_semaphore.Release();
		}
	}
}