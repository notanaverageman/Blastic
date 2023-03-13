using System.Text.Json.Serialization;
using SkiaSharp;

namespace Blastic.Maui.Sample.Actions;

public class SetRoad : GameAction
{
	public float X { get; }
	public float Y { get; }
	public int PlayerId { get; }

	[JsonConstructor]
	public SetRoad(float x, float y, int playerId)
	{
		X = x;
		Y = y;
		PlayerId = playerId;
	}
	
	public override void Apply(IServiceProvider serviceProvider)
	{
		Board board = serviceProvider.GetRequiredService<Board>();
		MainViewModel mainViewModel = serviceProvider.GetRequiredService<MainViewModel>();

		Player player = mainViewModel.Players.Single(x => x.Id == PlayerId);
		board.SetEdgePlayer(new SKPoint(X, Y), player);
	}

	public override void Undo(IServiceProvider serviceProvider)
	{
		Board board = serviceProvider.GetRequiredService<Board>();
		board.SetEdgePlayer(new SKPoint(X, Y), null);
	}
}