using System.Windows;
using System.Windows.Controls;

namespace Blastic.Wpf.Controls
{
	public class HelpView : ContentControl
	{
		static HelpView()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(HelpView), new FrameworkPropertyMetadata(typeof(HelpView)));
		}

		public static readonly DependencyProperty HelpContentProperty = DependencyProperty.Register(
			nameof(HelpContentProperty).Replace("Property", ""),
			typeof(object),
			typeof(HelpView),
			new PropertyMetadata(default(object)));
		public object HelpContent
		{
			get => GetValue(HelpContentProperty);
			set => SetValue(HelpContentProperty, value);
		}

		public static readonly DependencyProperty DisableInsteadOfCollapseProperty = DependencyProperty.Register(
			nameof(DisableInsteadOfCollapseProperty).Replace("Property", ""),
			typeof(bool),
			typeof(HelpView),
			new PropertyMetadata(default(bool)));
		public bool DisableInsteadOfCollapse
		{
			get => (bool)GetValue(DisableInsteadOfCollapseProperty);
			set => SetValue(DisableInsteadOfCollapseProperty, value);
		}

		public static readonly DependencyProperty HelpIconMarginProperty = DependencyProperty.Register(
			nameof(HelpIconMarginProperty).Replace("Property", ""),
			typeof(Thickness),
			typeof(HelpView),
			new PropertyMetadata(new Thickness(8, 0, 0, 0)));
		public Thickness HelpIconMargin
		{
			get => (Thickness)GetValue(HelpIconMarginProperty);
			set => SetValue(HelpIconMarginProperty, value);
		}
	}
}