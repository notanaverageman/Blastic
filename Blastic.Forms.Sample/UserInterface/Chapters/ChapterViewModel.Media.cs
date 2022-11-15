using System;
using System.Reactive.Linq;
using Blastic.Commanding;
using Blastic.Forms.Sample.Data;
using Blastic.Forms.Sample.Services;
using Blastic.Forms.Sample.UserInterface.MediaPlayer;
using Blastic.Reactive;

namespace Blastic.Forms.Sample.UserInterface.Chapters
{
	public partial class ChapterViewModel
	{
		public class MediaPart
		{
			private readonly ChapterViewModel _parent;
			private readonly MediaPlayerViewModel _mediaPlayer;
			private bool _isSeeking;

			public IReactiveProperty<string> Url { get; }
			public IReactiveProperty<TimeSpan> Duration { get; }
			public IReactiveProperty<string> SizeLabel { get; }

			public IReactiveProperty<double> PlayedPercent { get; }
			public IReactiveProperty<TimeSpan> SeekTime { get; }

			public IReadOnlyReactiveProperty<string> PlayedDurationLabel { get; }
			public IReadOnlyReactiveProperty<string> RemainingDurationLabel { get; }

			public IReactiveProperty<bool> IsPlaying { get; }

			public Command SeekStartedCommand { get; }
			public Command SeekCompletedCommand { get; }

			public Command TogglePlayCommand { get; }
			public Command SkipBackwardCommand { get; }
			public Command SkipForwardCommand { get; }

			public MediaPart(
				Chapter chapter,
				ChapterViewModel parent,
				MediaPlayerViewModel mediaPlayer)
			{
				_parent = parent;
				_mediaPlayer = mediaPlayer;
				Url = new ReactiveProperty<string>(ToUrl(chapter.FileName));
				Duration = new ReactiveProperty<TimeSpan>(chapter.Duration);
				SizeLabel = new ReactiveProperty<string>(ToReadableString(chapter.SizeInBytes));

				PlayedPercent = new ReactiveProperty<double>(0);
				SeekTime = new ReactiveProperty<TimeSpan>(default);

				PlayedDurationLabel = PlayedPercent
					.Select(x => ToTimeString(ToTime(x)))
					.ToReadOnlyReactiveProperty("");

				RemainingDurationLabel = PlayedPercent
					.Select(x => ToTimeString(ToTime(1 - x)))
					.ToReadOnlyReactiveProperty("");

				SeekStartedCommand = new Command(SeekStarted);
				SeekCompletedCommand = new Command(SeekCompleted);

				IsPlaying = new ReactiveProperty<bool>(false);

				TogglePlayCommand = new Command(TogglePlay);
				SkipBackwardCommand = new Command(SkipBackward);
				SkipForwardCommand = new Command(SkipForward);
			}

			private void SeekStarted()
			{
				_isSeeking = true;
			}

			private void SeekCompleted()
			{
				SeekTime.Value = ToTime(PlayedPercent.Value);
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
					_mediaPlayer.PlayChapter(_parent);
				}
			}

			public void UpdateProgress(TimeSpan progress)
			{
				if (_isSeeking)
				{
					return;
				}

				PlayedPercent.Value = ToPercent(progress);
			}

			private void SkipBackward()
			{
				TimeSpan seek = ToTime(PlayedPercent.Value) - TimeSpan.FromSeconds(30);

				if (seek < TimeSpan.Zero)
				{
					seek = TimeSpan.Zero;
				}

				SeekTime.Value = seek;

				if (!IsPlaying.Value)
				{
					UpdateProgress(SeekTime.Value);
				}
			}

			private void SkipForward()
			{
				TimeSpan seek = ToTime(PlayedPercent.Value) + TimeSpan.FromSeconds(30);

				if (seek > Duration.Value)
				{
					seek = Duration.Value;
				}

				SeekTime.Value = seek;

				if (!IsPlaying.Value)
				{
					UpdateProgress(SeekTime.Value);
				}
			}

			private string ToUrl(string fileName)
			{
				return ArchiveOrgService.AudioBookChapterUrl + "/" + _parent.Book.Book.ArchiveOrgId + "/" + fileName;
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
		}
	}
}