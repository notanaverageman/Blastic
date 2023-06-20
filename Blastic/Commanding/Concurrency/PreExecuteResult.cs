using System.Threading;

namespace Blastic.Commanding.Concurrency;

/// <summary>
/// Result of the <see cref="IReentrancyHandler.PreExecute"/> method.
/// </summary>
public readonly struct PreExecuteResult
{
	/// <summary>
	/// Continue execution of this request.
	/// </summary>
	public bool ContinueExecution { get; }

	/// <summary>
	/// Cancellation token that may be updated by the <see cref="IReentrancyHandler"/>.
	/// </summary>
	public CancellationToken UpdatedCancellationToken { get; }

	/// <summary>
	/// Creates a new instance of <see cref="PreExecuteResult"/>.
	/// </summary>
	/// <param name="continueExecution">Continue execution of this request.</param>
	/// <param name="updatedCancellationToken">Cancellation token that may be updated by the <see cref="IReentrancyHandler"/>.</param>
	public PreExecuteResult(bool continueExecution, CancellationToken updatedCancellationToken)
	{
		ContinueExecution = continueExecution;
		UpdatedCancellationToken = updatedCancellationToken;
	}
}