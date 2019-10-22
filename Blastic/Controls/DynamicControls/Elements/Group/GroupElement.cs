using System;
using System.Collections.Generic;
using System.Windows;

namespace Blastic.Controls.DynamicControls.Elements.Group
{
	public class GroupElement : Element, IElementContainer
	{
		public List<IElement> Elements { get; }

		public GroupElement()
		{
			Elements = new List<IElement>();

			Margin = new Thickness(0, 0, 8, 0);
		}

		public void AddElement<TElement>(TElement element, Action<TElement> configure) where TElement : IElement
		{
			configure?.Invoke(element);
			Elements.Add(element);
		}

		protected override Presenter CreatePresenter()
		{
			return new GroupPresenter(Elements);
		}
	}
}