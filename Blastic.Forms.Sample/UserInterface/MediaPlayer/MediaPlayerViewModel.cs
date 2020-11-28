using Blastic.Commanding;
using Blastic.Forms.Sample.Controls.Overlay;
using Blastic.Reactive;

namespace Blastic.Forms.Sample.UserInterface.MediaPlayer
{
	public class MediaPlayerViewModel
	{
		public IReactiveProperty<OverlayState> OverlayState { get; }
		public IReactiveProperty<BookViewModel> CurrentBook { get; }

		public Command<OverlayState> ChangeOverlayStateCommand { get; }

		public MediaPlayerViewModel()
		{
			CurrentBook = new ReactiveProperty<BookViewModel>();
			OverlayState = new ReactiveProperty<OverlayState>();

			ChangeOverlayStateCommand = new Command<OverlayState>(
				x =>
				{
					OverlayState.Value = x;
				});
		}
	}
}