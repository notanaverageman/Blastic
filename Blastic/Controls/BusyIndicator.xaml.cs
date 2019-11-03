using System.Windows;
using Blastic.Execution;
using Reactive.Bindings;

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

		public ReactiveCommand Cancel { get; set; }

		public BusyIndicator()
		{
			InitializeComponent();

			Cancel = new ReactiveCommand().WithSubscribe(() => ExecutionContext?.CancellationTokenSource?.Cancel());
		}
	}
}