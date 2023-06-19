using System.Collections;
using Avalonia;
using Blastic.Reactive;

namespace Blastic.Avalonia.DynamicControls.Presenters;

public class SelectionPresenter : Presenter
{
	public static readonly AvaloniaProperty ValuesProperty = AvaloniaProperty.Register<SelectionPresenter, IEnumerable>(nameof(Values));

	public IEnumerable? Values
	{
		get => (IEnumerable?)GetValue(ValuesProperty);
		set => SetValue(ValuesProperty, value);
	}

	public IReactiveProperty<int> SelectedIndex { get; }
	
	public SelectionPresenter(IReactiveProperty<int> selectedIndex)
	{
		SelectedIndex = selectedIndex;
	}
}