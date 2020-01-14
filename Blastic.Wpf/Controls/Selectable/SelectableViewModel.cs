using Blastic.Reactive;

namespace Blastic.Wpf.Controls.Selectable
{
	public class SelectableViewModel<T>
	{
		public T Value { get; }
		public IReactiveProperty<bool> IsSelected { get; set; }

		public SelectableViewModel(T value, bool isSelected = false)
		{
			Value = value;
			IsSelected = new ReactiveProperty<bool>(isSelected);
		}
	}
}