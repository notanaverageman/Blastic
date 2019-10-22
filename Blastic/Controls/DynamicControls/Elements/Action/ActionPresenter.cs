using System.Windows;
using Reactive.Bindings;

namespace Blastic.Controls.DynamicControls.Elements.Action
{
	public class ActionPresenter : Presenter
	{
		public ReactiveCommand Command { get; set; }

		static ActionPresenter()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ActionPresenter), new FrameworkPropertyMetadata(typeof(ActionPresenter)));
		}
	}
}