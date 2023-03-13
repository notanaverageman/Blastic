using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Blastic.Commanding;
using Blastic.LifetimeManagement;
using Blastic.Maui.Sample.Actions;
using Blastic.Maui.Sample.Data;
using Blastic.Platform;
using Blastic.Reactive;
using Blastic.Skia;
using Blastic.Skia.Input;
using DynamicData;
using SkiaSharp;
using Animation = Blastic.Animations.Animation;
using Command = Blastic.Commanding.Command;

namespace Blastic.Maui.Sample;

public class MainViewModel : IHasLifetime
{
	private static readonly Dictionary<int, SKImage> DiceImages = new()
	{
		{ 1, Assets.Dice1.Picture.CreateImage() },
		{ 2, Assets.Dice2.Picture.CreateImage() },
		{ 3, Assets.Dice3.Picture.CreateImage() },
		{ 4, Assets.Dice4.Picture.CreateImage() },
		{ 5, Assets.Dice5.Picture.CreateImage() },
		{ 6, Assets.Dice6.Picture.CreateImage() },
	};

	private readonly Random _diceRandom;
	private readonly Random _diceAnimationRandom;

	private readonly Board _board;
	private readonly SourceList<Player> _players;
	
	public GameActionManager GameActionManager { get; }

	private SKPoint? _selectedCornerPosition;
	private SKPoint? _selectedEdgePosition;

	public ILifetime Lifetime { get; }
	public SkiaCanvas SkiaCanvas { get; set; }

	public IReactiveProperty<bool> IsGenerateMapEnabled { get; }
	public IReactiveProperty<bool> IsSettlementSelectorVisible { get; }
	public IReactiveProperty<bool> IsRoadSelectorVisible { get; }

	public IReactiveProperty<SKImage> FirstDice { get; }
	public IReactiveProperty<SKImage> SecondDice { get; }

	public ReadOnlyObservableCollection<Player> Players { get; }
	public List<DiceHistogramEntry> DiceHistogram { get; }

	public Commanding.Command<Player> BuildSettlementCommand { get; }
	public Commanding.Command<Player> BuildCityCommand { get; }
	public Commanding.Command<Player> BuildRoadCommand { get; }
	public Command RollDiceCommand { get; }
	public Command GenerateMapCommand { get; }

	public MainViewModel(
		GameActionManager gameActionManager,
		GameDatabase gameDatabase,
		SkiaCanvas skiaCanvas,
		Board board,
		IPlatformSpecifics platformSpecifics)
	{
		SkiaCanvas = skiaCanvas;
		GameActionManager = gameActionManager;

		_board = board;

		_diceRandom = new Random();
		_diceAnimationRandom = new Random();

		Lifetime = new Lifetime();

		Lifetime.Initialization.Subscribe(() =>
		{
			gameDatabase.OpenConnection();
			gameDatabase.Migrate();

			gameActionManager.Initialize();
		});

		(_players, Players) = SourceListExtensions.CreateAndBind<Player>(platformSpecifics);

		_players.Add(new Player(0, "Yusuf"));
		_players.Add(new Player(1, "Emre"));
		_players.Add(new Player(2, "Zeynep"));
		_players.Add(new Player(3, "Gökhan"));

		SkiaCanvas.DrawCommand.Subscribe(Draw);
		SkiaCanvas.Scale.Value = 0.9f;
		SkiaCanvas.ContentSize.Value = new SKSize(_board.Bounds.Size.Width * 1.2f, _board.Bounds.Size.Height * 1.2f);

		SkiaCanvas.FitMode = _board.Bounds.Width < _board.Bounds.Height
			? CanvasFitMode.ScaleToFitWidth
			: CanvasFitMode.ScaleToFitHeight;

		InputEvents inputEvents = SkiaCanvas.InputEvents;
		
		inputEvents.IsEnabled.Value = true;
		inputEvents.PointerMove.Skip(1).Subscribe(OnPointerMoved);
		inputEvents.PointerPress.Skip(1).Subscribe(OnPointerPressed);
		inputEvents.Tap.Skip(1).Subscribe(OnTapped);

		SkiaCanvas.MaxScrollX
			.Where(x => x > 0)
			.Take(1)
			.Subscribe(x => SkiaCanvas.ScrollX.Value = x / 2);

		SkiaCanvas.MaxScrollY
			.Where(x => x > 0)
			.Take(1)
			.Subscribe(x => SkiaCanvas.ScrollY.Value = x / 2);

		FirstDice = new ReactiveProperty<SKImage>(DiceImages[1]);
		SecondDice = new ReactiveProperty<SKImage>(DiceImages[1]);

		IsGenerateMapEnabled = new ReactiveProperty<bool>(true);
		IsSettlementSelectorVisible = new ReactiveProperty<bool>(false);
		IsRoadSelectorVisible = new ReactiveProperty<bool>(false);

		BuildSettlementCommand = new Commanding.Command<Player>(BuildSettlement);
		BuildCityCommand = new Commanding.Command<Player>(BuildCity);
		BuildRoadCommand = new Commanding.Command<Player>(BuildRoad);
		RollDiceCommand = new Command(RollDice);

		GenerateMapCommand = IsGenerateMapEnabled
			.ToCommand()
			.WithSubscribe(GenerateMap);

		DiceHistogram = new List<DiceHistogramEntry>();

		for (int i = 2; i <= 12; i++)
		{
			DiceHistogramEntry entry = new(i);
			DiceHistogram.Add(entry);
		}
	}

	private void BuildSettlement(Player player)
	{
		if (!_selectedCornerPosition.HasValue)
		{
			return;
		}

		SKPoint position = _selectedCornerPosition.Value;
		SetVillage action = new(position.X, position.Y, player.Id);

		GameActionManager.Apply(action);

		IsSettlementSelectorVisible.Value = false;
	}

	private void BuildCity(Player player)
	{
		if (!_selectedCornerPosition.HasValue)
		{
			return;
		}

		SKPoint position = _selectedCornerPosition.Value;
		SetCity action = new(position.X, position.Y, player.Id);

		GameActionManager.Apply(action);

		IsSettlementSelectorVisible.Value = false;
	}

	private void BuildRoad(Player player)
	{
		if (!_selectedEdgePosition.HasValue)
		{
			return;
		}

		SKPoint position = _selectedEdgePosition.Value;
		SetRoad action = new(position.X, position.Y, player.Id);

		GameActionManager.Apply(action);

		IsRoadSelectorVisible.Value = false;
	}

	private void RollDice()
	{
		Animation
			.Create(TimeSpan.FromSeconds(1))
			.Buffer(TimeSpan.FromMilliseconds(80))
			.Subscribe(
				onNext: x =>
				{
					int animationFirst = _diceAnimationRandom.Next(6) + 1;
					int animationSecond = _diceAnimationRandom.Next(6) + 1;

					FirstDice.Value = DiceImages[animationFirst];
					SecondDice.Value = DiceImages[animationSecond];
				},
				onCompleted: () =>
				{
					int first = _diceRandom.Next(6) + 1;
					int second = _diceRandom.Next(6) + 1;

					int sum = first + second;

					FirstDice.Value = DiceImages[first];
					SecondDice.Value = DiceImages[second];

					DiceHistogramEntry entry = DiceHistogram.Single(x => x.Number == sum);
					entry.Count.Value++;

					foreach (DiceHistogramEntry histogramEntry in DiceHistogram)
					{
						histogramEntry.DiceCount.Value++;
					}
				});
	}

	private void GenerateMap()
	{
		_board.Generate();
		GameActionManager.Reset(new Game());

		foreach (DiceHistogramEntry entry in DiceHistogram)
		{
			entry.Count.Value = 0;
			entry.DiceCount.Value = 0;
		}

		FirstDice.Value = DiceImages[1];
		SecondDice.Value = DiceImages[1];
	}

	private void Draw((SKCanvas Canvas, SKPaint? Paint) args)
	{
		SKCanvas canvas = args.Canvas;

		canvas.Translate(
			SkiaCanvas.ContentSize.Value.Width / 2,
			SkiaCanvas.ContentSize.Value.Height / 2);

		_board.Draw(canvas);
	}

	private void OnPointerMoved(PointerMoveEventArgs args)
	{
		SKPoint position = args.Position;
		SKSize contentSize = SkiaCanvas.ContentSizeScaled.Value;

		position.Offset(-contentSize.Width / 2, -contentSize.Height / 2);

		_board.HoverPoint(position);
	}

	private void OnPointerPressed(PointerPressEventArgs args)
	{
		OnClick(args.Position);
	}

	private void OnTapped(TapEventArgs args)
	{
		OnClick(args.Position);
	}

	private void OnClick(SKPoint position)
	{
		SKSize contentSize = SkiaCanvas.ContentSizeScaled.Value;

		position.Offset(-contentSize.Width / 2, -contentSize.Height / 2);

		(SKPoint? point, PointType pointType) = _board.GetClosestPoint(position);

		IsSettlementSelectorVisible.Value = false;
		IsRoadSelectorVisible.Value = false;

		if (point == null)
		{
			_selectedCornerPosition = null;
			_selectedEdgePosition = null;

			return;
		}

		if (pointType == PointType.Corner)
		{
			_selectedCornerPosition = point;
			IsSettlementSelectorVisible.Value = true;
		}
		else if (pointType == PointType.Edge)
		{
			_selectedEdgePosition = point;
			IsRoadSelectorVisible.Value = true;
		}
		else if (pointType == PointType.Tile)
		{
			_board.PutRobber(point.Value);
		}
	}
}