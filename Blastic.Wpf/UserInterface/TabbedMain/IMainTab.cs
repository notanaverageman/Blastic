using Blastic.LifetimeManagement;
using Blastic.Ordering;
using Blastic.Reactive;

namespace Blastic.Wpf.UserInterface.TabbedMain
{
	public interface IMainTab : IHasLifetime
	{
		Order Order { get; }
		bool IsFixed { get; }

		IReactiveProperty<string> Title { get; }
	}
}