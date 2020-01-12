using System.Collections.Generic;
using System.Windows.Input;
using Windows.UI.Xaml.Input;

namespace Blastic.Uno.Shared.Controls.DynamicControls.Elements.Action
{
	public sealed partial class ActionPresenter
	{
		public ICommand Command { get; set; }

		public ActionPresenter()
		{
			InitializeComponent();
		}

		internal void SetKeyboardAccelerators(List<KeyboardAccelerator> keyboardAccelerators)
		{
			foreach (KeyboardAccelerator keyboardAccelerator in keyboardAccelerators)
			{
				Button.KeyboardAccelerators.Add(keyboardAccelerator);
			}
		}
	}
}