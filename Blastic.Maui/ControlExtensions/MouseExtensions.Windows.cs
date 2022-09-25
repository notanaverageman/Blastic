using Microsoft.Maui.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

namespace Blastic.Maui.ControlExtensions;

public static partial class MouseExtensions
{
	private static partial void UpdatePointerEnteredExitedSubscription(VisualElement visual, bool useMouseOver)
	{
		visual.HandlerChanged += (_, _) =>
		{
			if (visual.Handler?.PlatformView is not UIElement element)
			{
				return;
			}

			if (useMouseOver)
			{
				element.PointerEntered += OnPointerEntered;
				element.PointerExited += OnPointerExited;
			}
			else
			{
				element.PointerEntered -= OnPointerEntered;
				element.PointerExited -= OnPointerExited;
			}
		};

		void OnPointerEntered(object sender, PointerRoutedEventArgs e)
		{
			SetIsPointerOver(visual, true);
		}

		void OnPointerExited(object sender, PointerRoutedEventArgs e)
		{
			SetIsPointerOver(visual, false);
		}
	}
}