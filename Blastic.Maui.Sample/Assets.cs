using SkiaSharp;
using Svg.Skia;

namespace Blastic.Maui.Sample;

internal static class Assets
{
	private static readonly Lazy<SKSvg> RobberSvg = new(() => Load("robber.svg"));
	public static SKPicture Robber => RobberSvg.Value.Picture!;
	private static readonly Lazy<SKSvg> VillageGreenSvg = new(() => Load("village_green.svg"));
	public static SKPicture VillageGreen => VillageGreenSvg.Value.Picture!;
	private static readonly Lazy<SKSvg> VillageBlueSvg = new(() => Load("village_blue.svg"));
	public static SKPicture VillageBlue => VillageBlueSvg.Value.Picture!;
	private static readonly Lazy<SKSvg> VillageRedSvg = new(() => Load("village_red.svg"));
	public static SKPicture VillageRed => VillageRedSvg.Value.Picture!;
	private static readonly Lazy<SKSvg> VillageBlackSvg = new(() => Load("village_black.svg"));
	public static SKPicture VillageBlack => VillageBlackSvg.Value.Picture!;
	private static readonly Lazy<SKSvg> CityGreenSvg = new(() => Load("city_green.svg"));
	public static SKPicture CityGreen => CityGreenSvg.Value.Picture!;
	private static readonly Lazy<SKSvg> CityBlueSvg = new(() => Load("city_blue.svg"));
	public static SKPicture CityBlue => CityBlueSvg.Value.Picture!;
	private static readonly Lazy<SKSvg> CityRedSvg = new(() => Load("city_red.svg"));
	public static SKPicture CityRed => CityRedSvg.Value.Picture!;
	private static readonly Lazy<SKSvg> CityBlackSvg = new(() => Load("city_black.svg"));
	public static SKPicture CityBlack => CityBlackSvg.Value.Picture!;
	private static readonly Lazy<SKSvg> RoadGreenSvg = new(() => Load("road_green.svg"));
	public static SKPicture RoadGreen => RoadGreenSvg.Value.Picture!;
	private static readonly Lazy<SKSvg> RoadBlueSvg = new(() => Load("road_blue.svg"));
	public static SKPicture RoadBlue => RoadBlueSvg.Value.Picture!;
	private static readonly Lazy<SKSvg> RoadRedSvg = new(() => Load("road_red.svg"));
	public static SKPicture RoadRed => RoadRedSvg.Value.Picture!;
	private static readonly Lazy<SKSvg> RoadBlackSvg = new(() => Load("road_black.svg"));
	public static SKPicture RoadBlack => RoadBlackSvg.Value.Picture!;
	private static readonly Lazy<SKSvg> PortSvg = new(() => Load("port.svg"));
	public static SKPicture Port => PortSvg.Value.Picture!;
	private static readonly Lazy<SKSvg> PortBrickSvg = new(() => Load("port_brick.svg"));
	public static SKPicture PortBrick => PortBrickSvg.Value.Picture!;
	private static readonly Lazy<SKSvg> PortGrainSvg = new(() => Load("port_grain.svg"));
	public static SKPicture PortGrain => PortGrainSvg.Value.Picture!;
	private static readonly Lazy<SKSvg> PortOreSvg = new(() => Load("port_ore.svg"));
	public static SKPicture PortOre => PortOreSvg.Value.Picture!;
	private static readonly Lazy<SKSvg> PortSheepSvg = new(() => Load("port_sheep.svg"));
	public static SKPicture PortSheep => PortSheepSvg.Value.Picture!;
	private static readonly Lazy<SKSvg> PortWoodSvg = new(() => Load("port_wood.svg"));
	public static SKPicture PortWood => PortWoodSvg.Value.Picture!;
	private static readonly Lazy<SKSvg> TileBrickSvg = new(() => Load("tile_brick.svg"));
	public static SKPicture TileBrick => TileBrickSvg.Value.Picture!;
	private static readonly Lazy<SKSvg> TileDesertSvg = new(() => Load("tile_desert.svg"));
	public static SKPicture TileDesert => TileDesertSvg.Value.Picture!;
	private static readonly Lazy<SKSvg> TileGrainSvg = new(() => Load("tile_grain.svg"));
	public static SKPicture TileGrain => TileGrainSvg.Value.Picture!;
	private static readonly Lazy<SKSvg> TileOreSvg = new(() => Load("tile_ore.svg"));
	public static SKPicture TileOre => TileOreSvg.Value.Picture!;
	private static readonly Lazy<SKSvg> TileSheepSvg = new(() => Load("tile_sheep.svg"));
	public static SKPicture TileSheep => TileSheepSvg.Value.Picture!;
	private static readonly Lazy<SKSvg> TileWoodSvg = new(() => Load("tile_wood.svg"));
	public static SKPicture TileWood => TileWoodSvg.Value.Picture!;
	private static readonly Lazy<SKSvg> Dice1Svg = new(() => Load("dice_1.svg"));
	public static SKPicture Dice1 => Dice1Svg.Value.Picture!;
	private static readonly Lazy<SKSvg> Dice2Svg = new(() => Load("dice_2.svg"));
	public static SKPicture Dice2 => Dice2Svg.Value.Picture!;
	private static readonly Lazy<SKSvg> Dice3Svg = new(() => Load("dice_3.svg"));
	public static SKPicture Dice3 => Dice3Svg.Value.Picture!;
	private static readonly Lazy<SKSvg> Dice4Svg = new(() => Load("dice_4.svg"));
	public static SKPicture Dice4 => Dice4Svg.Value.Picture!;
	private static readonly Lazy<SKSvg> Dice5Svg = new(() => Load("dice_5.svg"));
	public static SKPicture Dice5 => Dice5Svg.Value.Picture!;
	private static readonly Lazy<SKSvg> Dice6Svg = new(() => Load("dice_6.svg"));
	public static SKPicture Dice6 => Dice6Svg.Value.Picture!;
	private static readonly Lazy<SKSvg> Number2Svg = new(() => Load("number_2.svg"));
	public static SKPicture Number2 => Number2Svg.Value.Picture!;
	private static readonly Lazy<SKSvg> Number3Svg = new(() => Load("number_3.svg"));
	public static SKPicture Number3 => Number3Svg.Value.Picture!;
	private static readonly Lazy<SKSvg> Number4Svg = new(() => Load("number_4.svg"));
	public static SKPicture Number4 => Number4Svg.Value.Picture!;
	private static readonly Lazy<SKSvg> Number5Svg = new(() => Load("number_5.svg"));
	public static SKPicture Number5 => Number5Svg.Value.Picture!;
	private static readonly Lazy<SKSvg> Number6Svg = new(() => Load("number_6.svg"));
	public static SKPicture Number6 => Number6Svg.Value.Picture!;
	private static readonly Lazy<SKSvg> Number8Svg = new(() => Load("number_8.svg"));
	public static SKPicture Number8 => Number8Svg.Value.Picture!;
	private static readonly Lazy<SKSvg> Number9Svg = new(() => Load("number_9.svg"));
	public static SKPicture Number9 => Number9Svg.Value.Picture!;
	private static readonly Lazy<SKSvg> Number10Svg = new(() => Load("number_10.svg"));
	public static SKPicture Number10 => Number10Svg.Value.Picture!;
	private static readonly Lazy<SKSvg> Number11Svg = new(() => Load("number_11.svg"));
	public static SKPicture Number11 => Number11Svg.Value.Picture!;
	private static readonly Lazy<SKSvg> Number12Svg = new(() => Load("number_12.svg"));
	public static SKPicture Number12 => Number12Svg.Value.Picture!;

	private static SKSvg Load(string filename)
	{
		using Stream stream = typeof(Assets).Assembly.GetManifestResourceStream($"Blastic.Maui.Sample.Resources.Svg.{filename}")
			?? throw new InvalidOperationException($"Missing SVG asset: {filename}");

		SKSvg svg = new();
		
		try
		{
			if (svg.Load(stream) is null)
			{
				throw new InvalidOperationException($"Unable to load SVG asset: {filename}");
			}

			return svg;
		}
		catch
		{
			svg.Dispose();
			throw;
		}
	}
}