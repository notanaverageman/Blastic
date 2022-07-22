using System;
using System.Collections.Generic;
using Blastic.DynamicControls;
using Blastic.Forms.DynamicControls;
using Blastic.ViewManagement;
using Blastic.ViewManagement.TypeMappers;
using Xamarin.Forms;

namespace Blastic.Forms.ViewManagement
{
	/// <summary>
	/// Default implementation of <see cref="IViewLocator{T}"/> for Xamarin Forms.
	/// </summary>
	public class ViewLocator : ViewLocatorBase<VisualElement>
	{
		/// <summary>
		/// This value should be set on initialization.
		/// </summary>
		internal static IViewLocator<VisualElement> Current { get; set; }

		/// <inheritdoc />
		public ViewLocator(IEnumerable<ITypeMapper> typeMappers) : base(typeMappers)
		{
		}
		
		/// <inheritdoc />
		protected override void SubscribeViewUnloadEvent(VisualElement view, IViewAware viewAware)
		{
			// TODO: Use parent, navigation? https://forums.xamarin.com/discussion/80435/loaded-unloaded-events-for-views
			//view. += (sender, args) =>
			//{
			//	viewAware.View.Value = null;
			//};
		}

		/// <inheritdoc />
		protected override VisualElement PostProcessCachedView(VisualElement view)
		{
			return view;
		}

		/// <inheritdoc />
		protected override void PostProcessCreatedView(VisualElement view, object model)
		{
			view.BindingContext = model;

			if (view is DynamicControl dynamicControl && model is DynamicModel dynamicModel)
			{
				dynamicControl.Model = dynamicModel;
			}
		}

		/// <inheritdoc />
		protected override VisualElement CreateNotFoundView(Type type, string message)
		{
			return new Label
			{
				Text = message
			};
		}
	}
}