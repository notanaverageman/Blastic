using Blastic.Commanding;
using Blastic.Execution;
using Xamarin.Forms;
using Command = Blastic.Commanding.Command;

namespace Blastic.Forms.DynamicControls
{
	public class Form : TemplatedView
	{
		public static readonly BindableProperty ExecutionContextProperty = BindableProperty.Create(
			nameof(ExecutionContext),
			typeof(ExecutionContext),
			typeof(Form),
			default(ExecutionContext));

		public ExecutionContext ExecutionContext
		{
			get => (ExecutionContext)GetValue(ExecutionContextProperty);
			set => SetValue(ExecutionContextProperty, value);
		}

		public Command Cancel { get; }

		public Form()
		{
			Cancel = new Command().WithSubscribe(x =>
			{
				ExecutionContext?.Form.Value?.Cancel();
			});
		}
	}
}