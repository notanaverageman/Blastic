using System.Linq;
using Microsoft.Maui.Controls;
using UIKit;

namespace Blastic.Maui.ControlExtensions;

public static partial class MouseExtensions
{
	private static partial void UpdatePointerEnteredExitedSubscription(VisualElement visual, bool useMouseOver)
	{
		visual.HandlerChanged += (_, _) =>
		{
			if (visual.Handler?.PlatformView is not UIView view)
			{
				return;
			}

			if (useMouseOver)
			{
				// TODO: https://github.com/xamarin/xamarin-macios/issues/15335
				UIHoverGestureRecognizer? gestureRecognizer = null;

				gestureRecognizer = new UIHoverGestureRecognizer(() =>
				{
					SetIsPointerOver(
						visual,
						gestureRecognizer?.State is UIGestureRecognizerState.Began or UIGestureRecognizerState.Changed);
				});

				view.AddGestureRecognizer(gestureRecognizer);
			}
			else
			{
				UIHoverGestureRecognizer? gestureRecognizer = view.GestureRecognizers
					?.OfType<UIHoverGestureRecognizer>()
					.FirstOrDefault();

				if (gestureRecognizer != null)
				{
					view.RemoveGestureRecognizer(gestureRecognizer);
				}
			}
		};
	}
}