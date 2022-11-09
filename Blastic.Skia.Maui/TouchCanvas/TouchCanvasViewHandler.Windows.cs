using Blastic.Skia.Input;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Input;
using SkiaSharp.Views.Windows;

namespace Blastic.Skia.Maui.TouchCanvas;

public partial class TouchCanvasViewHandler
{
	protected override SKXamlCanvas CreatePlatformView()
	{
		return new SKXamlCanvas();
	}

	protected override void ConnectHandler(SKXamlCanvas platformView)
	{
		base.ConnectHandler(platformView);

		platformView.PointerMoved += OnPointerMoved;
		platformView.PointerPressed += (_, e) => OnPointerPressed(e);
		platformView.PointerReleased += (_, e) => OnPointerReleased(e);
	}

	private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
	{
		if (VirtualView is not TouchCanvasView view)
		{
			return;
		}

		Windows.Foundation.Point position = e.GetCurrentPoint(PlatformView).Position;
		Point location = new(position.X, position.Y);

		view.SendPointerMove(location);
	}

	private void OnPointerPressed(PointerRoutedEventArgs e)
	{
		if (VirtualView is not TouchCanvasView view)
		{
			return;
		}

		(Point position, MouseButton? mouseButton) = GetPositionAndMouseButton(e);

		if (mouseButton == null)
		{
			return;
		}

		PlatformView.CapturePointer(e.Pointer);
		view.SendPointerPress(position, mouseButton.Value);
	}

	private void OnPointerReleased(PointerRoutedEventArgs e)
	{
		if (VirtualView is not TouchCanvasView view)
		{
			return;
		}

		(Point position, MouseButton? mouseButton) = GetPositionAndMouseButton(e);

		if (mouseButton == null)
		{
			return;
		}

		PlatformView.ReleasePointerCapture(e.Pointer);
		view.SendPointerRelease(position, mouseButton.Value);
	}

	private (Point location, MouseButton? mouseButton) GetPositionAndMouseButton(PointerRoutedEventArgs e)
	{
		PointerPoint pointerPoint = e.GetCurrentPoint(PlatformView);
		Windows.Foundation.Point position = pointerPoint.Position;

		Point location = new(position.X, position.Y);
		MouseButton? mouseButton = ToMouseButton(pointerPoint);

		return (location, mouseButton);
	}

	private MouseButton? ToMouseButton(PointerPoint pointerPoint)
	{
		PointerPointProperties properties = pointerPoint.Properties;

		if (properties.PointerUpdateKind is PointerUpdateKind.LeftButtonPressed or PointerUpdateKind.LeftButtonReleased)
		{
			return MouseButton.Left;
		}

		if (properties.PointerUpdateKind is PointerUpdateKind.RightButtonPressed or PointerUpdateKind.RightButtonReleased)
		{
			return MouseButton.Right;
		}

		if (properties.PointerUpdateKind is PointerUpdateKind.MiddleButtonPressed or PointerUpdateKind.MiddleButtonReleased)
		{
			return MouseButton.Middle;
		}

		return null;
	}

	public static void MapEnableTouch(TouchCanvasViewHandler handler, TouchCanvasView view)
	{
	}
}