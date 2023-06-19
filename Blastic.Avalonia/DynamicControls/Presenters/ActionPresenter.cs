using System.Windows.Input;
using Avalonia;

namespace Blastic.Avalonia.DynamicControls.Presenters;

public class ActionPresenter : Presenter
{
	public static readonly AvaloniaProperty CommandProperty = AvaloniaProperty.Register<ActionPresenter, ICommand>(nameof(Command));

	public ICommand? Command
	{
		get => (ICommand?)GetValue(CommandProperty);
		set => SetValue(CommandProperty, value);
	}
}