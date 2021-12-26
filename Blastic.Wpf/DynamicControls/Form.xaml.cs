using System;
using System.Windows;
using Blastic.Commanding;
using Blastic.Execution;

namespace Blastic.Wpf.DynamicControls
{
	public partial class Form
	{
		private IDisposable _formSubscription;

		public static readonly DependencyProperty ExecutionContextProperty = DependencyProperty.Register(
			nameof(ExecutionContextProperty).Replace("Property", ""),
			typeof(ExecutionContext),
			typeof(Form),
			new PropertyMetadata(default(ExecutionContext), OnExecutionContextChanged));

		public ExecutionContext ExecutionContext
		{
			get => (ExecutionContext)GetValue(ExecutionContextProperty);
			set => SetValue(ExecutionContextProperty, value);
		}

		public AsyncCommand Ok { get; }
		public AsyncCommand Cancel { get; }

		public Form()
		{
			InitializeComponent();

			Ok = new AsyncCommand().WithSubscribe(x =>
			{
				ExecutionContext?.Form.Value?.Ok();
			});

			Cancel = new AsyncCommand().WithSubscribe(x =>
			{
				ExecutionContext?.Form.Value?.Cancel();
			});
		}

		private static void OnExecutionContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Form form = (Form) d;
			
			form._formSubscription?.Dispose();

			form._formSubscription = ((ExecutionContext) e.NewValue)?.Form?.Subscribe(x =>
			{
				if (x?.MinWidth == 0)
				{
					x.MinWidth = 400;
				}
			});
		}
	}
}