using System.Windows;
using Blastic.Reactive;

namespace Blastic.ViewManagement
{
	public interface IViewAware
	{
		IReactiveProperty<UIElement> View { get; }
	}
}