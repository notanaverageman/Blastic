using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;

namespace Blastic.Avalonia.ViewManagement;

public class View
{
	public static readonly AvaloniaProperty<object?> ModelProperty = AvaloniaProperty
		.RegisterAttached<View, Control, object?>(nameof(ModelProperty).Replace("Property", ""));

	public static object? GetModel(AvaloniaObject obj) => obj.GetValue(ModelProperty);
	public static void SetModel(AvaloniaObject obj, object value) => obj.SetValue(ModelProperty, value);

	static View()
	{
		ModelProperty.Changed.AddClassHandler<Control>(OnModelChanged);
	}

	private static void OnModelChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
	{
		if (e.NewValue == null)
		{
			SetContentProperty(d, e.NewValue);
			return;
		}

		StyledElement view = ViewLocator.Current.Locate(e.NewValue);

		if (!SetContentProperty(d, view))
		{
			view = ViewLocator.Current.Locate(e.NewValue.GetType());
			SetContentProperty(d, view);
		}
	}

	private static bool SetContentProperty(object targetLocation, object? view)
	{
		return SetContentPropertyCore(targetLocation, view);
	}

	private static bool SetContentPropertyCore(object targetLocation, object? view)
	{
		if (targetLocation is IContentControl contentControl)
		{
			contentControl.Content = view;
			return true;
		}

		if (targetLocation is IContentPresenter contentPresenter)
		{
			contentPresenter.Content = view;
			return true;
		}

		return false;
	}
}