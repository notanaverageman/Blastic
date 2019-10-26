using System.Windows;
using Blastic.Execution;

namespace Blastic.Controls
{
	public partial class BusyIndicator
	{
		public static readonly DependencyProperty ExecutionContextProperty = DependencyProperty.Register(
			nameof(ExecutionContextProperty).Replace("Property", ""),
			typeof(ExecutionContext),
			typeof(BusyIndicator),
			new PropertyMetadata(default(ExecutionContext)));
		public ExecutionContext ExecutionContext
		{
			get => (ExecutionContext)GetValue(ExecutionContextProperty);
			set => SetValue(ExecutionContextProperty, value);
		}

		public BusyIndicator()
		{
			InitializeComponent();
		}

		public void Cancel()
		{
			ExecutionContext?.CancellationTokenSource?.Cancel();
		}
	}
}