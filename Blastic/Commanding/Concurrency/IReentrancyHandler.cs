using System.Threading;

namespace Blastic.Commanding.Concurrency;

/// <summary>
/// Defines the reentrancy behavior of <see cref="Command"/> or <see cref="AsyncCommand"/>
/// </summary>
public interface IReentrancyHandler
{
	/// <summary>
	/// Specify if the command allows concurrent execution. Exact behavior depends on
	/// the implementation.
	/// </summary>
	bool AllowConcurrentExecution { get; }

	/// <summary>
	/// Return true if the execution should continue, or false if the execution should abort.
	/// </summary>
	/// <param name="cancellationToken">Token to cancel execution.</param>
	/// <returns>Result to specify whether to continue execution or not.</returns>
	PreExecuteResult PreExecute(CancellationToken cancellationToken);

	/// <summary>
	/// Cleanup after successful or aborted execution.
	/// </summary>
	void PostExecute();
}