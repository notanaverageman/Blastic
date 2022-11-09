namespace Blastic.Skia.Maui.Scrolling;

public class ViewportChangedEventArgs : EventArgs
{
	public double ScrollX { get; }
	public double ScrollY { get; }
	public double ZoomScale { get; }

	public ViewportChangedEventArgs(double scrollX, double scrollY, double zoomScale)
	{
		ScrollX = scrollX;
		ScrollY = scrollY;
		ZoomScale = zoomScale;
	}
}