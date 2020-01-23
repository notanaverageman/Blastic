using Blastic.LifetimeManagement;
using Blastic.Ordering;
using Blastic.Reactive;

namespace Blastic.Forms.UserInterface
{
	public interface IShellTab : IHasLifetime
	{
		Order Order { get; }
		IReadOnlyReactiveProperty<string> Title { get; }
	}
}