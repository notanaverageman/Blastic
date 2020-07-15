using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Blastic.ViewManagement;
using Blastic.ViewManagement.TypeMappers;

namespace Blastic.Wpf.ViewManagement
{
	public class ViewLocator : ViewLocatorBase<FrameworkElement>
	{
		internal static IViewLocator<FrameworkElement> Current { get; set; }

		public ViewLocator(IEnumerable<ITypeMapper> typeMappers) : base(typeMappers)
		{
		}

		protected override void PostProcessAttachView(FrameworkElement view, IViewAware viewAware)
		{
			view.Unloaded += (sender, args) =>
			{
				viewAware.View.Value = null;
			};
		}

		protected override FrameworkElement PostProcessCachedView(FrameworkElement view)
		{
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

		protected override void PostProcessCreatedView(FrameworkElement view, object model)
		{
			view.DataContext = model;
		}

		protected override FrameworkElement CreateNotFoundView(Type type, string message)
		{
			return new TextBlock
			{
				Text = message
			};
		}

		internal static void HookLoadedUnloadedEvents()
		{
			void Register<T>(RoutedEvent routedEvent, Action<object, RoutedEventArgs> action)
			{
				EventManager.RegisterClassHandler(
					typeof(T),
					routedEvent,
					new RoutedEventHandler(action),
					true);
			}

			Register<FrameworkElement>(FrameworkElement.LoadedEvent, OnFrameworkElementLoaded);
			Register<FrameworkElement>(FrameworkElement.UnloadedEvent, OnFrameworkElementUnloaded);
			Register<ContentElement>(FrameworkContentElement.LoadedEvent, OnFrameworkElementLoaded);
			Register<ContentElement>(FrameworkContentElement.UnloadedEvent, OnFrameworkElementUnloaded);
		}

		private static void OnFrameworkElementLoaded(object o, RoutedEventArgs e)
		{
			if (!(o is FrameworkElement frameworkElement))
			{
				return;
			}

			DependencyPropertyDescriptor
				.FromProperty(FrameworkElement.DataContextProperty, typeof(FrameworkElement))
				.AddValueChanged(frameworkElement, OnDataContextChanged);

			SetView(frameworkElement, false);
		}

		private static void OnFrameworkElementUnloaded(object o, RoutedEventArgs e)
		{
			if (!(o is FrameworkElement frameworkElement))
			{
				return;
			}

			DependencyPropertyDescriptor
				.FromProperty(FrameworkElement.DataContextProperty, typeof(FrameworkElement))
				.RemoveValueChanged(frameworkElement, OnDataContextChanged);

			SetView(frameworkElement, true);
		}

		private static void OnDataContextChanged(object x, EventArgs y)
		{
			if (!(x is FrameworkElement frameworkElement))
			{
				return;
			}

			SetView(frameworkElement, false);
		}

		private static void SetView(FrameworkElement frameworkElement, bool shouldSetNull)
		{
			if (!(frameworkElement.DataContext is IViewAware viewAware))
			{
				return;
			}

			ValueSource valueSource = DependencyPropertyHelper.GetValueSource(
				frameworkElement,
				FrameworkElement.DataContextProperty);

			if (valueSource.BaseValueSource == BaseValueSource.Inherited)
			{
				return;
			}

			viewAware.View.Value = shouldSetNull ? null : frameworkElement;
		}
	}
}