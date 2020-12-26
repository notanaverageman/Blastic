using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Blastic.Animations;
using Blastic.Reactive;

namespace Blastic.Wpf.Sample.UserInterface
{
	public partial class MainViewModel
	{
		public class ViewAnimations
		{
			private readonly IReadOnlyCollection<CityViewModel> _cities;
			private bool _isCityImage1Active;

			public IReactiveProperty<double> Opacity { get; }
			public IReactiveProperty<double> CityTextOffset { get; }
			public IReactiveProperty<double> TemperatureOffset { get; }
			public IReactiveProperty<double> WeatherOffset { get; }
			public IReactiveProperty<double> CityImage1Offset { get; }
			public IReactiveProperty<double> CityImage2Offset { get; }

			public double CityTextAppearOffsetMax { get; set; }
			public double CityTextDisappearOffsetMax { get; set; }
			public double TemperatureAppearOffsetMax { get; set; }
			public double TemperatureDisappearOffsetMax { get; set; }
			public double WeatherAppearOffsetMax { get; set; }
			public double WeatherDisappearOffsetMax { get; set; }

			public TimeSpan AppearAnimationDuration { get; set; }
			public TimeSpan DisappearAnimationDuration { get; set; }
			public TimeSpan DisappearImageAnimationDuration { get; set; }

			public ViewAnimations(IReadOnlyCollection<CityViewModel> cities)
			{
				_cities = cities;
				_isCityImage1Active = true;

				Opacity = new ReactiveProperty<double>();
				CityTextOffset = new ReactiveProperty<double>();
				TemperatureOffset = new ReactiveProperty<double>();
				WeatherOffset = new ReactiveProperty<double>();
				CityImage1Offset = new ReactiveProperty<double>();
				CityImage2Offset = new ReactiveProperty<double>();

				CityTextAppearOffsetMax = 50;
				CityTextDisappearOffsetMax = 300;
				TemperatureAppearOffsetMax = 100;
				TemperatureDisappearOffsetMax = 200;
				WeatherAppearOffsetMax = 100;
				WeatherDisappearOffsetMax = 250;

				AppearAnimationDuration = TimeSpan.FromMilliseconds(600);
				DisappearAnimationDuration = TimeSpan.FromMilliseconds(600);
				DisappearImageAnimationDuration = TimeSpan.FromMilliseconds(1000);
			}

			public void Appear()
			{
				Animation
					.Create(AppearAnimationDuration, Easing.QuadraticOut)
					.Subscribe(x =>
					{
						Opacity.Value = x;
						CityTextOffset.Value = CityTextAppearOffsetMax * (1 - x);
						TemperatureOffset.Value = TemperatureAppearOffsetMax * (1 - x);
						WeatherOffset.Value = WeatherAppearOffsetMax * (1 - x);
					});

				Observable
					.Interval(TimeSpan.FromMilliseconds(50))
					.Take(_cities.Count)
					.Zip(_cities)
					.Subscribe(x => x.Second.Appear());
			}

			public void Disappear()
			{
				Observable
					.Interval(TimeSpan.FromMilliseconds(50))
					.Take(_cities.Count)
					.Zip(_cities)
					.Subscribe(x => x.Second.Disappear());

				IConnectableObservable<double> imageAnimation = Animation
					.Create(DisappearImageAnimationDuration, Easing.QuinticOut)
					.Publish();

				imageAnimation
					.Subscribe(
						x =>
						{
							if (_isCityImage1Active)
							{
								CityImage1Offset.Value = -x;
								CityImage2Offset.Value = 1 - x;
							}
							else
							{
								CityImage2Offset.Value = -x;
								CityImage1Offset.Value = 1 - x;
							}
						},
						() => _isCityImage1Active = !_isCityImage1Active);

				Animation
					.Create(DisappearAnimationDuration, Easing.QuadraticIn)
					.Delay(TimeSpan.FromMilliseconds(300))
					.Subscribe(
						x =>
						{
							Opacity.Value = 1 - x;
							CityTextOffset.Value = -x * CityTextDisappearOffsetMax;
							TemperatureOffset.Value = -x * TemperatureDisappearOffsetMax;
							WeatherOffset.Value = -x * WeatherDisappearOffsetMax;
						},
						() => imageAnimation.Connect());
			}
		}
	}
}