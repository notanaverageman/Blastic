using System;
using Blastic.ViewManagement;
using Xamarin.Forms;

namespace Blastic.Forms.ViewManagement
{
	public class ViewLocator : ViewLocatorBase<VisualElement>
	{
		protected override void PostProcessAttachView(VisualElement view, IViewAware viewAware)
		{
			// TODO: Use parent, navigation? https://forums.xamarin.com/discussion/80435/loaded-unloaded-events-for-views
			//view. += (sender, args) =>
			//{
			//	viewAware.View.Value = null;
			//};
		}

		protected override VisualElement PostProcessCachedView(VisualElement view)
		{
			return view;
		}

		protected override VisualElement CreateNotFoundView(Type type, string message)
		{
			return new Label
			{
				Text = message
			};
		}
	}
}