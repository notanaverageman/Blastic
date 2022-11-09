using Foundation;
using UIKit;

namespace Blastic.Skia.Maui.GestureRecongnizers;

public class ForcePanGestureRecognizer : UIPanGestureRecognizer
{
	public double Force { get; private set; }

	public ForcePanGestureRecognizer(Action<UIPanGestureRecognizer> action) : base(action)
	{
	}

	public override void TouchesBegan(NSSet touches, UIEvent evt)
	{
		base.TouchesBegan(touches, evt);
		SetForce(touches);
	}

	public override void TouchesMoved(NSSet touches, UIEvent evt)
	{
		base.TouchesMoved(touches, evt);
		SetForce(touches);
	}

	public override void Reset()
	{
		base.Reset();
		Force = 0;
	}

	private void SetForce(NSSet touches)
	{
		UITouch? touch = touches.Cast<UITouch>().FirstOrDefault();

		if (touch == null)
		{
			return;
		}

		if (touch.Type == UITouchType.Stylus)
		{
			Force = touch.Force / Math.Sin(touch.AltitudeAngle);
		}
	}
}