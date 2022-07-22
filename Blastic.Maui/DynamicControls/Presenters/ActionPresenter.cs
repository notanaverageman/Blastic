using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace Blastic.Maui.DynamicControls.Presenters;

public class ActionPresenter : Presenter
{
	public static readonly BindableProperty CommandProperty = BindableProperty.Create(
		nameof(Command),
		typeof(ICommand),
		typeof(ActionPresenter));
	public ICommand Command
	{
		get => (ICommand)GetValue(CommandProperty);
		set => SetValue(CommandProperty, value);
	}
}