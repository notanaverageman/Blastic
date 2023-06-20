using System.Threading;

namespace Blastic.Commanding.Concurrency;

/// <summary>
/// Does not execute the current request if there is already one running.
/// </summary>
public class IgnoreReentrantReentrancyHandler : IReentrancyHandler
{
	/// <summary>
	/// Singleton instance.
	/// </summary>
	public static readonly IgnoreReentrantReentrancyHandler Instance = new();

	/// <inheritdoc/>
	public bool AllowConcurrentExecution => false;
	
	/// <inheritdoc/>
	public PreExecuteResult PreExecute(CancellationToken cancellationToken)
	{
		return new PreExecuteResult(true, cancellationToken);
	}

	/// <inheritdoc/>
	public void PostExecute()
	{
	}
}