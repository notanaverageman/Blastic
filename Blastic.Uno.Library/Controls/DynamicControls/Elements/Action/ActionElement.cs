using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Blastic.Uno.Shared.Controls.DynamicControls.Elements.Action;

namespace Blastic.Controls.DynamicControls.Elements.Action
{
	public class ActionElement : Element
	{
		private readonly List<KeyboardAccelerator> _keyboardAccelerators;

		public ICommand Command { get; }

		public ActionElement(ICommand command)
		{
			_keyboardAccelerators = new List<KeyboardAccelerator>(0);

			Command = command;

			Margin = new Thickness(2);
			Padding = new Thickness(8, 2, 8, 2);
			IconMargin = new Thickness(0);
			HorizontalAlignment = HorizontalAlignment.Right;
		}

		protected override Presenter CreatePresenter()
		{
			ActionPresenter presenter = new ActionPresenter
			{
				Command = Command
			};

			if (_keyboardAccelerators.Any())
			{
				presenter.SetKeyboardAccelerators(_keyboardAccelerators);
			}

			return presenter;
		}

		public ActionElement WithKeyboardAccelerator(KeyboardAccelerator keyboardAccelerator)
		{
			_keyboardAccelerators.Add(keyboardAccelerator);
			return this;
		}
	}
}