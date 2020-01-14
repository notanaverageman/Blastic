using System.Windows;
using Blastic.Reactive;

namespace Blastic.DynamicControls.Presenters
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