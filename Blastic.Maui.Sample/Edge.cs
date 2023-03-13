using SkiaSharp;

namespace Blastic.Maui.Sample;

public class Edge
{
	public SKPoint Position { get; }
	public Player? Player { get; set; }
	public float Slope { get; }

	public Edge(SKPoint firstCorner, SKPoint secondCorner)
	{
		Position = new SKPoint(
			(firstCorner.X + secondCorner.X) / 2,
			(firstCorner.Y + secondCorner.Y) / 2);

		Slope = (secondCorner.Y - firstCorner.Y) / (secondCorner.X - firstCorner.X);
	}
}