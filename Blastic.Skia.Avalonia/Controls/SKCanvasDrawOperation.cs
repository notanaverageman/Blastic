using Avalonia;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;

namespace Blastic.Skia.Avalonia.Controls;

public class SKCanvasDrawOperation : ICustomDrawOperation
{
	private readonly Action<SKCanvas> _invalidate;

	public Rect Bounds { get; }

	public SKCanvasDrawOperation(Rect bounds, Action<SKCanvas> invalidate)
	{
		Bounds = bounds;
		_invalidate = invalidate;
	}

	public void Render(IDrawingContextImpl context)
	{
		ISkiaSharpApiLeaseFeature? leaseFeature = context.GetFeature<ISkiaSharpApiLeaseFeature>();
		
		if (leaseFeature is null)
		{
			return;
		}

		using ISkiaSharpApiLease lease = leaseFeature.Lease();

		SKCanvas? canvas = lease?.SkCanvas;
		
		if (canvas != null)
		{
			canvas.Translate(-canvas.LocalClipBounds.Left, -canvas.LocalClipBounds.Top - 1);
			_invalidate(canvas);
		}
	}

	public void Dispose()
	{
	}

	public bool HitTest(Point p)
	{
		return Bounds.Contains(p);
	}

	public bool Equals(ICustomDrawOperation? other)
	{
		return false;
	}
}