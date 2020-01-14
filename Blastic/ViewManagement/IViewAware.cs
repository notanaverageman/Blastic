using Blastic.Reactive;

namespace Blastic.ViewManagement
{
	public interface IViewAware
	{
		IReactiveProperty<object> View { get; }
	}
}