namespace Blastic.Skia.Maui.Scrolling;

public class ZoomedEventArgs : EventArgs
{
	public double ZoomScale { get; }

	public ZoomedEventArgs(double zoomScale)
	{
		ZoomScale = zoomScale;
	}
}