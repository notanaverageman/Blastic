using SkiaSharp;

namespace Blastic.Skia.Avalonia.Controls;

public class SKCanvasEventArgs
{
	public SKCanvas Canvas { get; }

	public SKCanvasEventArgs(SKCanvas canvas)
	{
		Canvas = canvas;
	}
}