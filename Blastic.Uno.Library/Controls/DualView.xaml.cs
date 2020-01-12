using Windows.UI.Xaml;

namespace Blastic.Controls
{
	public partial class DualView
	{
		public static readonly DependencyProperty ShowFirstViewProperty = DependencyProperty.Register(
			nameof(ShowFirstViewProperty).Replace("Property", ""),
			typeof(bool),
			typeof(DualView),
			new PropertyMetadata(true));
		public bool ShowFirstView
		{
			get => (bool)GetValue(ShowFirstViewProperty);
			set => SetValue(ShowFirstViewProperty, value);
		}

		public static readonly DependencyProperty FirstViewProperty = DependencyProperty.Register(
			nameof(FirstViewProperty).Replace("Property", ""),
			typeof(UIElement),
			typeof(DualView),
			new PropertyMetadata(default(UIElement)));
		public UIElement FirstView
		{
			get => (UIElement)GetValue(FirstViewProperty);
			set => SetValue(FirstViewProperty, value);
		}

		public static readonly DependencyProperty SecondViewProperty = DependencyProperty.Register(
			nameof(SecondViewProperty).Replace("Property", ""),
			typeof(UIElement),
			typeof(DualView),
			new PropertyMetadata(default(UIElement)));
		public UIElement SecondView
		{
			get => (UIElement)GetValue(SecondViewProperty);
			set => SetValue(SecondViewProperty, value);
		}

		public DualView()
		{
			InitializeComponent();
		}
	}
}