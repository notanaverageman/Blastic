using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
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

		public UIElement Locate(object model)
		{
			if (model is IViewAware viewAware)
			{
				UIElement view = LocateCached(viewAware);

				if (view != null)
				{
					return view;
				}
			}
			else
			{
				viewAware = null;
			}

			UIElement uiElement = Locate(model.GetType());

			if (viewAware != null)
			{
				viewAware.View.Value = uiElement;
			}
			
			return uiElement;
		}

		private UIElement LocateCached(IViewAware viewAware)
		{
			UIElement view = viewAware.View.Value;

			if (view == null)
			{
				return null;
			}

			if (!(view is Window window))
			{
				return view;
			}

			if (window.IsLoaded && new WindowInteropHelper(window).Handle != IntPtr.Zero)
			{
				return view;
			}

			return null;
		}

		private UIElement Locate(Type modelType)
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

		private UIElement CreateView(Type viewType)
		{
			if (viewType.IsInterface || viewType.IsAbstract || !typeof(UIElement).IsAssignableFrom(viewType))
			{
				return new TextBlock
				{
					Text = $"Cannot create {viewType.FullName}."
				};
			}

			return (UIElement)Activator.CreateInstance(viewType);
		}
	}
}