using System.Windows.Input;
using Xamarin.Forms;

namespace Blastic.Forms.DynamicControls.Presenters
{
	public class ActionPresenter : Presenter
	{
		public static readonly BindableProperty CommandProperty = BindableProperty.Create(
			nameof(Command),
			typeof(ICommand),
			typeof(ActionPresenter),
			default(ICommand));
		public ICommand Command
		{
			get => (ICommand)GetValue(CommandProperty);
			set => SetValue(CommandProperty, value);
		}
	}
}