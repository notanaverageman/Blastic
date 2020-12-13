using Blastic.Reactive;

namespace Blastic.ViewManagement
{
	/// <summary>
	/// An interface implemented by viewmodels to get their views.
	/// </summary>
	public interface IViewAware
	{
		IReactiveProperty<object?> View { get; }
	}
}