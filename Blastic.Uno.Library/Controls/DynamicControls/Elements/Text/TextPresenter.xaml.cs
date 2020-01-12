using Blastic.Reactive;

namespace Blastic.Uno.Shared.Controls.DynamicControls.Elements.Text
{
	public sealed partial class TextPresenter
	{
		public IReactiveProperty<string> Mask { get; set; }

		public TextPresenter()
		{
			InitializeComponent();
		}
	}
}