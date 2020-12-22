using System;
using Blastic.Commanding;
using Blastic.Forms.Sample.Controls.Overlay;
using Blastic.Forms.Sample.Media;
using Blastic.Forms.Sample.UserInterface.Chapters;
using Blastic.Reactive;

namespace Blastic.Forms.Sample.UserInterface.MediaPlayer
{
	public class MediaPlayerViewModel
	{
		private readonly IAudioPlayer _audioPlayer;
		private IDisposable? _seekSubscription;
		
		public IReactiveProperty<OverlayState> OverlayState { get; }
		public IReactiveProperty<ChapterViewModel?> CurrentChapter { get; }

		public Command<OverlayState> ChangeOverlayStateCommand { get; }

		public MediaPlayerViewModel(IAudioPlayer audioPlayer)
		{
			_audioPlayer = audioPlayer;

			OverlayState = new ReactiveProperty<OverlayState>();
			CurrentChapter = new ReactiveProperty<ChapterViewModel?>();

			ChangeOverlayStateCommand = new Command<OverlayState>(
				x =>
				{
					OverlayState.Value = x;
				});

			_audioPlayer.RemotePlayCommand.Subscribe(() => PlayChapter(CurrentChapter.Value));
			_audioPlayer.RemotePauseCommand.Subscribe(Pause);
			_audioPlayer.RemoteStopCommand.Subscribe(Stop);

			_audioPlayer.SkipBackwardCommand.Subscribe(SkipBackward);
			_audioPlayer.SkipForwardCommand.Subscribe(SkipForward);

			_audioPlayer.Progress.Subscribe(UpdateProgress);
		}

		public void PlayChapter(ChapterViewModel? chapter)
		{
			ChapterViewModel? currentChapter = CurrentChapter.Value;

			// Current chapter resumes from pause state.
			if (currentChapter != null && currentChapter == chapter)
			{
				_audioPlayer.Play();
				chapter.Media.IsPlaying.Value = true;

				return;
			}

			// Play request from another chapter. Stop the previous chapter.
			if (currentChapter != null)
			{
				_audioPlayer.Stop();
				_seekSubscription?.Dispose();

				currentChapter.Media.IsPlaying.Value = false;
			}

			CurrentChapter.Value = chapter;

			if (chapter == null)
			{
				return;
			}

			_audioPlayer.Load(chapter);
			_audioPlayer.Play();

			chapter.Media.IsPlaying.Value = true;

			_seekSubscription = chapter.Media.SeekTime.Subscribe(x => _audioPlayer.Seek(x));

			OverlayState.Value = Controls.Overlay.OverlayState.Collapsed;
		}

		public void Pause()
		{
			ChapterViewModel? currentChapter = CurrentChapter.Value;

			if (currentChapter == null)
			{
				return;
			}

			_audioPlayer.Pause();
			currentChapter.Media.IsPlaying.Value = false;
		}

		public void Stop()
		{
			ChapterViewModel? currentChapter = CurrentChapter.Value;

			if (currentChapter == null)
			{
				return;
			}

			_audioPlayer.Stop();
			currentChapter.Media.IsPlaying.Value = false;
		}

		private void SkipBackward()
		{
			CurrentChapter.Value?.Media.SkipBackwardCommand.Execute();
		}

		private void SkipForward()
		{
			CurrentChapter.Value?.Media.SkipForwardCommand.Execute();
		}

		private void UpdateProgress(TimeSpan progress)
		{
			CurrentChapter.Value?.Media.UpdateProgress(progress);
		}
	}
}