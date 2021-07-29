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
		/// Runs the last queued request after the current one finishes, ignoring the requests between.
		/// </summary>
		RunLatest,

		/// <summary>
		/// Cancels the running operation and runs the last request after the cancellation is
		/// complete, ignoring the requests between.
		/// </summary>
		RunLatestCancelRunning
	}
}