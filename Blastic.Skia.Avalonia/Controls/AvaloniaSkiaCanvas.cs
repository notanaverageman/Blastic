using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using SkiaSharp;

namespace Blastic.Skia.Avalonia.Controls;

public class AvaloniaSkiaCanvas : Control
{
	public event EventHandler<SKCanvasEventArgs>? Draw;

	public override void Render(DrawingContext context)
	{
		Rect viewPort = new(Bounds.Size);

		SKCanvasDrawOperation drawOperation = new(
			new Rect(0, 0, viewPort.Width, viewPort.Height),
			RaiseOnDraw);

		context.Custom(drawOperation);
	}

	private void RaiseOnDraw(SKCanvas canvas)
	{
		Dispatcher.UIThread.InvokeAsync(() =>
		{
			SKCanvasEventArgs e = new(canvas);
			Draw?.Invoke(this, e);
		}).Wait();
	}
}