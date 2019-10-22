using System;
using System.Collections.Generic;
using Blastic.ControlExtensions;
using Blastic.Controls.DynamicControls.Elements;
using Blastic.Controls.DynamicControls.Elements.Action;
using Blastic.Controls.DynamicControls.Elements.Boolean;
using Blastic.Controls.DynamicControls.Elements.Group;
using Blastic.Controls.DynamicControls.Elements.Password;
using Blastic.Controls.DynamicControls.Elements.Text;
using Reactive.Bindings;

namespace Blastic.Controls.DynamicControls
{
	public interface IElementContainer
	{
		List<IElement> Elements { get; }
		void AddElement<TElement>(TElement element, Action<TElement> configure) where TElement : IElement;
	}

	public class ElementContainer : IElementContainer
	{
		public List<IElement> Elements { get; }

		public ElementContainer()
		{
			Elements = new List<IElement>();
		}

		public void AddElement<TElement>(TElement element, Action<TElement> configure) where TElement : IElement
		{
			configure?.Invoke(element);
			Elements.Add(element);
		}
	}

	public static class ElementContainerExtensions
	{
		public static T AddText<T>(
			this T container,
			IReactiveProperty<string> property,
			Action<TextField> configure = null) where T : IElementContainer
		{
			TextField element = new TextField(property);
			container.AddElement(element, configure);

			return container;
		}

		public static T AddPassword<T>(
			this T container,
			IReactiveProperty<string> property,
			Action<PasswordField> configure = null) where T : IElementContainer
		{
			PasswordField element = new PasswordField(property);
			container.AddElement(element, configure);

			return container;
		}

		public static T AddNumber<T>(
			this T container,
			IReactiveProperty<int> property,
			Action<TextField> configure = null) where T : IElementContainer
		{
			TextField element = new TextField(property);
			element.Mask.Value = TextBoxMasking.IntegerMask;

			container.AddElement(element, configure);

			return container;
		}

		public static T AddNumber<T>(
			this T container,
			IReactiveProperty<double> property,
			Action<TextField> configure = null) where T : IElementContainer
		{
			TextField element = new TextField(property);
			element.Mask.Value = TextBoxMasking.FloatingPointMask;

			container.AddElement(element, configure);

			return container;
		}

		public static T AddBoolean<T>(
			this T container,
			IReactiveProperty<bool> property,
			Action<BooleanField> configure = null) where T : IElementContainer
		{
			BooleanField element = new BooleanField(property);
			container.AddElement(element, configure);

			return container;
		}

		public static T AddAction<T>(
			this T container,
			ReactiveCommand command,
			Action<ActionElement> configure = null) where T : IElementContainer
		{
			ActionElement element = new ActionElement(command);
			container.AddElement(element, configure);

			return container;
		}

		public static T AddGroup<T>(
			this T container,
			Action<GroupElement> configure = null) where T : IElementContainer
		{
			GroupElement element = new GroupElement();
			container.AddElement(element, configure);

			return container;
		}
	}
}