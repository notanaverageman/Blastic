using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Blastic.Controls.DynamicControls;
using Blastic.Uno.Shared.Controls.DynamicControls;
using Blastic.ViewManagement.TypeMappers;

namespace Blastic.ViewManagement
{
	public class ViewLocator : IViewLocator
	{
		private readonly List<ITypeMapper> _typeMappers;

		public ViewLocator()
		{
			_typeMappers = new List<ITypeMapper>();
		}

		public ViewLocator WithTypeMapper<TViewModel, TView>()
		{
			return WithTypeMapper(new InheritanceTypeMapper(typeof(TViewModel), typeof(TView)));
		}

		public ViewLocator WithTypeMapper(ITypeMapper typeMapper)
		{
			_typeMappers.Add(typeMapper);
			return this;
		}

		public FrameworkElement Locate(object model)
		{
			if (model is IViewAware viewAware)
			{
				FrameworkElement view = LocateCached(viewAware);

				if (view != null)
				{
					return view;
				}
			}
			else
			{
				viewAware = null;
			}

			FrameworkElement frameworkElement = Locate(model.GetType());

			if (frameworkElement != null)
			{
				frameworkElement.DataContext = model;
			}

			if (frameworkElement is DynamicControl dynamicControl && model is DynamicModel dynamicModel)
			{
				dynamicControl.Model = dynamicModel;
			}

			if (viewAware != null)
			{
				viewAware.View.Value = frameworkElement;
			}
			
			return frameworkElement;
		}

		private FrameworkElement LocateCached(IViewAware viewAware)
		{
			FrameworkElement view = viewAware.View.Value;

			if (view == null)
			{
				return null;
			}

			// TODO:
			//if (!(view is Window window))
			//{
			//	return view;
			//}

			// TODO:
			//if (window.IsLoaded && new WindowInteropHelper(window).Handle != IntPtr.Zero)
			//{
			//	return view;
			//}

			// TODO:
			return view;
		}

		private FrameworkElement Locate(Type modelType)
		{
			Type viewType = GetViewTypeForModelType(modelType);

			return viewType == null
				? new TextBlock
				{
					Text = $"Cannot find view for {modelType}."
				}
				: CreateView(viewType);
		}

		private Type GetViewTypeForModelType(Type modelType)
		{
			foreach (ITypeMapper typeMapper in _typeMappers)
			{
				Type viewType = typeMapper.Map(modelType);

				if (viewType != null)
				{
					return viewType;
				}
			}

			return null;
		}

		private FrameworkElement CreateView(Type viewType)
		{
			if (viewType.IsInterface || viewType.IsAbstract || !typeof(UIElement).IsAssignableFrom(viewType))
			{
				return new TextBlock
				{
					Text = $"Cannot create {viewType.FullName}."
				};
			}

			return (FrameworkElement)Activator.CreateInstance(viewType);
		}
	}
}