using System.Windows;
using Reactive.Bindings;

namespace Blastic.Controls.DynamicControls.Elements.Action
{
	public class ActionElement : Element
	{
		public ReactiveCommand Command { get; }

		public ActionElement(ReactiveCommand command)
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