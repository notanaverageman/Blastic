using Blastic.LifetimeManagement;
using Blastic.Ordering;
using Blastic.Reactive;

namespace Blastic.UserInterface.TabbedMain
{
	public interface IMainTab : IHasLifetime
	{
		Order Order { get; }
		bool IsFixed { get; }

		IReactiveProperty<string> Title { get; }
	}
}