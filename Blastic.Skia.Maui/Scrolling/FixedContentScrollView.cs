using Bindables.Maui;

namespace Blastic.Skia.Maui.Scrolling;

public partial class FixedContentScrollView : ScrollView
{
	private const float MinimumZoomScaleDefaultValue = 1;
	private const float MaximumZoomScaleDefaultValue = 1;
	private const float RequestedZoomScaleDefaultValue = 1;

	public event EventHandler<ViewportChangedEventArgs>? ViewportChanged;

	[BindableProperty(typeof(float))]
	public static readonly BindableProperty ContentWidthProperty;
	
	[BindableProperty(typeof(float))]
	public static readonly BindableProperty ContentHeightProperty;

	[BindableProperty(typeof(float), DefaultValueField = nameof(MinimumZoomScaleDefaultValue))]
	public static readonly BindableProperty MinimumZoomScaleProperty;

	[BindableProperty(typeof(float), DefaultValueField = nameof(MaximumZoomScaleDefaultValue))]
	public static readonly BindableProperty MaximumZoomScaleProperty;

	[BindableProperty(typeof(float), DefaultValueField = nameof(RequestedZoomScaleDefaultValue))]
	public static readonly BindableProperty RequestedZoomScaleProperty;

	public void SetScrollAndZoom(double scrollX, double scrollY, double zoomScale)
	{
		ViewportChanged?.Invoke(this, new ViewportChangedEventArgs(scrollX, scrollY, zoomScale));
	}
}