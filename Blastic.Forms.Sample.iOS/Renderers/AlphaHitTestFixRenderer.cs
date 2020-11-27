using Blastic.Forms.Sample.iOS.Renderers;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ContentView), typeof(AlphaHitTestFixRenderer))]

namespace Blastic.Forms.Sample.iOS.Renderers
{
	public class AlphaHitTestFixRenderer : VisualElementRenderer<VisualElement>
	{
		public override UIView HitTest(CGPoint point, UIEvent uievent)
		{
			if (!UserInteractionEnabled)
			{
				return null;
			}

			UIView result = base.HitTest(point, uievent);

			if (UserInteractionEnabled && Element is Layout layout && !layout.CascadeInputTransparent)
			{
				if (Equals(result))
				{
					return null;
				}
			}

			return result;
		}

		private void ResolveLayoutChanges()
		{
			if (Element is Layout layout)
			{
				layout.ResolveLayoutChanges();
			}
		}

		public override void LayoutSubviews()
		{
			ResolveLayoutChanges();
			base.LayoutSubviews();
		}

		public override CGSize SizeThatFits(CGSize size)
		{
			ResolveLayoutChanges();
			return base.SizeThatFits(size);
		}
	}
}