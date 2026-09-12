using Blastic.Reactive;
using SkiaSharp;

namespace Blastic.Maui.Sample;

public class Player
{
	private readonly SKPicture _villagePicture;
	private readonly SKPicture _cityPicture;
	private readonly SKPicture _roadPicture;

	public int Id { get; }
	public IReactiveProperty<string> Name { get; }

	public SKImage VillageImage { get; }
	public SKImage CityImage { get; }
	public SKImage RoadImage { get; }

	public Player(int id, string name)
	{
		_villagePicture = GetVillagePicture(id);
		_cityPicture = GetCityPicture(id);
		_roadPicture = GetRoadPicture(id);

		Id = id;
		Name = new ReactiveProperty<string>(name);
		
		VillageImage = _villagePicture.CreateImage();
		CityImage = _cityPicture.CreateImage();
		RoadImage = _roadPicture.CreateImage();
	}

	public void DrawSettlement(SKCanvas canvas, SettlementType settlementType)
	{
		if (settlementType == SettlementType.Village)
		{
			canvas.DrawPictureCentered(_villagePicture);
		}
		else if (settlementType == SettlementType.City)
		{
			canvas.DrawPictureCentered(_cityPicture);
		}
	}

	public void DrawRoad(SKCanvas canvas)
	{
		canvas.DrawPictureCentered(_roadPicture);
	}

	private SKPicture GetVillagePicture(int id)
	{
		return id switch
		{
			0 => Assets.VillageRed,
			1 => Assets.VillageBlue,
			2 => Assets.VillageGreen,
			3 => Assets.VillageBlack,
			_ => throw new ArgumentOutOfRangeException(nameof(id), id, null)
		};
	}

	private SKPicture GetCityPicture(int id)
	{
		return id switch
		{
			0 => Assets.CityRed,
			1 => Assets.CityBlue,
			2 => Assets.CityGreen,
			3 => Assets.CityBlack,
			_ => throw new ArgumentOutOfRangeException(nameof(id), id, null)
		};
	}

	private SKPicture GetRoadPicture(int id)
	{
		return id switch
		{
			0 => Assets.RoadRed,
			1 => Assets.RoadBlue,
			2 => Assets.RoadGreen,
			3 => Assets.RoadBlack,
			_ => throw new ArgumentOutOfRangeException(nameof(id), id, null)
		};
	}
}