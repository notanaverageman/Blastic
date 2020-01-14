namespace Blastic.LifetimeManagement
{
	public interface IHasLifetime
	{
		ILifetime Lifetime { get; }
	}
}