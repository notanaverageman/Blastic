using System.Windows;
using Reactive.Bindings;

namespace Blastic.ViewManagement
{
	public interface IViewAware
	{
		IReactiveProperty<UIElement> View { get; }
	}
}