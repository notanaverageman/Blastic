using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Blastic.Commanding;
using Blastic.LifetimeManagement;
using Blastic.Ordering;
using Blastic.Reactive;
using Blastic.Wpf.Commanding;

namespace Blastic.Wpf.Sample.UserInterface
{
	public partial class MainViewModel : IHasLifetime
	{
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

		public ILifetime Lifetime { get; }

		public Order Order { get; }
		public bool IsFixed => true;

		public IReadOnlyReactiveProperty<string> Title { get; }

		public IReactiveProperty<CityViewModel> MainCity { get; }
		public ObservableCollection<CityViewModel> Cities { get; }

		public IReactiveProperty<CityImage> CityImageSource1 { get; }
		public IReactiveProperty<CityImage> CityImageSource2 { get; }

		public Command CycleCommand { get; }
		public Command ActivateCommand { get; }
		public Command DeactivateCommand { get; }

		public ViewAnimations Animations { get; }

		public MainViewModel()
		{
			Lifetime = new Lifetime();

			Order = new Order(1);
			Title = new ReactiveProperty<string>("TRT World Wheather");

			MainCity = new ReactiveProperty<CityViewModel>();

			_random = new Random(1);

			Cities = new ObservableCollection<CityViewModel>();

			CityImageSource1 = new ReactiveProperty<CityImage>(CityImage.Singapore);
			CityImageSource2 = new ReactiveProperty<CityImage>(CityImage.Dubai);

			Animations = new ViewAnimations(Cities);

			PopulateCities();

			CycleCommand = new Command(Cycle);
			ActivateCommand = new Command(Activate);
			DeactivateCommand = new Command(Deactivate);

			Lifetime.Initialization.Subscribe(() => CycleCommand.AddInputGesture(new KeyGesture(Key.Left)));
			Lifetime.Activation.Subscribe(Activate);
		}

		public async Task Cycle()
		{
			Animations.Disappear();
			await Task.Delay(TimeSpan.FromMilliseconds(1200));
			Animations.Appear();
		}

		public void Activate()
		{
			Animations.Appear();
		}

		public void Deactivate()
		{
			Animations.Disappear();
		}

		private void PopulateCities()
		{
			(int temperature, Weather weather) = GetWeather();

			MainCity.Value = new CityViewModel(new ReactiveProperty<string>(CityNames[0]));
			MainCity.Value.Weather.Value = weather;
			MainCity.Value.Temperature.Value = temperature;

			Cities.Clear();

			for (int i = 0; i < 12; i++)
			{
				(int cityTemperature, Weather cityWeather) = GetWeather();

				string name = CityNames[i + 1];
				CityViewModel city = new(new ReactiveProperty<string>(name));

				city.Weather.Value = cityWeather;
				city.Temperature.Value = cityTemperature;

				Cities.Add(city);
			}
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