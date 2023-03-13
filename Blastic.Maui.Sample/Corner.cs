using SkiaSharp;

namespace Blastic.Maui.Sample;

public class Corner
{
	public SKPoint Position { get; }
	public Player? Player { get; set; }
	public SettlementType SettlementType { get; set; }

	public Corner(SKPoint position)
	{
		Position = position;
	}
}