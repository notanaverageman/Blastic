using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;

namespace Blastic.Skia.Maui.Scrolling;

public partial class FixedContentScrollViewHandler : ViewHandler<FixedContentScrollView, FixedContentScrollViewUIKit>
{
	UIScrollView IScrollViewHandler.PlatformView => PlatformView;

	protected override FixedContentScrollViewUIKit CreatePlatformView()
	{
		return new FixedContentScrollViewUIKit();
	}

	protected override void ConnectHandler(FixedContentScrollViewUIKit platformView)
	{
		base.ConnectHandler(platformView);

		FixedContentScrollViewUIKit scrollView = platformView;
		scrollView.Handler = this;
	}
	
	public override void SetVirtualView(IView view)
	{
		base.SetVirtualView(view);

		FixedContentScrollViewUIKit scrollView = PlatformView;
		scrollView.Initialize();
	}

	private static void MapContentSize(FixedContentScrollViewHandler handler, FixedContentScrollView view)
	{
		FixedContentScrollViewUIKit scrollView = handler.PlatformView;
		scrollView.UpdateContentSize();
	}

	private static void MapMinimumZoomScale(FixedContentScrollViewHandler handler, FixedContentScrollView view)
	{
		FixedContentScrollViewUIKit scrollView = handler.PlatformView;
		scrollView.UpdateMinimumZoomScale();
	}

	private static void MapMaximumZoomScale(FixedContentScrollViewHandler handler, FixedContentScrollView view)
	{
		FixedContentScrollViewUIKit scrollView = handler.PlatformView;
		scrollView.UpdateMaximumZoomScale();
	}

	private static void MapRequestedZoomScale(FixedContentScrollViewHandler handler, FixedContentScrollView view)
	{
		FixedContentScrollViewUIKit scrollView = handler.PlatformView;
		scrollView.UpdateZoomScale();
	}

	public static void MapContent(IScrollViewHandler handler, IScrollView scrollView)
	{
		if (handler.PlatformView == null || handler.MauiContext == null || scrollView.PresentedContent == null)
		{
			return;
		}
		
		UIScrollView platformScrollView = handler.PlatformView;
		UIView nativeContent = scrollView.PresentedContent.ToPlatform(handler.MauiContext);

		platformScrollView.ClearSubviews();
		platformScrollView.AddSubview(nativeContent);
	}

	public static void MapIsEnabled(IScrollViewHandler handler, IScrollView scrollView)
	{
		handler.PlatformView.UpdateIsEnabled(scrollView);
	}

	public static void MapHorizontalScrollBarVisibility(IScrollViewHandler handler, IScrollView scrollView)
	{
		handler.PlatformView.UpdateHorizontalScrollBarVisibility(scrollView.HorizontalScrollBarVisibility);
	}

	public static void MapVerticalScrollBarVisibility(IScrollViewHandler handler, IScrollView scrollView)
	{
		handler.PlatformView.UpdateVerticalScrollBarVisibility(scrollView.VerticalScrollBarVisibility);
	}

	public static void MapOrientation(IScrollViewHandler handler, IScrollView scrollView)
	{
		// Nothing to do here for now, but we might need to make adjustments for FlowDirection when the orientation is set to Horizontal
	}

	public static void MapRequestScrollTo(IScrollViewHandler handler, IScrollView scrollView, object? args)
	{
		if (args is ScrollToRequest request)
		{
			handler.PlatformView.SetContentOffset(new CoreGraphics.CGPoint(request.HorizontalOffset, request.VerticalOffset), !request.Instant);

			if (request.Instant)
			{
				scrollView.ScrollFinished();
			}
		}
	}
}