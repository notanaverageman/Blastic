using System.Threading;

namespace Blastic.Commanding.Concurrency;

/// <summary>
/// No restriction on reentrancy.
/// </summary>
public class ConcurrentReentrancyHandler : IReentrancyHandler
{
	/// <summary>
	/// Singleton instance.
	/// </summary>
	public static readonly ConcurrentReentrancyHandler Instance = new();

	/// <inheritdoc/>
	public bool AllowConcurrentExecution => true;
	
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