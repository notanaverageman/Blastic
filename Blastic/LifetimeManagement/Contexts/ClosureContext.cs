using Blastic.Commanding;

namespace Blastic.LifetimeManagement.Contexts
{
	/// <summary>
	/// Parameter for <see cref="ILifetime.Closure"/> command.
	/// </summary>
	/// <remarks>
	/// This class implements <see cref="ICancellable"/> so that it can be
	/// used to cancel the closure.
	/// </remarks>
	public class ClosureContext : ICancellable
	{
		/// <summary>
		/// Result of closure. It is null if the closure is cancelled.
		/// </summary>
		public bool? Result { get; private set; }

		/// <summary>
		/// Returns true if the closure is cancelled.
		/// </summary>
		public bool IsCancelled => Result == null;

		/// <summary>
		/// Creates a new instance with given result.
		/// </summary>
		/// <param name="result">The result value.</param>
		public ClosureContext(bool result = false)
		{
			Result = result;
		}

		/// <summary>
		/// Cancel the closure.
		/// </summary>
		public void Cancel()
		{
			Result = null;
		}
	}
}