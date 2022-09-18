using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blastic.Commanding;
using Blastic.ControlExtensions;
using Blastic.DynamicControls.Elements;
using Blastic.Reactive;

namespace Blastic.DynamicControls
{
	public interface IElementContainer
	{
		double MinWidth { get; set; }
		double MinHeight { get; set; }

		List<IElement> Elements { get; }
		void AddElement<TElement>(TElement element, Action<TElement>? configure) where TElement : IElement;
	}

	public class ElementContainer : IElementContainer
	{
		public double MinWidth { get; set; }
		public double MinHeight { get; set; }

		public List<IElement> Elements { get; }

		public ElementContainer()
		{
			Elements = new List<IElement>();
		}

		public void AddElement<TElement>(TElement element, Action<TElement>? configure) where TElement : IElement
		{
			configure?.Invoke(element);
			Elements.Add(element);
		}
	}

	public static class ElementContainerExtensions
	{
		public static T WithMinWidth<T>(
			this T container,
			double minWidth) where T : IElementContainer
		{
			container.MinWidth = minWidth;
			return container;
		}

		public static T WithMinHeight<T>(
			this T container,
			double minHeight) where T : IElementContainer
		{
			container.MinHeight = minHeight;
			return container;
		}

		public static T AddText<T>(
			this T container,
			IReactiveProperty<string?> property,
			Action<TextField>? configure = null) where T : IElementContainer
		{
			TextField element = new(property);
			container.AddElement(element, configure);

			return container;
		}

		public static T AddLabel<T>(
			this T container,
			IReadOnlyReactiveProperty<string?> property,
			Action<LabelField>? configure = null) where T : IElementContainer
		{
			LabelField element = new(property);
			container.AddElement(element, configure);

			return container;
		}

		public static T AddLabel<T>(
			this T container,
			string? label,
			Action<LabelField>? configure = null) where T : IElementContainer
		{
			return container.AddLabel(new ReactiveProperty<string?>(label), configure);
		}

		public static T AddPassword<T>(
			this T container,
			IReactiveProperty<string> property,
			Action<PasswordField>? configure = null) where T : IElementContainer
		{
			PasswordField element = new(property);
			container.AddElement(element, configure);

			return container;
		}

		public static T AddNumber<T>(
			this T container,
			IReactiveProperty<int> property,
			Action<TextField>? configure = null) where T : IElementContainer
		{
			TextField element = new(property)
			{
				Mask =
				{
					Value = TextBoxMasks.IntegerMask
				},
				Keyboard =
				{
					Value = Keyboard.Numeric
				}
			};

			container.AddElement(element, configure);

			return container;
		}

		public static T AddNumber<T>(
			this T container,
			IReactiveProperty<double> property,
			Action<TextField>? configure = null) where T : IElementContainer
		{
			TextField element = new(property)
			{
				Mask =
				{
					Value = TextBoxMasks.FloatingPointMask
				},
				Keyboard =
				{
					Value = Keyboard.Numeric
				}
			};

			container.AddElement(element, configure);

			return container;
		}

		public static T AddBoolean<T>(
			this T container,
			IReactiveProperty<bool> property,
			Action<BooleanField>? configure = null) where T : IElementContainer
		{
			BooleanField element = new(property);
			container.AddElement(element, configure);

			return container;
		}

		public static T AddSelection<T, TSelection>(
			this T container,
			IReactiveProperty<TSelection> property,
			IReadOnlyList<SelectionValueWithLabel<TSelection>> values,
			Action<SelectionField<TSelection>>? configure = null) where T : IElementContainer
		{
			SelectionField<TSelection> element = new(property, values);
			container.AddElement(element, configure);

			return container;
		}

		public static T AddAction<T>(
			this T container,
			AsyncCommand command,
			Action<ActionElement>? configure = null) where T : IElementContainer
		{
			ActionElement element = new(command);
			container.AddElement(element, configure);

			return container;
		}

		public static T AddAction<T>(
			this T container,
			Action action,
			Action<ActionElement>? configure = null) where T : IElementContainer
		{
			return container.AddAction(new AsyncCommand(action), configure);
		}

		public static T AddAction<T>(
			this T container,
			Func<Task> action,
			Action<ActionElement>? configure = null) where T : IElementContainer
		{
			return container.AddAction(new AsyncCommand(action), configure);
		}

		public static T AddGroup<T>(
			this T container,
			Action<GroupElement>? configure = null) where T : IElementContainer
		{
			GroupElement element = new();
			container.AddElement(element, configure);

			return container;
		}
	}
}