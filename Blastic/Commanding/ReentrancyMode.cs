namespace Blastic.Commanding
{
	public enum ReentrancyMode
	{
		/// <summary>
		/// No restriction on reentrancy.
		/// </summary>
		Enabled,

		/// <summary>
		/// Does not execute the current request if there is already one running.
		/// </summary>
		IgnoreReentrant,

		/// <summary>
		/// Cancels the running operation and runs the current request after the cancellation is complete.
		/// </summary>
		CancelRunning
	}
}