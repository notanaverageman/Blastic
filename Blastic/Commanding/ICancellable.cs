namespace Blastic.Commanding
{
	/// <summary>
	/// An interface that is used by <see cref="Command"/> to stop execution when the
	/// parameter implements this interface and <see cref="IsCancelled"/> returns true.
	/// </summary>
	public interface ICancellable
	{
		/// <summary>
		/// Returns true if cancelled.
		/// </summary>
		bool IsCancelled { get; }
	}
}