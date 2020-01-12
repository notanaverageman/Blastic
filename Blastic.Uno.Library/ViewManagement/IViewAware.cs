using Windows.UI.Xaml;
using Blastic.Reactive;

namespace Blastic.ViewManagement
{
	public interface IViewAware
	{
		IReactiveProperty<FrameworkElement> View { get; }
	}
}