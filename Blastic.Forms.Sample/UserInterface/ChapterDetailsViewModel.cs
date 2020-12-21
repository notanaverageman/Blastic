using Blastic.Commanding;
using Blastic.Forms.Sample.Controls.Overlay;
using Blastic.Forms.Sample.Resources;
using Blastic.Reactive;

namespace Blastic.Forms.Sample.UserInterface
{
	public class ChapterDetailsViewModel
	{
		public IReactiveProperty<ChapterViewModel> Chapter { get; }
		public IReactiveProperty<OverlayState> State { get; }

		public LocalizableProperties LocalizableProperties { get; }

		public Command<ChapterViewModel> HideDetailsCommand { get; }

		public ChapterDetailsViewModel(LocalizableProperties localizableProperties)
		{
			LocalizableProperties = localizableProperties;
			
			Chapter = new ReactiveProperty<ChapterViewModel>();
			State = new ReactiveProperty<OverlayState>();

			HideDetailsCommand = new Command<ChapterViewModel>(HideDetails);
		}
		
		public void ShowDetails(ChapterViewModel chapter)
		{
			Chapter.Value = chapter;
			State.Value = OverlayState.Expanded;
		}

		public void HideDetails()
		{
			State.Value = OverlayState.Invisible;
		}
	}
}