using Bindables.Maui;
using Blastic.Skia.Input;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace Blastic.Skia.Maui.TouchCanvas;

public partial class TouchCanvasView : SKCanvasView
{
	public event EventHandler<TapEventArgs>? Tapped;
	public event EventHandler<PanEventArgs>? Panned;
	public event EventHandler<PointerMoveEventArgs>? PointerMoved;
	public event EventHandler<PointerPressEventArgs>? PointerPressed;
	public event EventHandler<PointerReleaseEventArgs>? PointerReleased;

	[BindableProperty(typeof(bool))]
	public static readonly BindableProperty EnableTouchProperty;

	public void SendTap(Point location, InputSource source)
	{
		Tapped?.Invoke(this, new TapEventArgs(location.ToSKPoint(), source));
	}

	public void SendPan(Point location, InputSource source, double force)
	{
		Panned?.Invoke(this, new PanEventArgs(location.ToSKPoint(), source, force));
	}

	public void SendPointerMove(Point location)
	{
		PointerMoved?.Invoke(this, new PointerMoveEventArgs(location.ToSKPoint()));
	}

	public void SendPointerPress(Point location, MouseButton mouseButton)
	{
		PointerPressed?.Invoke(this, new PointerPressEventArgs(location.ToSKPoint(), mouseButton));
	}

	public void SendPointerRelease(Point location, MouseButton mouseButton)
	{
		PointerReleased?.Invoke(this, new PointerReleaseEventArgs(location.ToSKPoint(), mouseButton));
	}
}