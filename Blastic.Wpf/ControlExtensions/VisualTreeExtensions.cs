using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using Blastic.Reactive;

namespace Blastic.Wpf.ControlExtensions
{
	public static class VisualTreeExtensions
	{
		private static readonly Dictionary<Type, HashSet<DependencyProperty>> DefaultDependencyProperties;

		static VisualTreeExtensions()
		{
			DefaultDependencyProperties = new Dictionary<Type, HashSet<DependencyProperty>>();

			AddDefaultDependencyProperty<TextBox>(TextBox.TextProperty);
			AddDefaultDependencyProperty<TextBlock>(TextBlock.TextProperty);
			AddDefaultDependencyProperty<ButtonBase>(ButtonBase.CommandProperty);
		}

		public static T? FindChild<T>(DependencyObject? parent) where T : DependencyObject
		{
			if (parent == null)
			{
				return null;
			}

			T? foundChild = null;

			int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

			for (int i = 0; i < childrenCount; i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(parent, i);

				if (child is T result)
				{
					foundChild = result;
					break;
				}

				foundChild = FindChild<T>(child);

				if (foundChild != null)
				{
					break;
				}
			}

			return foundChild;
		}

		public static FrameworkElement? FindChild(DependencyObject? parent, object source)
		{
			if (parent == null)
			{
				return null;
			}

			FrameworkElement? foundChild = null;
			int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

			for (int childIndex = 0; childIndex < childrenCount; childIndex++)
			{
				if (VisualTreeHelper.GetChild(parent, childIndex) is FrameworkElement child)
				{
					foreach (DependencyProperty dependencyProperty in GetDefaultDependencyProperties(child))
					{
						if (QueryBinding(child, dependencyProperty, source))
						{
							return child;
						}
					}

					foundChild = FindChild(child, source);

					if (foundChild != null)
					{
						break;
					}
				}
			}
			return foundChild;
		}

		private static bool QueryBinding(
			DependencyObject dependencyObject,
			DependencyProperty dependencyProperty,
			object source)
		{
			BindingExpression? bindingExpression = BindingOperations.GetBindingExpression(dependencyObject, dependencyProperty);

			if (bindingExpression == null)
			{
				return false;
			}

			if (source is IReactiveProperty)
			{
				return bindingExpression.ResolvedSource == source;
			}

			object resolvedSource = bindingExpression.ResolvedSource;

			if (resolvedSource == null)
			{
				return false;
			}

			object? bindingSource = bindingExpression.ResolvedSource
				?.GetType()
				 .GetProperty(bindingExpression.ResolvedSourcePropertyName)
				?.GetValue(bindingExpression.ResolvedSource);

			return bindingSource == source;
		}

		private static IEnumerable<DependencyProperty> GetDefaultDependencyProperties(DependencyObject dependencyObject)
		{
			Type objectType = dependencyObject.GetType();
			IEnumerable<DependencyProperty> result = [];

			foreach (Type index in DefaultDependencyProperties.Keys.Where(x => x.IsAssignableFrom(objectType)))
			{
				result = result.Concat(DefaultDependencyProperties[index]);
			}

			return result;
		}

		public static void AddDefaultDependencyProperty<T>(DependencyProperty dependencyProperty)
		{
			if (!DefaultDependencyProperties.TryGetValue(typeof(T), out HashSet<DependencyProperty>? properties))
			{
				properties = [];
				DefaultDependencyProperties[typeof(T)] = properties;
			}

			properties.Add(dependencyProperty);
		}
	}
}