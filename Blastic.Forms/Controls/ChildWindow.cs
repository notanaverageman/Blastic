using Xamarin.Forms;

namespace Blastic.Forms.Controls
{
	public class ChildWindow : ContentView
	{
		public static readonly BindableProperty IsOpenProperty = BindableProperty.Create(
			nameof(IsOpen),
			typeof(bool),
			typeof(ChildWindow),
			default(bool),
			propertyChanged: OnIsOpenChanged);
		public bool IsOpen
		{
			get => (bool)GetValue(IsOpenProperty);
			set => SetValue(IsOpenProperty, value);
		}

		public static readonly BindableProperty OverlayColorProperty = BindableProperty.Create(
			nameof(OverlayColor).Replace("Property", ""),
			typeof(Color),
			typeof(ChildWindow),
			Color.Transparent);
		public Color OverlayColor
		{
			get => (Color)GetValue(OverlayColorProperty);
			set => SetValue(OverlayColorProperty, value);
		}

		private static void OnIsOpenChanged(BindableObject bindable, object oldValue, object newValue)
		{
			ChildWindow childWindow = (ChildWindow)bindable;

			if ((bool)newValue)
			{
				childWindow.Focus();
			}

			VisualStateManager.GoToState(childWindow, (bool)newValue == false ? "Hide" : "Show");
		}
	}
}