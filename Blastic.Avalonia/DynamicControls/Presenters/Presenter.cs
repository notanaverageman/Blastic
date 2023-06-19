using Avalonia;
using Avalonia.Controls;
using Blastic.DynamicControls;
using Blastic.Reactive;

namespace Blastic.Avalonia.DynamicControls.Presenters;

public class Presenter : ContentControl, IPresenter
{
	public static readonly AvaloniaProperty LabelProperty = AvaloniaProperty.Register<Presenter, IReadOnlyReactiveProperty<string>>(nameof(Label));
	public IReadOnlyReactiveProperty<string?>? Label
	{
		get => (IReadOnlyReactiveProperty<string>?)GetValue(LabelProperty);
		set => SetValue(LabelProperty, value);
	}

	public static readonly AvaloniaProperty PropertyProperty = AvaloniaProperty.Register<Presenter, IReadOnlyReactiveProperty>(nameof(Property));
	public IReadOnlyReactiveProperty? Property
	{
		get => (IReadOnlyReactiveProperty?)GetValue(PropertyProperty);
		set => SetValue(PropertyProperty, value);
	}

	public static readonly AvaloniaProperty HelpProperty = AvaloniaProperty.Register<Presenter, IReadOnlyReactiveProperty<string>>(nameof(Help));
	public IReadOnlyReactiveProperty<string?>? Help
	{
		get => (IReadOnlyReactiveProperty<string?>?)GetValue(HelpProperty);
		set => SetValue(HelpProperty, value);
	}

	public static readonly AvaloniaProperty IconProperty = AvaloniaProperty.Register<Presenter, IReadOnlyReactiveProperty>(nameof(Icon));
	public IReadOnlyReactiveProperty? Icon
	{
		get => (IReadOnlyReactiveProperty?)GetValue(IconProperty);
		set => SetValue(IconProperty, value);
	}

	public static readonly AvaloniaProperty IsEnabledReactiveProperty = AvaloniaProperty.Register<Presenter, IReadOnlyReactiveProperty<bool>>(nameof(IsEnabledReactive));
	public IReadOnlyReactiveProperty<bool>? IsEnabledReactive
	{
		get => (IReadOnlyReactiveProperty<bool>?)GetValue(IsEnabledReactiveProperty);
		set => SetValue(IsEnabledReactiveProperty, value);
	}

	// TODO: BindableProperty?
	public Blastic.DynamicControls.Properties.GridLength ColumnWidth { get; set; }
	public Blastic.DynamicControls.Properties.Thickness IconMargin { get; set; }
	public double IconSize { get; set; }
}