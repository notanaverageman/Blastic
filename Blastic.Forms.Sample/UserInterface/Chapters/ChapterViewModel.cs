using Blastic.Forms.Sample.Data;
using Blastic.Forms.Sample.UserInterface.Books;
using Blastic.Forms.Sample.UserInterface.Downloads;
using Blastic.Forms.Sample.UserInterface.MediaPlayer;
using Blastic.Reactive;

namespace Blastic.Forms.Sample.UserInterface.Chapters
{
	public partial class ChapterViewModel
	{
		public MediaPart Media { get; }
		public DownloadPart Download { get; }
		
		public BookViewModel Book { get; }
		public string FileName { get; }

		public IReactiveProperty<string> Title { get; }
		
		public ChapterViewModel(
			DownloadsViewModel downloads,
			MediaPlayerViewModel mediaPlayer,
			BookViewModel book,
			Chapter chapter)
		{
			Book = book;
			FileName = chapter.FileName;

			Title = new ReactiveProperty<string>(chapter.Title);

			Media = new MediaPart(chapter, this, mediaPlayer);
			Download = new DownloadPart(chapter, this, downloads, Media);
		}
	}
}