using SkiaSharp;

namespace Blastic.Maui.Sample;

public class Tile
{
	public SKPoint Position { get; }
	public ResourceType ResourceType { get; }
	public int Number { get; }

	public SKRect Bounds { get; }
	public IReadOnlyList<SKPoint> Corners { get; }

	public Tile(SKPoint position, ResourceType resourceType, int number)
	{
		ResourceType = resourceType;
		Number = number;
		
		float xOffset = MathF.Sqrt(3) / 2 * position.X;
		float yOffset = 1.5f * position.Y;

		Position = new SKPoint(xOffset, yOffset);

		Bounds = SKRect.Create(
			xOffset,
			yOffset,
			MathF.Sqrt(3),
			2);

		Corners = GetCorners()
			.Select(x => new SKPoint(x.X + xOffset, x.Y + yOffset))
			.ToArray();
	}

	public void Draw(SKCanvas canvas)
	{
		canvas.DrawPictureCentered(GetTilePicture(ResourceType), scale: 4);

		if (Number == 0)
		{
			return;
		}

		using (new SKAutoCanvasRestore(canvas))
		{
			canvas.Translate(0, 0.3f);
			canvas.DrawPictureCentered(GetNumberPicture(Number));
		}
	}

	public void DrawHighlight(SKCanvas canvas)
	{
		canvas.DrawPictureCentered(GetTilePicture(ResourceType), scale: 4);
	}

	private static SKPoint[] GetCorners()
	{
		float size = 1;
		float halfSize = size / 2;
		float diagonalSize = MathF.Sqrt(3) * size / 2;

		return new SKPoint[]
		{
			new(0, size),
			new(diagonalSize, halfSize),
			new(diagonalSize, -halfSize),
			new(0, -size),
			new(-diagonalSize, -halfSize),
			new(-diagonalSize, halfSize)
		};
	}

	private static SKPicture GetTilePicture(ResourceType resourceType)
	{
		return resourceType switch
		{
			ResourceType.Brick => Assets.TileBrick.Picture,
			ResourceType.Desert => Assets.TileDesert.Picture,
			ResourceType.Grain => Assets.TileGrain.Picture,
			ResourceType.Ore => Assets.TileOre.Picture,
			ResourceType.Sheep => Assets.TileSheep.Picture,
			ResourceType.Wood => Assets.TileWood.Picture,
			_ => throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null)
		};
	}

	private static SKPicture GetNumberPicture(int number)
	{
		return number switch
		{
			2 => Assets.Number2.Picture,
			3 => Assets.Number3.Picture,
			4 => Assets.Number4.Picture,
			5 => Assets.Number5.Picture,
			6 => Assets.Number6.Picture,
			8 => Assets.Number8.Picture,
			9 => Assets.Number9.Picture,
			10 => Assets.Number10.Picture,
			11 => Assets.Number11.Picture,
			12 => Assets.Number12.Picture,
			_ => throw new ArgumentOutOfRangeException(nameof(number), number, null)
		};
	}
}