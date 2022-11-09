using System.Runtime.InteropServices;
using Blastic.Skia.Input;
using Blastic.Skia.Maui.GestureRecongnizers;
using Foundation;
using UIKit;
using SKCanvasView = SkiaSharp.Views.iOS.SKCanvasView;

namespace Blastic.Skia.Maui.TouchCanvas;

public partial class TouchCanvasViewHandler
{
	private UITapGestureRecognizer? _touchTapRecognizer;

	private UITapGestureRecognizer? _mouseTapRecognizer;
	private UIHoverGestureRecognizer? _mouseMoveRecognizer;

	private UITapGestureRecognizer? _stylusTapRecognizer;
	private ForcePanGestureRecognizer? _stylusPanRecognizer;

	protected override SKCanvasView CreatePlatformView()
	{
		return new SKCanvasView();
	}

	protected override void ConnectHandler(SKCanvasView platformView)
	{
		_touchTapRecognizer = new UITapGestureRecognizer(x => HandleTap(x, InputSource.Touch))
		{
			AllowedTouchTypes = new NSNumber[]
			{
				(int)UITouchType.Direct
			}
		};

		_mouseTapRecognizer = new UITapGestureRecognizer(x => HandleTap(x, InputSource.Mouse))
		{
			AllowedTouchTypes = new NSNumber[]
			{
				(int)UITouchType.IndirectPointer
			}
		};

		_mouseMoveRecognizer = new UIHoverGestureRecognizer(HandleHover)
		{
			AllowedTouchTypes = new NSNumber[]
			{
				(int)UITouchType.IndirectPointer
			}
		};

		_stylusTapRecognizer = new UITapGestureRecognizer(x => HandleTap(x, InputSource.Stylus))
		{
			AllowedTouchTypes = new NSNumber[]
			{
				(int)UITouchType.Stylus
			}
		};

		_stylusPanRecognizer = new ForcePanGestureRecognizer(x => HandlePan(x, InputSource.Stylus))
		{
			AllowedTouchTypes = new NSNumber[]
			{
				(int)UITouchType.Stylus
			}
		};

		base.ConnectHandler(platformView);
	}

	private void HandleTap(UITapGestureRecognizer recognizer, InputSource source)
	{
		if (VirtualView is not TouchCanvasView view)
		{
			return;
		}

		(NFloat x, NFloat y) = recognizer.LocationInView(PlatformView);

		view.SendTap(new Point(x, y), source);
	}

	private void HandlePan(UIPanGestureRecognizer recognizer, InputSource source)
	{
		if (VirtualView is not TouchCanvasView view)
		{
			return;
		}

		(NFloat x, NFloat y) = recognizer.LocationInView(PlatformView);
		double force = _stylusPanRecognizer?.Force ?? 0;

		view.SendPan(new Point(x, y), source, force);
	}

	private void HandleHover()
	{
		if (VirtualView is not TouchCanvasView view || _mouseMoveRecognizer == null)
		{
			return;
		}

		if (_mouseMoveRecognizer.State is not (UIGestureRecognizerState.Began or UIGestureRecognizerState.Changed))
		{
			return;
		}

		(NFloat x, NFloat y) = _mouseMoveRecognizer.LocationInView(PlatformView);

		view.SendPointerMove(new Point(x, y));
	}

	public static void MapEnableTouch(TouchCanvasViewHandler handler, TouchCanvasView view)
	{
		handler.HandleEnableTouch();
	}

	private void HandleEnableTouch()
	{
		if (VirtualView is not TouchCanvasView view)
		{
			return;
		}

		if (view.EnableTouch)
		{
			EnableGestureRecognizer(_touchTapRecognizer);

			EnableGestureRecognizer(_mouseTapRecognizer);
			EnableGestureRecognizer(_mouseMoveRecognizer);

			EnableGestureRecognizer(_stylusTapRecognizer);
			EnableGestureRecognizer(_stylusPanRecognizer);
		}
		else
		{
			DisableGestureRecognizer(_touchTapRecognizer);

			DisableGestureRecognizer(_mouseTapRecognizer);
			DisableGestureRecognizer(_mouseMoveRecognizer);

			DisableGestureRecognizer(_stylusTapRecognizer);
			DisableGestureRecognizer(_stylusPanRecognizer);
		}

		PlatformView.UserInteractionEnabled = PlatformView.GestureRecognizers != null;
	}

	private void EnableGestureRecognizer(UIGestureRecognizer? gestureRecognizer)
	{
		if (gestureRecognizer == null)
		{
			return;
		}

		if (PlatformView.GestureRecognizers?.Contains(gestureRecognizer) != true)
		{
			PlatformView.AddGestureRecognizer(gestureRecognizer);
		}
	}

	private void DisableGestureRecognizer(UIGestureRecognizer? gestureRecognizer)
	{
		if (gestureRecognizer == null)
		{
			return;
		}

		if (PlatformView.GestureRecognizers?.Contains(gestureRecognizer) == true)
		{
			PlatformView.RemoveGestureRecognizer(gestureRecognizer);
		}
	}
}