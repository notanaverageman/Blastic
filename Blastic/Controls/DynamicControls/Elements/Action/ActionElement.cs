using System.Windows;
using System.Windows.Input;

namespace Blastic.Controls.DynamicControls.Elements.Action
{
	public class ActionElement : Element
	{
		public ICommand Command { get; }

		public ActionElement(ICommand command)
		{
			Command = command;

			Margin = new Thickness(2);
			Padding = new Thickness(8, 2, 8, 2);
			IconMargin = new Thickness(0);
			HorizontalAlignment = HorizontalAlignment.Right;
		}

		protected override Presenter CreatePresenter()
		{
			return new ActionPresenter
			{
				Command = Command
			};
		}
	}
}