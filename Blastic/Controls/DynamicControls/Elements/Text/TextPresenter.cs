using System.Windows;
using Reactive.Bindings;

namespace Blastic.Controls.DynamicControls.Elements.Text
{
	public class TextPresenter : Presenter
	{
		static TextPresenter()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TextPresenter), new FrameworkPropertyMetadata(typeof(TextPresenter)));
		}

		public IReactiveProperty<string> Mask { get; set; }
	}
}