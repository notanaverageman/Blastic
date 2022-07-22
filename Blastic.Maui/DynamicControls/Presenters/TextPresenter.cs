using Blastic.Reactive;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace Blastic.Maui.DynamicControls.Presenters;

public class TextPresenter : Presenter
{
	public static readonly BindableProperty MaskProperty = BindableProperty.Create(
		nameof(Mask),
		typeof(IReadOnlyReactiveProperty<string>),
		typeof(TextPresenter));
	public IReadOnlyReactiveProperty<string> Mask
	{
		get => (IReadOnlyReactiveProperty<string>)GetValue(MaskProperty);
		set => SetValue(MaskProperty, value);
	}

	public static readonly BindableProperty KeyboardProperty = BindableProperty.Create(
		nameof(Keyboard),
		typeof(IReadOnlyReactiveProperty<Keyboard>),
		typeof(TextPresenter));
	public IReadOnlyReactiveProperty<Keyboard> Keyboard
	{
		get => (IReadOnlyReactiveProperty<Keyboard>)GetValue(KeyboardProperty);
		set => SetValue(KeyboardProperty, value);
	}
}