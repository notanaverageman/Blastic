using Blastic.Skia;
using SkiaSharp;

namespace Blastic.Maui.Sample;

public class Board
{
	private const float HighlightSize = 0.2f;

	private static readonly SKPoint[] TilePositions =
	{
		new(-2, -2), new(0, -2), new(2, -2),
		new(-3, -1), new(-1, -1), new(1, -1), new(3, -1),
		new(-4, 0), new(-2, 0), new(0, 0), new(2, 0), new(4, 0),
		new(-3, 1), new(-1, 1), new(1, 1), new(3, 1),
		new(-2, 2), new(0, 2), new(2, 2),
	};

	private static readonly SKPoint[] PortPositions =
	{
		new(-3.8f, -1.8f),
		new(-5.6f, 0),
		new(-3.8f, 1.8f),
		new(-0.8f, 2.8f),
		new(2.8f, 2.7f),
		new(4.4f, 1f),
		new(4.4f, -1f),
		new(2.8f, -2.8f),
		new(-0.8f, -2.8f),
	};

	private readonly int[] _numbers =
	{
		2, 3, 3, 4, 4, 5, 5, 6, 6, 8, 8, 9, 9, 10, 10, 11, 11, 12
	};

	private readonly ResourceType[] _resourceTypes =
	{
		ResourceType.Wood,
		ResourceType.Wood,
		ResourceType.Wood,
		ResourceType.Wood,
		ResourceType.Brick,
		ResourceType.Brick,
		ResourceType.Brick,
		ResourceType.Sheep,
		ResourceType.Sheep,
		ResourceType.Sheep,
		ResourceType.Sheep,
		ResourceType.Grain,
		ResourceType.Grain,
		ResourceType.Grain,
		ResourceType.Grain,
		ResourceType.Ore,
		ResourceType.Ore,
		ResourceType.Ore
	};

	private readonly PortType[] _portTypes =
	{
		PortType.ThreeToOne,
		PortType.ThreeToOne,
		PortType.ThreeToOne,
		PortType.ThreeToOne,
		PortType.Brick,
		PortType.Grain,
		PortType.Ore,
		PortType.Sheep,
		PortType.Wood,
	};

	private readonly Random _random;
	private readonly SkiaCanvas _skiaCanvas;

	private readonly SKColor _background = SKColors.DeepSkyBlue;
	private readonly SKPaint _highlightPaint;

	private readonly List<SKPoint> _highlightedPoints;

	private readonly Dictionary<SKPoint, Tile> _tiles;
	private readonly List<Tile> _tilesToHighlight;
	
	private SKPicture _picture;
	private SKPoint _robberPosition;
	private List<Corner> _corners;
	private List<Edge> _edges;

	public SKRect Bounds { get; private set; }
	public IReadOnlyCollection<Tile> Tiles => _tiles.Values;


	public Board(SkiaCanvas skiaCanvas)
	{
		_skiaCanvas = skiaCanvas;
		_tiles = new Dictionary<SKPoint, Tile>();
		_tilesToHighlight = new List<Tile>();
		
		_highlightedPoints = new List<SKPoint>();

		_highlightPaint = new SKPaint
		{
			IsAntialias = true,
			Style = SKPaintStyle.Fill,
			ColorFilter = SKColorFilter.CreateBlendMode(SKColor.Parse("88eaeae5"), SKBlendMode.SrcIn)
		};

		_random = new Random(0);

		Generate();
	}

	public void Generate()
	{
		List<SKPoint> allTilePositions = TilePositions.ToList();

		int desertTileIndex = _random.Next(allTilePositions.Count);

		SKPoint desertTilePosition = allTilePositions[desertTileIndex];
		allTilePositions.RemoveAt(desertTileIndex);
		
		_tiles[desertTilePosition] = new Tile(desertTilePosition, ResourceType.Desert, 0);

		_numbers.Shuffle(_random);
		_resourceTypes.Shuffle(_random);
		_portTypes.Shuffle(_random);

		for (int i = 0; i < allTilePositions.Count; i++)
		{
			SKPoint positions = allTilePositions[i];
			int number = _numbers[i];
			ResourceType resourceType = _resourceTypes[i];

			Tile tile = new(positions, resourceType, number);
			_tiles[positions] = tile;
		}

		_picture = CreatePicture();

		float left = _tiles.Min(x => x.Value.Bounds.Left);
		float top = _tiles.Min(x => x.Value.Bounds.Top);
		float right = _tiles.Max(x => x.Value.Bounds.Right);
		float bottom = _tiles.Max(x => x.Value.Bounds.Bottom);

		Bounds = new SKRect(left, top, right, bottom);

		_corners = _tiles.Values
			.SelectMany(x => x.Corners)
			.ToHashSet()
			.Select(x => new Corner(x))
			.ToList();

		_edges = new List<Edge>();
		HashSet<SKPoint> uniqueEdges = new();

		foreach (Tile tile in _tiles.Values)
		{
			IReadOnlyList<SKPoint> corners = tile.Corners;

			for (int i = 0; i < corners.Count; i++)
			{
				int first = i;
				int second = i + 1;

				if (second == corners.Count)
				{
					second = 0;
				}

				if (!uniqueEdges.Add(corners[first] + corners[second]))
				{
					continue;
				}

				Edge edge = new(corners[first], corners[second]);
				_edges.Add(edge);
			}
		}

		PutRobber(_tiles[desertTilePosition].Position);

		_skiaCanvas.Redraw();
	}

	private SKPicture CreatePicture()
	{
		using SKPictureRecorder recorder = new();
		
		SKCanvas canvas = recorder.BeginRecording(new SKRect(-10000, -10000, 10000, 10000));
		canvas.Clear(_background);

		foreach (Tile tile in _tiles.Values)
		{
			using (new SKAutoCanvasRestore(canvas))
			{
				canvas.Translate(tile.Bounds.Location);
				tile.Draw(canvas);
			}
		}

		for (int i = 0; i < PortPositions.Length; i++)
		{
			PortType portType = _portTypes[i];
			SKPoint position = PortPositions[i];

			float xOffset = MathF.Sqrt(3) / 2 * position.X;
			float yOffset = 1.5f * position.Y;

			SKPicture portPicture = GetPortPicture(portType);

			using (new SKAutoCanvasRestore(canvas))
			{
				canvas.Translate(xOffset, yOffset);
				canvas.DrawPictureCentered(portPicture, scale: 1.3f);
			}
		}

		return recorder.EndRecording();
	}

	public void SetCornerPlayer(SKPoint position, Player? player, SettlementType settlementType)
	{
		Corner? corner = _corners.SingleOrDefault(x => x.Position == position);

		if (corner == null)
		{
			return;
		}

		corner.Player = player;
		corner.SettlementType = settlementType;

		_skiaCanvas.Redraw();
	}

	public void SetEdgePlayer(SKPoint position, Player? player)
	{
		Edge? edge = _edges.SingleOrDefault(x => x.Position == position);

		if (edge == null)
		{
			return;
		}

		edge.Player = player;
		_skiaCanvas.Redraw();
	}

	public void PutRobber(SKPoint position)
	{
		_robberPosition = position;
		_skiaCanvas.Redraw();
	}

	public void AddTileToHighlight(Tile tile)
	{
		_tilesToHighlight.Add(tile);
	}

	public void ClearTilesToHighlight()
	{
		_tilesToHighlight.Clear();
	}

	public void HoverPoint(SKPoint point)
	{
		_highlightedPoints.Clear();

		SKPoint? closestPoint = GetClosestPoint(point).Position;

		if (closestPoint.HasValue)
		{
			_highlightedPoints.Add(closestPoint.Value);
		}

		_skiaCanvas.Redraw();
	}

	public (SKPoint? Position, PointType Type) GetClosestPoint(SKPoint point)
	{
		SKPoint closestCornerPoint = _corners.MinBy(x => SKPoint.Distance(x.Position, point))!.Position;
		SKPoint closestEdgePoint = _edges.MinBy(x => SKPoint.Distance(x.Position, point))!.Position;
		SKPoint closestTilePoint = _tiles.Values.MinBy(x => SKPoint.Distance(x.Position, point))!.Position;

		float cornerDistance = SKPoint.Distance(point, closestCornerPoint);
		float edgeDistance = SKPoint.Distance(point, closestEdgePoint);
		float tileDistance = SKPoint.Distance(point, closestTilePoint);

		float minDistance = MathF.Min(cornerDistance, MathF.Min(edgeDistance, tileDistance));

		SKPoint closestPoint = closestCornerPoint;
		PointType type = PointType.Corner;

		if (minDistance == edgeDistance)
		{
			closestPoint = closestEdgePoint;
			type = PointType.Edge;
		}
		else if (minDistance == tileDistance)
		{
			closestPoint = closestTilePoint;
			type = PointType.Tile;
		}
		
		if (minDistance < HighlightSize)
		{
			return (closestPoint, type);
		}
		if (minDistance == tileDistance && minDistance < HighlightSize * 3)
		{
			return (closestPoint, type);
		}

		return (null, type);
	}

	public void Draw(SKCanvas canvas)
	{
		canvas.DrawPicture(_picture);

		using (new SKAutoCanvasRestore(canvas))
		{
			canvas.Translate(_robberPosition);
			canvas.Translate(-0.4f, 0f);
			canvas.DrawPictureCentered(Assets.Robber.Picture);
		}

		foreach (Tile tile in _tilesToHighlight)
		{
			using (new SKAutoCanvasRestore(canvas))
			{
				canvas.Translate(tile.Bounds.Location);
				tile.DrawHighlight(canvas);
			}
		}

		foreach (SKPoint point in _highlightedPoints)
		{
			canvas.DrawCircle(point, HighlightSize, _highlightPaint);
		}

		foreach (Corner corner in _corners)
		{
			Player? player = corner.Player;

			if (player == null)
			{
				continue;
			}

			using (new SKAutoCanvasRestore(canvas))
			{
				canvas.Translate(corner.Position);
				player.DrawSettlement(canvas, corner.SettlementType);
			}
		}

		foreach (Edge edge in _edges)
		{
			Player? player = edge.Player;

			if (player == null)
			{
				continue;
			}

			using (new SKAutoCanvasRestore(canvas))
			{
				canvas.Translate(edge.Position);
				canvas.RotateRadians(MathF.Atan(edge.Slope) + MathF.PI / 2);
				player.DrawRoad(canvas);
			}
		}
	}

	private static SKPicture GetPortPicture(PortType portType)
	{
		return portType switch
		{
			PortType.ThreeToOne => Assets.Port.Picture,
			PortType.Brick => Assets.PortBrick.Picture,
			PortType.Grain => Assets.PortGrain.Picture,
			PortType.Ore => Assets.PortOre.Picture,
			PortType.Sheep => Assets.PortSheep.Picture,
			PortType.Wood => Assets.PortWood.Picture,
			_ => throw new ArgumentOutOfRangeException(nameof(portType), portType, null)
		};
	}
}