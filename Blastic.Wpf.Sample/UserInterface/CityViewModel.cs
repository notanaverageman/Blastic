using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using Blastic.Animations;
using Blastic.LifetimeManagement;
using Blastic.Reactive;

namespace Blastic.Wpf.Sample.UserInterface
{
	public class CityViewModel
	{
		public class ViewProperties
		{
			public IReactiveProperty<double> UnderlineWidth { get; }
			public IReactiveProperty<double> Opacity { get; }
			public IReactiveProperty<Thickness> Margin { get; }

			public ViewProperties()
			{
				UnderlineWidth = new ReactiveProperty<double>();
				Opacity = new ReactiveProperty<double>();
				Margin = new ReactiveProperty<Thickness>();
			}
		}
		
		public ILifetime Lifetime { get; }

		public IReadOnlyReactiveProperty<string> City { get; }
		public IReactiveProperty<Weather> Weather { get; }

		public IReactiveProperty<int> Temperature { get; }
		public IReadOnlyReactiveProperty<string> TemperatureText { get; }

		public ViewProperties ViewValues { get; }

		public CityViewModel(IReadOnlyReactiveProperty<string> city)
		{
			Lifetime = new Lifetime();

			City = city;
			Weather = new ReactiveProperty<Weather>();

			Temperature = new ReactiveProperty<int>();
			TemperatureText = Temperature
				.Select(x => x + "°")
				.ToReadOnlyReactiveProperty();

			Lifetime.Initialization.Subscribe(Initialize);

			ViewValues = new ViewProperties();
		}

		public void Initialize()
		{
			IObservable<double> animation1 = Animation.Create(
					TimeSpan.FromMilliseconds(400));

			IConnectableObservable<double> animation2 = Animation.Create(
					TimeSpan.FromMilliseconds(1000),
					Easing.QuadraticOut)
				.Publish();

			animation1
				.Subscribe(
					x =>
					{
						ViewValues.Opacity.Value = x;
						ViewValues.Margin.Value = new Thickness(10, (x - 1) * 10, 10, 0);
					},
					() => animation2.Connect());

			animation2.Subscribe(y => ViewValues.UnderlineWidth.Value = y);
		}
	}
}