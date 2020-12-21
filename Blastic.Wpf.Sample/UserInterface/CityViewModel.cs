using System.Reactive.Linq;
using Blastic.LifetimeManagement;
using Blastic.Reactive;

namespace Blastic.Wpf.Sample.UserInterface
{
	public partial class CityViewModel
	{
		public ILifetime Lifetime { get; }

		public IReadOnlyReactiveProperty<string> City { get; }
		public IReactiveProperty<Weather> Weather { get; }

		public IReactiveProperty<int> Temperature { get; }
		public IReadOnlyReactiveProperty<string> TemperatureText { get; }

		public ViewAnimations Animations { get; }

		public CityViewModel(IReadOnlyReactiveProperty<string> city)
		{
			Lifetime = new Lifetime();

			City = city;
			Weather = new ReactiveProperty<Weather>();

			Temperature = new ReactiveProperty<int>();
			TemperatureText = Temperature
				.Select(x => x + "°")
				.ToReadOnlyReactiveProperty();

			Animations = new ViewAnimations();
			
			Lifetime.Activation.Subscribe(Appear);
			Lifetime.Deactivation.Subscribe(Disappear);
		}

		public void Appear()
		{
			Animations.Appear();
		}

		public void Disappear()
		{
			Animations.Disappear();
		}
	}
}