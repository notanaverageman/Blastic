using System;
using System.Collections.Generic;
using System.Linq;
using Blastic.ViewManagement.TypeMappers;

namespace Blastic.ViewManagement
{
	public abstract class ViewLocatorBase<T> : IViewLocator<T> where T : class
	{
		private readonly List<ITypeMapper> _typeMappers;

		public ViewLocatorBase(IEnumerable<ITypeMapper> typeMappers)
		{
			_typeMappers = typeMappers
				.OrderBy(x => x.Order)
				.ToList();
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
			IViewAware? viewAware = null;

			if (model is IViewAware)
			{
				viewAware = (IViewAware)model;

				T? view = LocateCached(viewAware);

				if (view != null)
				{
					return view;
				}
			}

			T element = Locate(model.GetType(), model);

			if (viewAware != null)
			{
				PostProcessAttachView(element, viewAware);
			}
			
			return element;
		}

		private T? LocateCached(IViewAware viewAware)
		{
			if (!(viewAware.View.Value is T view))
			{
				return default;
			}

			return PostProcessCachedView(view);
		}

		private T Locate(Type modelType, object model)
		{
			Type? viewType = GetViewTypeForModelType(modelType);

			return viewType == null
				? CreateNotFoundView(modelType, $"Cannot find view for {modelType}.")
				: CreateView(viewType, model);
		}

		private Type? GetViewTypeForModelType(Type modelType)
		{
			foreach (ITypeMapper typeMapper in _typeMappers)
			{
				Type? viewType = typeMapper.Map(modelType);

				if (viewType != null)
				{
					return viewType;
				}
			}

			return null;
		}

		private T CreateView(Type viewType, object model)
		{
			if (viewType.IsInterface || viewType.IsAbstract || !typeof(T).IsAssignableFrom(viewType))
			{
				return CreateNotFoundView(viewType, $"Cannot create {viewType}");
			}

			T view = (T)Activator.CreateInstance(viewType);

			PostProcessCreatedView(view, model);

			return view;
		}

		protected abstract T CreateNotFoundView(Type type, string message);
		protected abstract T PostProcessCachedView(T view);
		protected abstract void PostProcessCreatedView(T view, object model);
		protected abstract void PostProcessAttachView(T view, IViewAware viewAware);
	}
}