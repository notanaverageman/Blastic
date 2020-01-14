using System;
using System.Collections.Generic;
using Blastic.ViewManagement.TypeMappers;

namespace Blastic.ViewManagement
{
	public abstract class ViewLocatorBase<T> : IViewLocator<T> where T : class
	{
		private readonly List<ITypeMapper> _typeMappers;

		public ViewLocatorBase()
		{
			_typeMappers = new List<ITypeMapper>();
		}

		public ViewLocatorBase<T> WithTypeMapper<TViewModel, TView>()
		{
			return WithTypeMapper(new InheritanceTypeMapper(typeof(TViewModel), typeof(TView)));
		}

		public ViewLocatorBase<T> WithTypeMapper(ITypeMapper typeMapper)
		{
			_typeMappers.Add(typeMapper);
			return this;
		}

		public T Locate(object model)
		{
			if (model is IViewAware viewAware)
			{
				T view = LocateCached(viewAware);

				if (view != null)
				{
					return view;
				}
			}
			else
			{
				viewAware = null;
			}

			T element = Locate(model.GetType());

			if (viewAware != null)
			{
				viewAware.View.Value = element;

				PostProcessAttachView(element, viewAware);
			}
			
			return element;
		}

		private T LocateCached(IViewAware viewAware)
		{
			T view = viewAware.View.Value as T;

			if (view == null)
			{
				return default;
			}

			return PostProcessCachedView(view);
		}

		private T Locate(Type modelType)
		{
			Type viewType = GetViewTypeForModelType(modelType);

			return viewType == null
				? CreateNotFoundView(modelType, $"Cannot find view for {modelType}.")
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

		private T CreateView(Type viewType)
		{
			if (viewType.IsInterface || viewType.IsAbstract || !typeof(T).IsAssignableFrom(viewType))
			{
				return CreateNotFoundView(viewType, $"Cannot create {viewType}");
			}

			return (T)Activator.CreateInstance(viewType);
		}

		protected abstract T CreateNotFoundView(Type type, string message);
		protected abstract T PostProcessCachedView(T view);
		protected abstract void PostProcessAttachView(T view, IViewAware viewAware);
	}
}