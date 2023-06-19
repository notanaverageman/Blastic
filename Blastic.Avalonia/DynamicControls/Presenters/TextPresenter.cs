using Avalonia;
using Blastic.Reactive;

namespace Blastic.Avalonia.DynamicControls.Presenters;

public class TextPresenter : Presenter
{
	public static readonly AvaloniaProperty MaskProperty = AvaloniaProperty.Register<TextPresenter, IReadOnlyReactiveProperty<string>>(nameof(Mask));
	
	public IReadOnlyReactiveProperty<string?>? Mask
	{
		get => (IReadOnlyReactiveProperty<string?>?)GetValue(MaskProperty);
		set => SetValue(MaskProperty, value);
	}
}