using System.Windows;
using Blastic.Execution;
using Blastic.Reactive;

namespace Blastic.Controls.DynamicControls
{
	public partial class Form
	{
		public static readonly DependencyProperty ExecutionContextProperty = DependencyProperty.Register(
			nameof(ExecutionContextProperty).Replace("Property", ""),
			typeof(ExecutionContext),
			typeof(Form),
			new PropertyMetadata(default(ExecutionContext)));
		public ExecutionContext ExecutionContext
		{
			get => (ExecutionContext)GetValue(ExecutionContextProperty);
			set => SetValue(ExecutionContextProperty, value);
		}

		public Command Cancel { get; }

		public Form()
		{
			InitializeComponent();

			Cancel = new Command().WithSubscribe(x =>
			{
				ExecutionContext?.Form.Value?.Cancel();
			});
		}
	}
}