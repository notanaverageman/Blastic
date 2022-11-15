using Blastic.Commanding;
using Blastic.Forms.Sample.Controls.Overlay;
using Blastic.Forms.Sample.Resources;
using Blastic.Reactive;

namespace Blastic.Forms.Sample.UserInterface.Chapters
{
	public class ChapterDetailsViewModel
	{
		public IReactiveProperty<ChapterViewModel?> Chapter { get; }
		public IReactiveProperty<OverlayState> State { get; }

		public LocalizableProperties LocalizableProperties { get; }

		public Command HideCommand { get; }

		public ChapterDetailsViewModel(LocalizableProperties localizableProperties)
		{
			LocalizableProperties = localizableProperties;
			
			Chapter = new ReactiveProperty<ChapterViewModel?>(default);
			State = new ReactiveProperty<OverlayState>(OverlayState.Invisible);

			HideCommand = new Command(Hide);
		}
		
		public void Show(ChapterViewModel chapter)
		{
			Chapter.Value = chapter;
			State.Value = OverlayState.Expanded;
		}

		public void Hide()
		{
			State.Value = OverlayState.Invisible;
		}
	}
}