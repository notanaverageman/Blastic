using Microsoft.Maui.Handlers;

namespace Blastic.Skia.Maui.Scrolling;

public partial class FixedContentScrollViewHandler : IScrollViewHandler
{
	public static readonly PropertyMapper<FixedContentScrollView, FixedContentScrollViewHandler> FixedContentScrollViewMapper = new(ViewMapper)
	{
		[nameof(IScrollView.Content)] = MapContent,
		[nameof(IScrollView.HorizontalScrollBarVisibility)] = MapHorizontalScrollBarVisibility,
		[nameof(IScrollView.VerticalScrollBarVisibility)] = MapVerticalScrollBarVisibility,
		[nameof(IScrollView.Orientation)] = MapOrientation,

		[nameof(FixedContentScrollView.ContentWidth)] = MapContentSize,
		[nameof(FixedContentScrollView.ContentHeight)] = MapContentSize,
		[nameof(FixedContentScrollView.MinimumZoomScale)] = MapMinimumZoomScale,
		[nameof(FixedContentScrollView.MaximumZoomScale)] = MapMaximumZoomScale,
		[nameof(FixedContentScrollView.RequestedZoomScale)] = MapRequestedZoomScale,
#if __IOS__
		[nameof(IScrollView.ContentSize)] = MapContentSize,
		[nameof(IScrollView.IsEnabled)] = MapIsEnabled,
#endif
	};
	
	IScrollView IScrollViewHandler.VirtualView => VirtualView;
	
	public FixedContentScrollViewHandler() : base(FixedContentScrollViewMapper)
	{
#if WINDOWS
		ViewCommandMapper[nameof(IScrollView.RequestScrollTo)] = MapRequestScrollTo;
#endif
	}

	public FixedContentScrollViewHandler(PropertyMapper? mapper = null) : base(mapper ?? FixedContentScrollViewMapper)
	{
	}
}