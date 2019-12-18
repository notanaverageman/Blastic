using System.Windows;
using Blastic.Execution;
using Blastic.Reactive;

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

		public Command Cancel { get; }

		public BusyIndicator()
		{
			InitializeComponent();

			Cancel = new Command().WithSubscribe(x =>
			{
				ExecutionContext?.CancellationTokenSource?.Cancel();
			});
		}
	}
}