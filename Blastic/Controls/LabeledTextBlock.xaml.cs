using System.Windows;

namespace Blastic.Controls
{
	public partial class LabeledTextBlock
	{
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
			nameof(TextProperty).Replace("Property", ""),
			typeof(string),
			typeof(LabeledTextBlock),
			new PropertyMetadata(default));
		public string Text
		{
			get => (string)GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}

		public LabeledTextBlock()
		{
			InitializeComponent();
		}
	}
}