using System.ComponentModel;
using Blastic.Forms.Sample.Controls;
using Blastic.Forms.Sample.iOS.Renderers;
using CoreGraphics;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ExtendedTabbedPage), typeof(ExtendedTabbedPageRenderer))]

namespace Blastic.Forms.Sample.iOS.Renderers
{
	public class ExtendedTabbedPageRenderer : TabbedRenderer
	{
		private float? _nativeHeight;
		private float? _nativeY;

		public override void ViewWillLayoutSubviews()
		{
			base.ViewWillLayoutSubviews();

			if (!(Element is ExtendedTabbedPage tabbedPage))
			{
				return;
			}

			if (_nativeY == null || _nativeHeight == null)
			{
				return;
			}

			TabBar.Frame = new CGRect(
				TabBar.Frame.X,
				_nativeY.Value + tabbedPage.TabBarOffset * TabBar.Frame.Height,
				TabBar.Frame.Width,
				TabBar.Frame.Height);

			TabBar.Alpha = 1 - tabbedPage.TabBarOffset;

			Page page = (Page)Element;
			page.Padding = new Thickness(0, 0, 0, tabbedPage.ContainerMargin);
		}

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();

			_nativeY = (float)TabBar.Frame.Y;
			_nativeHeight = (float)Element.Bounds.Height;

			if (!(Element is ExtendedTabbedPage tabbedPage))
			{
				return;
			}

			tabbedPage.TabBarHeight = (float) TabBar.Frame.Height;
		}

		protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null)
			{
				e.OldElement.PropertyChanged -= OnPropertyChanged;
				return;
			}

			if (Element != null)
			{
				Element.PropertyChanged += OnPropertyChanged;
			}
		}

		private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			if (args.PropertyName != nameof(ExtendedTabbedPage.TabBarOffset) && args.PropertyName != nameof(ExtendedTabbedPage.ContainerMargin))
			{
				return;
			}

			ViewWillLayoutSubviews();
		}
	}
}