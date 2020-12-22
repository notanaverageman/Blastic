using System;
using System.IO;
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
		private readonly Chapter _chapter;

		private bool _isSeeking;

		public BookViewModel Book { get; }
		public string FileName { get; }

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

		public IReactiveProperty<bool> IsDownloaded { get; }
		public IReactiveProperty<double> DownloadProgress { get; }
		public Command DownloadCommand { get; }
		public Command DeleteDownloadedFileCommand { get; }

		public Command SeekStartedCommand { get; }
		public Command SeekCompletedCommand { get; }

		public Command TogglePlayCommand { get; }
		public Command SkipBackwardCommand { get; }
		public Command SkipForwardCommand { get; }

		public ChapterViewModel(
			DownloadsViewModel downloads,
			MediaPlayerViewModel mediaPlayer,
			BookViewModel book,
			Chapter chapter)
		{
			_mediaPlayer = mediaPlayer;
			_chapter = chapter;
			Book = book;
			FileName = chapter.FileName;

			Title = new ReactiveProperty<string>(chapter.Title);
			Url = new ReactiveProperty<string>(ToUrl(chapter.FileName));
			Duration = new ReactiveProperty<TimeSpan>(chapter.Duration);
			SizeInBytes = new ReactiveProperty<string>(ToReadableString(chapter.SizeInBytes));

			ProgressPercent = new ReactiveProperty<double>();
			Seek = new ReactiveProperty<TimeSpan>();

			IsDownloaded = new ReactiveProperty<bool>(File.Exists(GetDownloadedFilePath()));
			DownloadProgress = new ReactiveProperty<double>();

			DownloadCommand = new Command(IsDownloaded.Select(x => !x), () => downloads.Queue(this));
			DeleteDownloadedFileCommand = new Command(IsDownloaded, DeleteDownloadedFile);

			ProgressLabel = ProgressPercent
				.Select(x => ToTimeString(ToTime(x)))
				.ToReadOnlyReactiveProperty();

			RemainingLabel = ProgressPercent
				.Select(x => ToTimeString(ToTime(1 - x)))
				.ToReadOnlyReactiveProperty();

			SeekStartedCommand = new Command(SeekStarted);
			SeekCompletedCommand = new Command(SeekCompleted);

			IsPlaying = new ReactiveProperty<bool>();

			PlayPauseIconGlyph = IsPlaying
				.Select(x => x ? IconFont.PauseCircle : IconFont.PlayCircle)
				.ToReadOnlyReactiveProperty();

			TogglePlayCommand = new Command(TogglePlay);
			SkipBackwardCommand = new Command(SkipBackward);
			SkipForwardCommand = new Command(SkipForward);
		}
		
		private void DeleteDownloadedFile()
		{
			string filePath = GetDownloadedFilePath();

			if (!File.Exists(filePath))
			{
				return;
			}

			File.Delete(filePath);

			IsDownloaded.Value = false;
		}

		private string GetDownloadedFilePath()
		{
			return Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				"Chapters",
				Book.Book.ArchiveOrgId,
				_chapter.FileName);
		}

		private void SeekStarted()
		{
			_isSeeking = true;
		}

		private void SeekCompleted()
		{
			Seek.Value = ToTime(ProgressPercent.Value);
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

		private void SkipBackward()
		{
			TimeSpan seek = ToTime(ProgressPercent.Value) - TimeSpan.FromSeconds(30);

			if (seek < TimeSpan.Zero)
			{
				seek = TimeSpan.Zero;
			}

			Seek.Value = seek;

			if (!IsPlaying.Value)
			{
				UpdateProgress(Seek.Value);
			}
		}

		private void SkipForward()
		{
			TimeSpan seek = ToTime(ProgressPercent.Value) + TimeSpan.FromSeconds(30);

			if (seek > Duration.Value)
			{
				seek = Duration.Value;
			}

			Seek.Value = seek;

			if (!IsPlaying.Value)
			{
				UpdateProgress(Seek.Value);
			}
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

		private double ToPercent(TimeSpan time)
		{
			return time.TotalSeconds / Duration.Value.TotalSeconds;
		}

		private TimeSpan ToTime(double percent)
		{
			return TimeSpan.FromSeconds(percent * Duration.Value.TotalSeconds);
		}

		private string ToTimeString(TimeSpan time)
		{
			if (time.Hours > 0)
			{
				return time.ToString("hh\\:mm\\:ss");
			}

			return time.ToString("mm\\:ss");
		}
	}
}