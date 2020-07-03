using System;
using System.Collections.Generic;
using Blastic.DynamicControls;
using Blastic.Forms.DynamicControls;
using Blastic.ViewManagement;
using Blastic.ViewManagement.TypeMappers;
using Xamarin.Forms;

namespace Blastic.Forms.ViewManagement
{
	public class ViewLocator : ViewLocatorBase<VisualElement>
	{
		public ViewLocator(IEnumerable<ITypeMapper> typeMappers) : base(typeMappers)
		{
		}

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

		protected override void PostProcessCreatedView(VisualElement view, object model)
		{
			view.BindingContext = model;

			if (view is DynamicControl dynamicControl && model is DynamicModel dynamicModel)
			{
				dynamicControl.Model = dynamicModel;
			}
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