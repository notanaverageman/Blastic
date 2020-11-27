using Xamarin.Forms;

namespace Blastic.Forms.Sample.Controls
{
	public class ExtendedTabbedPage : TabbedPage
	{
		public static readonly BindableProperty TabBarOffsetProperty = BindableProperty.Create(
			nameof(TabBarOffsetProperty).Replace("Property", ""),
			typeof(float),
			typeof(ExtendedTabbedPage),
			0f);
		public float TabBarOffset
		{
			get => (float)GetValue(TabBarOffsetProperty);
			set => SetValue(TabBarOffsetProperty, value);
		}

		public static readonly BindableProperty ContainerMarginProperty = BindableProperty.Create(
			nameof(ContainerMarginProperty).Replace("Property", ""),
			typeof(float),
			typeof(ExtendedTabbedPage),
			0f);
		public float ContainerMargin
		{
			get => (float)GetValue(ContainerMarginProperty);
			set => SetValue(ContainerMarginProperty, value);
		}

		public static readonly BindableProperty TabBarHeightProperty = BindableProperty.Create(
			nameof(TabBarHeightProperty).Replace("Property", ""),
			typeof(float),
			typeof(ExtendedTabbedPage),
			0f,
			BindingMode.OneWayToSource);
		public float TabBarHeight
		{
			get => (float)GetValue(TabBarHeightProperty);
			set => SetValue(TabBarHeightProperty, value);
		}
	}
}