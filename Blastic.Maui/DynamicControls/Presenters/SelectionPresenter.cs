using System.Collections;
using Microsoft.Maui.Controls;

namespace Blastic.Maui.DynamicControls.Presenters;

public class SelectionPresenter : Presenter
{
	public static readonly BindableProperty ValuesProperty = BindableProperty.Create(
		nameof(Values).Replace("Property", ""),
		typeof(IEnumerable),
		typeof(SelectionPresenter));
	public IEnumerable Values
	{
		get => (IEnumerable)GetValue(ValuesProperty);
		set => SetValue(ValuesProperty, value);
	}
}