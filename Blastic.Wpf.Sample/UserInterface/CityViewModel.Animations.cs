using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Blastic.Animations;
using Blastic.Reactive;

namespace Blastic.Wpf.Sample.UserInterface
{
	public partial class CityViewModel
	{
		public class ViewAnimations
		{
			public IReactiveProperty<double> UnderlineWidth { get; }
			public IReactiveProperty<double> Opacity { get; }
			public IReactiveProperty<double> Offset { get; }

			public double OffsetMax { get; set; }
			public TimeSpan OffsetAnimationDuration { get; set; }
			public TimeSpan UnderlineAnimationDuration { get; set; }

			public ViewAnimations()
			{
				UnderlineWidth = new ReactiveProperty<double>(0);
				Opacity = new ReactiveProperty<double>(0);
				Offset = new ReactiveProperty<double>(0);

				OffsetMax = 10;
				OffsetAnimationDuration = TimeSpan.FromMilliseconds(400);
				UnderlineAnimationDuration = TimeSpan.FromMilliseconds(1000);
			}

			public void Appear()
			{
				UnderlineWidth.Value = 0;
				
				IObservable<double> animation1 = Animation.Create(OffsetAnimationDuration);

				IConnectableObservable<double> animation2 = Animation
					.Create(OffsetAnimationDuration, Easing.QuadraticOut)
					.Publish();

				animation1
					.Subscribe(
						x =>
						{
							Opacity.Value = x;
							Offset.Value = (x - 1) * OffsetMax;
						},
						() => animation2.Connect());

				animation2.Subscribe(x => UnderlineWidth.Value = x);
			}

			public void Disappear()
			{
				Animation
					.Create(OffsetAnimationDuration)
					.Subscribe(x =>
					{
						Opacity.Value = 1 - x;
						Offset.Value = x * OffsetMax;
					});
			}
		}
	}
}