using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Blastic.Animations;
using Blastic.Commanding;
using Blastic.LifetimeManagement;
using Blastic.Ordering;
using Blastic.Platform;
using Blastic.Reactive;
using Blastic.Wpf.UserInterface.TabbedMain;
using DynamicData;

namespace Blastic.Wpf.Sample.UserInterface
{
	public class MainViewModel : IMainTab
	{
		public class ViewProperties
		{
			public IReactiveProperty<double> Opacity { get; }
			public IReactiveProperty<double> Offset { get; }

			public ViewProperties()
			{
				Opacity = new ReactiveProperty<double>();
				Offset = new ReactiveProperty<double>();
			}
		}

		private static readonly IReadOnlyList<string> CityNames = new[]
		{
			"Singapore",
			"Beijing",
			"Bangkok",
			"Hong Kong",
			"Jakarta",
			"Kuala Lumpur",
			"Manila",
			"Osaka",
			"Pyongyang",
			"Seoul",
			"Sydney",
			"Taipei",
			"Tokyo",
		};

		private readonly Random _random;

		private readonly SourceCache<CityViewModel, int> _citiesSource;
		private readonly ReadOnlyObservableCollection<CityViewModel> _cities;

		public ILifetime Lifetime { get; }

		public Order Order { get; }
		public bool IsFixed => true;

		public IReadOnlyReactiveProperty<string> Title { get; }

		public IReactiveProperty<CityViewModel> MainCity { get; }
		public ReadOnlyObservableCollection<CityViewModel> Cities => _cities;

		public Command TestCommand { get; }

		public ViewProperties ViewValues { get; }
		
		public MainViewModel()
		{
			Lifetime = new Lifetime();

			Order = new Order(1);
			Title = new ReactiveProperty<string>("TRT World Wheather");

			MainCity = new ReactiveProperty<CityViewModel>();

			_random = new Random(1);

			_citiesSource = new SourceCache<CityViewModel, int>(x => x.GetHashCode());

			_citiesSource
				.Connect()
				.ObserveOnUI()
				.Bind(out _cities)
				.Subscribe();

			Lifetime.Initialization.Subscribe(Initialize);

			TestCommand = new Command(Initialize);

			ViewValues = new ViewProperties();
		}

		public void Initialize()
		{
			Animation
				.Create(
					TimeSpan.FromMilliseconds(2000),
					Easing.QuadraticOut)
				.Subscribe(x => ViewValues.Offset.Value = 200 * (1 - x));
			
			Animation
				.Create(
					TimeSpan.FromMilliseconds(1000),
					Easing.QuadraticIn)
				.Subscribe(x => ViewValues.Opacity.Value = x);

			(int temperature, Weather weather) = GetWeather();

			MainCity.Value = new CityViewModel(new ReactiveProperty<string>(CityNames[0]));
			MainCity.Value.Weather.Value = weather;
			MainCity.Value.Temperature.Value = temperature;

			_citiesSource.Edit(
				x =>
				{
					x.Clear();

					for (int i = 0; i < 12; i++)
					{
						(int cityTemperature, Weather cityWeather) = GetWeather();

						string name = CityNames[i + 1];
						CityViewModel city = new(new ReactiveProperty<string>(name));

						city.Weather.Value = cityWeather;
						city.Temperature.Value = cityTemperature;

						x.AddOrUpdate(city);
					}
				});

			Observable
				.Interval(TimeSpan.FromMilliseconds(50))
				.Take(_citiesSource.Count)
				.Zip(_citiesSource.Items)
				.Subscribe(async x => await x.Second.Lifetime.Initialize());
		}

		private (int Temperature, Weather weather) GetWeather()
		{
			int temperature = _random.Next(-20, 40);
			int weatherRandom = _random.Next(6);

			Weather weather = temperature switch
			{
				< 5 when weatherRandom == 0 => Weather.DayClear,
				< 5 when weatherRandom == 1 => Weather.Rainy,
				< 5 when weatherRandom == 2 => Weather.ThunderStorms,
				< 5 when weatherRandom == 3 => Weather.Flurry,
				< 5 => Weather.Snowy,
				< 20 when weatherRandom == 1 => Weather.Rainy,
				< 20 when weatherRandom == 2 => Weather.ThunderStorms,
				< 20 when weatherRandom == 3 => Weather.Foggy,
				< 20 => Weather.DayClear,
				_ => Weather.DayClear
			};

			return (temperature, weather);
		}
	}
}