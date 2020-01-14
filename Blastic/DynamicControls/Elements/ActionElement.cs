using System.Windows.Input;
using Blastic.DynamicControls.Properties;

namespace Blastic.DynamicControls.Elements
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
			HorizontalAlignment = Properties.HorizontalAlignment.Right;
		}
	}
}