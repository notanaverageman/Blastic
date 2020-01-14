using System.Windows;
using Blastic.Execution;

namespace Blastic.Wpf.Execution
{
	public partial class ExecutionContextView
	{
		public static readonly DependencyProperty ExecutionContextProperty = DependencyProperty.Register(
			nameof(ExecutionContextProperty).Replace("Property", ""),
			typeof(ExecutionContext),
			typeof(ExecutionContextView),
			new PropertyMetadata(default(ExecutionContext)));
		public ExecutionContext ExecutionContext
		{
			get => (ExecutionContext)GetValue(ExecutionContextProperty);
			set => SetValue(ExecutionContextProperty, value);
		}

		public ExecutionContextView()
		{
			InitializeComponent();
		}
	}
}