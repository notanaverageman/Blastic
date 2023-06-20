using System;
using System.Threading;

namespace Blastic.Commanding.Concurrency;

/// <summary>
/// Runs the last queued request after the current one finishes, ignoring the requests between.
/// </summary>
public class RunLatestReentrancyHandler : IReentrancyHandler
{
	private readonly SemaphoreSlim _semaphore = new(initialCount: 1, maxCount: 1);
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

		return new PreExecuteResult(true, cancellationToken);
	}

	/// <inheritdoc />
	public void PostExecute()
	{
		if (_acquiredSemaphore)
		{
			_semaphore.Release();
		}
	}
}