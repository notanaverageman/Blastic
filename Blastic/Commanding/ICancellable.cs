namespace Blastic.Commanding
{
	public interface ICancellable
	{
		bool IsCancelled { get; }
	}
}