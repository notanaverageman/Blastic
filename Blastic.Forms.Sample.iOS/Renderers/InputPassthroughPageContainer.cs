using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Blastic.Forms.Sample.iOS.Renderers
{
	public class InputPassthroughPageContainer : UIView, IUIAccessibilityContainer
	{
		private readonly InputPassthroughPageRenderer _parent;
		private NSArray _accessibilityElements;
		private bool _disposed;
		private bool _loaded;

		public InputPassthroughPageContainer(InputPassthroughPageRenderer parent)
		{
			AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			_parent = parent;

			Frame = UIScreen.MainScreen.Bounds;
		}

		public InputPassthroughPageContainer()
		{
			IsAccessibilityElement = false;
		}

		public override bool IsAccessibilityElement
		{
			get => false;
			set => base.IsAccessibilityElement = value;
		}

		public virtual NSArray AccessibilityElements
		{
			[Export("accessibilityElements", ArgumentSemantic.Copy)]
			get
			{
				if (_loaded)
				{
					return _accessibilityElements;
				}

				// lazy-loading this list so that the expensive call to GetAccessibilityElements only happens when VoiceOver is on.
				if (_accessibilityElements == null || _accessibilityElements.Count == 0)
				{
					List<NSObject> elements = _parent.GetAccessibilityElements();
					if (elements != null)
					{
						_accessibilityElements = NSArray.FromNSObjects(elements.ToArray());
					}
				}

				_loaded = true;
				return _accessibilityElements;
			}
		}

		public void ClearAccessibilityElements()
		{
			_accessibilityElements = null;
			_loaded = false;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !_disposed)
			{
				ClearAccessibilityElements();
				_disposed = true;
			}
			base.Dispose(disposing);
		}

		public override UIView HitTest(CGPoint point, UIEvent uievent)
		{
			UIView hitTest = base.HitTest(point, uievent);
			return Equals(hitTest) ? null : hitTest;
		}
	}
}