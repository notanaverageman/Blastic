using System.Reactive.Linq;
using Blastic.DynamicControls;
using Blastic.Reactive;
using MaterialDesignThemes.Wpf;

namespace Blastic.Wpf.Material.DynamicControls
{
	public static class ElementExtensions
	{
		public static T WithIcon<T>(this T element, PackIconKind icon) where T : IElement
		{
			element.Icon = new ReactiveProperty<object>(icon);
			return element;
		}

		public static T WithIcon<T>(this T element, IReactiveProperty<PackIconKind> icon) where T : IElement
		{
			element.Icon = icon.Select(x => (object)x).ToReadOnlyReactiveProperty(icon.Value);
			return element;
		}
	}
}