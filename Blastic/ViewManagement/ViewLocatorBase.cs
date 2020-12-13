using System;
using System.Collections.Generic;
using System.Linq;
using Blastic.ViewManagement.TypeMappers;

namespace Blastic.ViewManagement
{
	/// <summary>
	/// Base implementation for <see cref="IViewLocator{T}"/>. This class can have many
	/// <see cref="ITypeMapper"/> instances to lookup the type of the view that
	/// corresponds to a given viewmodel.</summary>
	/// <typeparam name="T">Base type for platform's views.</typeparam>
	public abstract class ViewLocatorBase<T> : IViewLocator<T> where T : class
	{
		private readonly List<ITypeMapper> _typeMappers;

		/// <summary>
		/// Creates a new instance with given type mappers.
		/// </summary>
		/// <param name="typeMappers">Type mappers that are used to lookup view types for viewmodels.</param>
		public ViewLocatorBase(IEnumerable<ITypeMapper> typeMappers)
		{
			_typeMappers = typeMappers
				.OrderBy(x => x.Order)
				.ToList();
		}

		/// <summary>
		/// Fluent method that adds a new <see cref="InheritanceTypeMapper"/> for given
		/// type parameters.
		/// </summary>
		/// <typeparam name="TViewModel">Type of the view model.</typeparam>
		/// <typeparam name="TView">Type of the view.</typeparam>
		/// <returns></returns>
		public ViewLocatorBase<T> WithTypeMapper<TViewModel, TView>()
		{
			return WithTypeMapper(new InheritanceTypeMapper(typeof(TViewModel), typeof(TView)));
		}

		/// <summary>
		/// Fluent method that adds the given type mapper to the list.
		/// </summary>
		/// <param name="typeMapper"></param>
		/// <returns></returns>
		public ViewLocatorBase<T> WithTypeMapper(ITypeMapper typeMapper)
		{
			_typeMappers.Add(typeMapper);
			return this;
		}

		/// <inheritdoc />
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
				AttachView(element, viewAware);
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

		/// <summary>
		/// Create a placeholder view for a viewmodel that could not be mapped to a view.
		/// </summary>
		/// <param name="type">Type of viewmodel.</param>
		/// <param name="message">Error message.</param>
		/// <returns>A view that shows the error message.</returns>
		protected abstract T CreateNotFoundView(Type type, string message);

		/// <summary>
		/// Modify or check a cached view before using it. Can return a different view or null.
		/// </summary>
		/// <param name="view">The view to inspect.</param>
		/// <returns>The given view or another view or null.</returns>
		protected abstract T? PostProcessCachedView(T view);

		/// <summary>
		/// Modify a newly created view.
		/// </summary>
		/// <param name="view">The view to inspect.</param>
		/// <param name="model">The viewmodel corresponding to the view.</param>
		protected abstract void PostProcessCreatedView(T view, object model);

		/// <summary>
		/// Attach the given view to given view aware viewmodel.
		/// </summary>
		/// <param name="view">The view to attach.</param>
		/// <param name="viewAware">The viewmodel to attach the view.</param>
		protected abstract void AttachView(T view, IViewAware viewAware);
	}
}