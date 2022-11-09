using Windows.System;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using ScrollMode = Microsoft.UI.Xaml.Controls.ScrollMode;
using Size = Windows.Foundation.Size;
using Window = Microsoft.UI.Xaml.Window;

namespace Blastic.Skia.Maui.Scrolling;

public partial class FixedContentScrollViewHandler : ScrollViewHandler
{
	private bool _isShiftPressed;

	protected override void ConnectHandler(ScrollViewer platformView)
	{
		base.ConnectHandler(platformView);
		
		if (VirtualView is not FixedContentScrollView view)
		{
			return;
		}
		
		if (view.Window?.Handler.PlatformView is not Window window)
		{
			return;
		}

		// https://stackoverflow.com/q/53772739/3670437
		window.Content.PreviewKeyDown += (_, args) =>
		{
			if (args.Key != VirtualKey.Shift)
			{
				return;
			}

			if (_isShiftPressed)
			{
				return;
			}

			_isShiftPressed = true;
			platformView.VerticalScrollMode = ScrollMode.Disabled;
			platformView.IsScrollInertiaEnabled = false;
		};

		window.Content.PreviewKeyUp += (_, args) =>
		{
			if (args.Key != VirtualKey.Shift)
			{
				return;
			}

			_isShiftPressed = false;
			platformView.VerticalScrollMode = ScrollMode.Enabled;
			platformView.IsScrollInertiaEnabled = true;
		};
	}

	public static void MapContent(FixedContentScrollViewHandler handler, FixedContentScrollView view)
	{
		IScrollView scrollView = view;

		if (scrollView.PresentedContent == null || handler.MauiContext == null)
		{
			return;
		}

		ScrollViewer scrollViewer = handler.PlatformView;
		FrameworkElement nativeContent = scrollView.PresentedContent.ToPlatform(handler.MauiContext);

		Panel? container = GetContainer(scrollViewer);
		
		if (container != null)
		{
			if (container.Children.Count == 0 || container.Children[0] != nativeContent)
			{
				container.Children.Clear();
				container.Children.Add(nativeContent);
			}
		}
		else
		{
			InsertContainer(scrollViewer, scrollView, nativeContent);
		}
		
		scrollViewer.ZoomMode = ZoomMode.Enabled;

		scrollViewer.ViewChanged += (_, _) =>
		{
			double scrollX = scrollViewer.HorizontalOffset;
			double scrollY = scrollViewer.VerticalOffset;
			float zoomScale = scrollViewer.ZoomFactor;
			
			view.SetScrollAndZoom(scrollX, scrollY, zoomScale);
		};
		
		scrollViewer.Loaded += (_, _) =>
		{
			FixContentPosition(scrollViewer);
		};
	}

	private static Panel? GetContainer(ScrollViewer scrollViewer)
	{
		return scrollViewer.Content as Panel;
	}

	private class ContentPanel : Canvas
	{
		public double ContentWidth { get; set; }
		public double ContentHeight { get; set; }

		public ContentPanel()
		{
			VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Top;
			HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center;
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			return new Size(ContentWidth, ContentHeight);
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			return new Size(ContentWidth, ContentHeight);
		}
	}

	private static void InsertContainer(ScrollViewer scrollViewer, IScrollView scrollView, FrameworkElement nativeContent)
	{
		if (scrollView.PresentedContent == null)
		{
			return;
		}
		
		ContentPanel container = new();

		scrollViewer.Content = null;
		container.Children.Add(nativeContent);
		scrollViewer.Content = container;
	}
	
	private static async void MapRequestScrollTo(IViewHandler handler, IView view, object? args)
	{
		if (args is ScrollToRequest request && handler is IScrollViewHandler scrollViewHandler)
		{
			await Task.Delay(1);
			scrollViewHandler.PlatformView.ChangeView(request.HorizontalOffset, request.VerticalOffset, null, request.Instant);
		}
	}

	private static void MapContentSize(FixedContentScrollViewHandler handler, FixedContentScrollView view)
	{
		ScrollViewer scrollViewer = handler.PlatformView;
		ContentPanel? container = GetContainer(scrollViewer) as ContentPanel;

		if (container == null)
		{
			return;
		}

		if (view.ContentWidth > 0)
		{
			container.ContentWidth = view.ContentWidth / scrollViewer.ZoomFactor;
		}

		if (view.ContentHeight > 0)
		{
			container.ContentHeight = view.ContentHeight / scrollViewer.ZoomFactor;
		}

		container.InvalidateMeasure();
	}

	private static void MapMinimumZoomScale(FixedContentScrollViewHandler handler, FixedContentScrollView view)
	{
		handler.PlatformView.MinZoomFactor = view.MinimumZoomScale;
	}

	private static void MapMaximumZoomScale(FixedContentScrollViewHandler handler, FixedContentScrollView view)
	{
		handler.PlatformView.MaxZoomFactor = view.MaximumZoomScale;
	}

	private static void MapRequestedZoomScale(FixedContentScrollViewHandler handler, FixedContentScrollView view)
	{
		handler.PlatformView.ZoomToFactor(view.RequestedZoomScale);
	}
	
	private static void FixContentPosition(ScrollViewer scrollViewer)
	{
		Panel? container = GetContainer(scrollViewer);

		if (container == null)
		{
			return;
		}

		UIElement target = container.Children[0];

		CompositionPropertySet propertySet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollViewer);
		
		ExpressionAnimation scrollAnimation = propertySet.Compositor.CreateExpressionAnimation("-scrollViewer.Translation / scrollViewer.Scale");
		ExpressionAnimation zoomAnimation = propertySet.Compositor.CreateExpressionAnimation("Vector3(1, 1, 1) / scrollViewer.Scale");

		scrollAnimation.SetReferenceParameter("scrollViewer", propertySet);
		zoomAnimation.SetReferenceParameter("scrollViewer", propertySet);

		Visual visual = ElementCompositionPreview.GetElementVisual(target);

		ElementCompositionPreview.SetIsTranslationEnabled(target, true);
		visual.StartAnimation("Translation", scrollAnimation);
		visual.StartAnimation("Scale", zoomAnimation);
	}
}