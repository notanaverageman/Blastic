using System.Reactive.Linq;
using Blastic.Commanding;
using Blastic.Forms.Sample.Controls.Overlay;
using Blastic.Forms.Sample.Resources;
using Blastic.Reactive;

namespace Blastic.Forms.Sample.UserInterface
{
	public class ChapterDetailsViewModel
	{
		private readonly DownloadsViewModel _downloads;
		
		public IReactiveProperty<ChapterViewModel?> Chapter { get; }
		public IReactiveProperty<OverlayState> State { get; }

		public LocalizableProperties LocalizableProperties { get; }

		public Command HideCommand { get; }
		public Command DownloadCommand { get; }

		public ChapterDetailsViewModel(
			DownloadsViewModel downloads,
			LocalizableProperties localizableProperties)
		{
			_downloads = downloads;
			LocalizableProperties = localizableProperties;
			
			Chapter = new ReactiveProperty<ChapterViewModel?>();
			State = new ReactiveProperty<OverlayState>();

			HideCommand = new Command(Hide);
			DownloadCommand = new Command(Download);
		}

		private void Download()
		{
			ChapterViewModel? chapter = Chapter.Value;

			if (chapter == null)
			{
				return;
			}
			
			_downloads.Queue(chapter);
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