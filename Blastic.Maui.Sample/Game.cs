namespace Blastic.Maui.Sample;

public class Game
{
	public string Id { get; }

	public Game()
	{
		Id = Guid.NewGuid().ToString("N");
	}

	public Game(string id)
	{
		Id = id;
	}
}