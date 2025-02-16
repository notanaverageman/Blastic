using System;
using System.Collections.Generic;
using Blastic.DynamicControls.Properties;

namespace Blastic.DynamicControls.Elements
{
	public class GroupElement : Element, IElementContainer
	{
		public List<IElement> Elements { get; }

		public GroupElement()
		{
			Elements = [];

			Margin = new Thickness(0, 0, 8, 0);
		}

		public void AddElement<TElement>(TElement element, Action<TElement>? configure) where TElement : IElement
		{
			configure?.Invoke(element);
			Elements.Add(element);
		}
	}
}