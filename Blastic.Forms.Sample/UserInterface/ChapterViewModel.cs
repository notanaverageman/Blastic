using System;
using System.Reactive.Linq;
using Blastic.Commanding;
using Blastic.Forms.Sample.Data;
using Blastic.Forms.Sample.Icons;
using Blastic.Forms.Sample.Services;
using Blastic.Forms.Sample.UserInterface.MediaPlayer;
using Blastic.Reactive;

namespace Blastic.Forms.Sample.UserInterface
{
	public class ChapterViewModel
	{
		private readonly MediaPlayerViewModel _mediaPlayer;

		private bool _isSeeking;

		public BookViewModel Book { get; }

		public IReactiveProperty<string> Title { get; }
		public IReactiveProperty<string> Url { get; }
		public IReactiveProperty<TimeSpan> Duration { get; }
		public IReactiveProperty<string> SizeInBytes { get; }

		public IReactiveProperty<double> ProgressPercent { get; }
		public IReactiveProperty<TimeSpan> Seek { get; }

		public IReadOnlyReactiveProperty<string> ProgressLabel { get; }
		public IReadOnlyReactiveProperty<string> RemainingLabel { get; }

		public IReactiveProperty<bool> IsPlaying { get; }
		public IReadOnlyReactiveProperty<string> PlayPauseIconGlyph { get; }

		public Command SeekStartedCommand { get; }
		public Command SeekCompletedCommand { get; }

		public Command TogglePlayCommand { get; }

		public ChapterViewModel(
			MediaPlayerViewModel mediaPlayer,
			BookViewModel book,
			Chapter chapter)
		{
			_mediaPlayer = mediaPlayer;
			Book = book;

			Title = new ReactiveProperty<string>(chapter.Title);
			Url = new ReactiveProperty<string>(ToUrl(chapter.FileName));
			Duration = new ReactiveProperty<TimeSpan>(chapter.Duration);
			SizeInBytes = new ReactiveProperty<string>(ToReadableString(chapter.SizeInBytes));

			ProgressPercent = new ReactiveProperty<double>();
			Seek = new ReactiveProperty<TimeSpan>();

			ProgressLabel = ProgressPercent
				.Select(ToTimeString)
				.ToReadOnlyReactiveProperty();

			RemainingLabel = ProgressPercent
				.Select(x => ToTimeString(1 - x))
				.ToReadOnlyReactiveProperty();

			SeekStartedCommand = new Command(SeekStarted);
			SeekCompletedCommand = new Command(SeekCompleted);

			IsPlaying = new ReactiveProperty<bool>();

			PlayPauseIconGlyph = IsPlaying
				.Select(x => x ? IconFont.PauseCircle : IconFont.PlayCircle)
				.ToReadOnlyReactiveProperty();

			TogglePlayCommand = new Command(TogglePlay);
		}

		private void SeekStarted()
		{
			_isSeeking = true;
		}

		private void SeekCompleted()
		{
			TimeSpan progress = TimeSpan.FromSeconds(Duration.Value.TotalSeconds * ProgressPercent.Value);
			Seek.Value = progress;

			_isSeeking = false;
		}

		private void TogglePlay()
		{
			if (IsPlaying.Value)
			{
				_mediaPlayer.Pause();
			}
			else
			{
				_mediaPlayer.PlayChapter(this);
			}
		}

		public void UpdateProgress(TimeSpan progress)
		{
			if (_isSeeking)
			{
				return;
			}

			ProgressPercent.Value = ToPercent(progress);
		}

		private string ToUrl(string fileName)
		{
			return ArchiveOrgService.AudioBookChapterUrl + "/" + Book.Book.ArchiveOrgId + "/" + fileName;
		}

		private string ToReadableString(int size)
		{
			const int kb = 1024;
			const int mb = 1024 * kb;

			if (size < kb)
			{
				return "1 KB";
			}

			if (size < mb)
			{
				return size / kb + " KB";
			}

			return size / mb + " MB";
		}

		private double ToPercent(TimeSpan x)
		{
			return x.TotalSeconds / Duration.Value.TotalSeconds;
		}

		private string ToTimeString(double x)
		{
			TimeSpan time = TimeSpan.FromSeconds(x * Duration.Value.TotalSeconds);

			if (time.Hours > 0)
			{
				return time.ToString("hh\\:mm\\:ss");
			}

			return time.ToString("mm\\:ss");
		}
	}
}