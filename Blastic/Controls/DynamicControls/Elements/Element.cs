using System.Windows;
using Blastic.Reactive;
using MaterialDesignThemes.Wpf;

namespace Blastic.Controls.DynamicControls.Elements
{
	public interface IElement
	{
		IReactiveProperty<PackIconKind?> Icon { get; set; }
		IReactiveProperty<string> Label { get; set; }
		IReactiveProperty<string> Help { get; set; }

		IReactiveProperty<bool> IsEnabled { get; set; }

		Thickness Margin { get; set; }
		Thickness Padding { get; set; }
		Thickness IconMargin { get; set; }
		GridLength ColumnWidth { get; set; }
		HorizontalAlignment HorizontalAlignment { get; set; }

		double MinWidth { get; set; }
		double MinHeight { get; set; }

		Presenter ToPresenter();
	}

	public abstract class Element : IElement
	{
		public IReactiveProperty<PackIconKind?> Icon { get; set; }
		public IReactiveProperty<string> Label { get; set; }
		public IReactiveProperty<string> Help { get; set; }

		public IReactiveProperty<bool> IsEnabled { get; set; }

		public Thickness Margin { get; set; }
		public Thickness Padding { get; set; }
		public Thickness IconMargin { get; set; }
		public double IconSize { get; set; }
		public GridLength ColumnWidth { get; set; }
		public HorizontalAlignment HorizontalAlignment { get; set; }

		public double MinWidth { get; set; }
		public double MinHeight { get; set; }

		public Element()
		{
			Icon = new ReactiveProperty<PackIconKind?>();
			Label = new ReactiveProperty<string>();
			Help = new ReactiveProperty<string>();

			IsEnabled = new ReactiveProperty<bool>(true);

			IconSize = 18;
			Margin = new Thickness(8, 8, 8, 8);
			IconMargin = new Thickness(0, 0, 8, 0);
			HorizontalAlignment = HorizontalAlignment.Stretch;
		}

		public virtual Presenter ToPresenter()
		{
			Presenter presenter = CreatePresenter();

			presenter.Help = Help;
			presenter.Label = Label;
			presenter.IconKind = Icon;

			presenter.IsEnabledReactive = IsEnabled;

			presenter.Margin = Margin;
			presenter.Padding = Padding;
			presenter.IconMargin = IconMargin;
			presenter.IconSize = IconSize;
			presenter.ColumnWidth = ColumnWidth;
			presenter.HorizontalAlignment = HorizontalAlignment;

			presenter.MinWidth = MinWidth;
			presenter.MinHeight = MinHeight;

			return presenter;
		}

		protected abstract Presenter CreatePresenter();
	}

	public static class ElementExtensions
	{
		public static T WithIcon<T>(this T element, PackIconKind? icon) where T : IElement
		{
			element.Icon.Value = icon;
			return element;
		}

		public static T WithIcon<T>(this T element, IReactiveProperty<PackIconKind?> icon) where T : IElement
		{
			element.Icon = icon;
			return element;
		}

		public static T WithLabel<T>(this T element, string label) where T : IElement
		{
			element.Label.Value = label;
			return element;
		}

		public static T WithLabel<T>(this T element, IReactiveProperty<string> label) where T : IElement
		{
			element.Label = label;
			return element;
		}

		public static T WithHelp<T>(this T element, string help) where T : IElement
		{
			element.Help.Value = help;
			return element;
		}

		public static T WithHelp<T>(this T element, IReactiveProperty<string> help) where T : IElement
		{
			element.Help = help;
			return element;
		}

		public static T WithIsEnabled<T>(this T element, IReactiveProperty<bool> isEnabled) where T : IElement
		{
			element.IsEnabled = isEnabled;
			return element;
		}

		public static T WithMargin<T>(this T element, Thickness margin) where T : IElement
		{
			element.Margin = margin;
			return element;
		}

		public static T WithPadding<T>(this T element, Thickness padding) where T : IElement
		{
			element.Padding = padding;
			return element;
		}

		public static T WithIconMargin<T>(this T element, Thickness iconMargin) where T : IElement
		{
			element.IconMargin = iconMargin;
			return element;
		}

		public static T WithColumnWidth<T>(this T element, GridLength length) where T : IElement
		{
			element.ColumnWidth = length;
			return element;
		}

		public static T WithHorizontalAlignment<T>(this T element, HorizontalAlignment alignment) where T : IElement
		{
			element.HorizontalAlignment = alignment;
			return element;
		}

		public static T WithMinWidth<T>(this T element, double minWidth) where T : IElement
		{
			element.MinWidth = minWidth;
			return element;
		}

		public static T WithMinHeight<T>(this T element, double minHeight) where T : IElement
		{
			element.MinHeight = minHeight;
			return element;
		}
	}
}