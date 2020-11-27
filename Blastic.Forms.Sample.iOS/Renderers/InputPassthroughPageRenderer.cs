using Blastic.Forms.Sample.Controls;
using Blastic.Forms.Sample.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(InputPassthroughPage), typeof(InputPassthroughPageRenderer))]

namespace Blastic.Forms.Sample.iOS.Renderers
{
	public class InputPassthroughPageRenderer : PageRenderer
	{
		private InputPassthroughPageContainer _pageContainer;

		public override void LoadView()
		{
			if (_pageContainer == null)
			{
				_pageContainer = new InputPassthroughPageContainer(this);
			}

			View = _pageContainer;
		}
	}
}