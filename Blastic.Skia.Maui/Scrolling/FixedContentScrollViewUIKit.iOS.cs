using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Blastic.Skia.Maui.Scrolling;

public class FixedContentScrollViewUIKit : UIScrollView
{
	private readonly UIView _dummyView;
	private UIView? _viewToFixPosition;
	private bool _zoomBounceAnimationStarted;

	public FixedContentScrollViewHandler? Handler { get; set; }

	public FixedContentScrollViewUIKit()
	{
		// UIScrollView will only update this invisible dummy view. The actual view should not be modified
		// by the scroll view. It will be modified according to the dummy view's values.
		_dummyView = new UIView();
		ViewForZoomingInScrollView += GetViewForZooming;

		Scrolled += HandleScroll;
	}

	public override void LayoutSubviews()
	{
		base.LayoutSubviews();
		UpdateContentSize();

		if (_viewToFixPosition == null)
		{
			return;
		}

		// View's frame is reset when view's size changes during zooming.
		_viewToFixPosition.Frame = new CGRect(Layer.Bounds.Location, _viewToFixPosition.Frame.Size);
	}

	private void HandleScroll(object? sender, EventArgs args)
	{
		LayoutIfNeeded();
		HandleZoomBouncingAnimation();
	}

	public override void LayoutIfNeeded()
	{
		base.LayoutIfNeeded();

		if (_viewToFixPosition == null)
		{
			return;
		}

		CGAffineTransform transform = _dummyView.Transform;
		CGRect layerBounds = Layer.Bounds;
		CGRect viewFrame = _viewToFixPosition.Frame;
		
		// Using dummy view's bounds ensures that the actual view is always fixed at the top left
		// corner of the screen.
		_viewToFixPosition.Frame = new CGRect(layerBounds.Location, viewFrame.Size);
		
		// Update the scroll and zoom scale values only if there is no zoom bouncing animation. Dummy view's bounds
		// will be set to the target value just before the animation and we don't want to jump there immediately.
		if (Layer.AnimationKeys == null)
		{
			FixedContentScrollView? scrollView = Handler?.VirtualView;

			scrollView?.SetScrollAndZoom(layerBounds.X, layerBounds.Y, transform.A);
		}
	}

	private void HandleZoomBouncingAnimation()
	{
		// Zoom bouncing is an animation and LayoutIfNeeded is not called during this animation.
		// So run a function on NSRunLoop.Main that will send the actual scroll and zoom scale values.

		if (_zoomBounceAnimationStarted)
		{
			return;
		}

		// Handle only if there is an animation, which is the zoom bouncing.
		if (Layer.AnimationKeys == null)
		{
			return;
		}

		if (_viewToFixPosition == null)
		{
			return;
		}

		FixedContentScrollView? scrollView = Handler?.VirtualView;

		CADisplayLink? displayLink = null;
		displayLink = CADisplayLink.Create(() =>
		{
			// Check if the animation is finished. If so this function will be unregistered from run loop.
			if (Layer.AnimationKeys != null)
			{
				scrollView?.SetScrollAndZoom(
					Layer.PresentationLayer!.Bounds.X,
					Layer.PresentationLayer.Bounds.Y,
					_dummyView.Layer.PresentationLayer!.Transform.M11);
			}
			else
			{
				_zoomBounceAnimationStarted = false;
				// ReSharper disable once AccessToModifiedClosure
				// We actually want to a reference to the variable itself. A copy will always be null.
				displayLink!.RemoveFromRunLoop(NSRunLoop.Main, NSRunLoopMode.Common);
			}
		});

		_zoomBounceAnimationStarted = true;
		// Use NSRunLoopMode.Common since Default will not run the function while there is touch input.
		displayLink.AddToRunLoop(NSRunLoop.Main, NSRunLoopMode.Common);
	}

	internal void Initialize()
	{
		if (!Subviews.Contains(_dummyView))
		{
			_viewToFixPosition = GetViewToFixPosition();

			AddSubview(_dummyView);

			if (_viewToFixPosition != null)
			{
				BringSubviewToFront(_viewToFixPosition);
			}
		}

		UpdateContentSize();
		UpdateMinimumZoomScale();
		UpdateMaximumZoomScale();
		UpdateZoomScale();
	}
	
	internal void UpdateContentSize()
	{
		FixedContentScrollView? virtualView = Handler?.VirtualView;

		if (virtualView == null)
		{
			return;
		}

		_dummyView.Frame = new CGRect(0, 0, virtualView.ContentWidth, virtualView.ContentHeight);
		ContentSize = new CGSize(virtualView.ContentWidth, virtualView.ContentHeight);
	}

	internal void UpdateMinimumZoomScale()
	{
		FixedContentScrollView? virtualView = Handler?.VirtualView;

		if (virtualView == null)
		{
			return;
		}

		MinimumZoomScale = virtualView.MinimumZoomScale;
	}

	internal void UpdateMaximumZoomScale()
	{
		FixedContentScrollView? virtualView = Handler?.VirtualView;

		if (virtualView == null)
		{
			return;
		}

		MaximumZoomScale = virtualView.MaximumZoomScale;
	}

	internal void UpdateZoomScale()
	{
		FixedContentScrollView? virtualView = Handler?.VirtualView;

		if (virtualView == null)
		{
			return;
		}

		ZoomScale = virtualView.RequestedZoomScale;
	}

	private UIView? GetViewToFixPosition()
	{
		return Subviews.FirstOrDefault();
	}

	public UIView GetViewForZooming(UIScrollView sv)
	{
		return _dummyView;
	}
}