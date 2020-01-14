using System.Windows;
using System.Windows.Controls;
using Blastic.Commanding;
using Blastic.Execution;

namespace Blastic.Wpf.Controls
{
	public class BusyIndicator : Control
	{
		static BusyIndicator()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BusyIndicator), new FrameworkPropertyMetadata(typeof(BusyIndicator)));
		}

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
			Cancel = new Command().WithSubscribe(x =>
			{
				ExecutionContext?.CancellationTokenSource?.Cancel();
			});
		}
	}
}