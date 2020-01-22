using Blastic.Reactive;

namespace Blastic.Forms.DynamicControls.Presenters
{
	public class TextPresenter : Presenter
	{
		public IReactiveProperty<string> Mask { get; set; }
	}
}