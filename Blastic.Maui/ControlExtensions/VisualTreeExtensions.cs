using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Blastic.Reactive;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace Blastic.Maui.ControlExtensions
{
	public static class VisualTreeExtensions
	{
		private static readonly Dictionary<Type, HashSet<BindableProperty>> DefaultProperties;

		static VisualTreeExtensions()
		{
			DefaultProperties = new Dictionary<Type, HashSet<BindableProperty>>();

			AddDefaultDependencyProperty<Label>(Label.TextProperty);
			AddDefaultDependencyProperty<Entry>(Entry.TextProperty);
			AddDefaultDependencyProperty<Button>(Button.CommandProperty);
		}

		public static T? FindChild<T>(IVisualTreeElement? parent) where T : VisualElement
		{
			if (parent == null)
			{
				return null;
			}

			T? foundChild = null;
			IReadOnlyList<IVisualTreeElement> children = parent.GetVisualChildren();

			foreach (IVisualTreeElement child in children)
			{
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

		public static VisualElement? FindChild(IVisualTreeElement? parent, object source)
		{
			if (parent == null)
			{
				return null;
			}

			VisualElement? foundChild = null;
			IReadOnlyList<IVisualTreeElement> children = parent.GetVisualChildren();

			foreach (IVisualTreeElement child in children)
			{
				if (child is not VisualElement element)
				{
					continue;
				}

				foreach (BindableProperty property in GetDefaultDependencyProperties(element))
				{
					if (QueryBinding(element, property, source))
					{
						return element;
					}
				}

				foundChild = FindChild(child, source);

				if (foundChild != null)
				{
					break;
				}
			}

			return foundChild;
		}

		private static bool QueryBinding(
			BindableObject bindable,
			BindableProperty property,
			object source)
		{
			object? bindingExpression = bindable.GetBinding(property)?.GetBindingExpression();
			IList? parts = bindingExpression?.GetParts();

			if (parts == null)
			{
				return false;
			}

			object? value = bindable.BindingContext;

			for (int i = 0; i < parts.Count; i++)
			{
				if (value == null)
				{
					return false;
				}

				object? part = parts[i];

				if (part == null)
				{
					return false;
				}

				value = part.GetValue(value);

				if (i == parts.Count - 2 && source is IReactiveProperty)
				{
					return value == source;
				}
			}

			return false;
		}

		private static IEnumerable<BindableProperty> GetDefaultDependencyProperties(BindableObject bindable)
		{
			Type objectType = bindable.GetType();
			IEnumerable<BindableProperty> result = Enumerable.Empty<BindableProperty>();

			foreach (Type index in DefaultProperties.Keys.Where(x => x.IsAssignableFrom(objectType)))
			{
				result = result.Concat(DefaultProperties[index]);
			}

			return result;
		}

		public static void AddDefaultDependencyProperty<T>(BindableProperty property)
		{
			if (!DefaultProperties.TryGetValue(typeof(T), out HashSet<BindableProperty>? properties))
			{
				properties = new HashSet<BindableProperty>();
				DefaultProperties[typeof(T)] = properties;
			}

			properties.Add(property);
		}

		private static Binding? GetBinding(this BindableObject bindable, BindableProperty property)
		{
			object? context = typeof(BindableObject)
				.GetTypeInfo()
				.GetDeclaredMethod("GetContext")
				?.Invoke(bindable, [property]);

			object? bindings = context
				?.GetType()
				.GetTypeInfo()
				.GetDeclaredField("Bindings")
				?.GetValue(context);

			object? values = bindings
				?.GetType()
				.GetTypeInfo()
				.GetDeclaredProperty("Values")
				?.GetValue(bindings);

			if (values is not IList<BindingBase> bindingList)
			{
				throw new InvalidOperationException("Reflection is not successful while getting bindings.");
			}

			return bindingList.LastOrDefault() as Binding;
		}

		private static object? GetBindingExpression(this Binding self)
		{
			FieldInfo? fieldInfo = self.GetType().GetTypeInfo().GetDeclaredField("_expression");
			return fieldInfo?.GetValue(self);
		}

		private static IList? GetParts(this object bindingExpression)
		{
			FieldInfo? fieldInfo = bindingExpression.GetType().GetTypeInfo().GetDeclaredField("_parts");
			return fieldInfo?.GetValue(bindingExpression) as IList;
		}

		private static object? GetValue(this object part, object source)
		{
			MethodInfo? methodInfo = part
				.GetType()
				.GetTypeInfo()
				.GetDeclaredMethod("TryGetValue");

			object? value = null;
			object?[] parameters = { source, value };

			methodInfo?.Invoke(part, parameters);
			value = parameters[1];

			return value;
		}
	}
}