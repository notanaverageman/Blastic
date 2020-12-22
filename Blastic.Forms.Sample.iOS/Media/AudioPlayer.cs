using System;
using System.Diagnostics;
using AVFoundation;
using Blastic.Commanding;
using Blastic.Forms.Sample.Media;
using Blastic.Forms.Sample.UserInterface.Chapters;
using Blastic.Reactive;
using CoreFoundation;
using CoreMedia;
using Foundation;
using MediaPlayer;

namespace Blastic.Forms.Sample.iOS.Media
{
	public class AudioPlayer : IAudioPlayer
	{
		private readonly AVPlayer _player;
		private bool _notifyProgress;

		public IReactiveProperty<TimeSpan> Progress { get; }

		public Command RemotePlayCommand { get; }
		public Command RemotePauseCommand { get; }
		public Command RemoteStopCommand { get; }

		public Command SkipBackwardCommand { get; }
		public Command SkipForwardCommand { get; }

		public AudioPlayer()
		{
			Progress = new ReactiveProperty<TimeSpan>();

			RemotePlayCommand = new Command();
			RemotePauseCommand = new Command();
			RemoteStopCommand = new Command();

			SkipBackwardCommand = new Command();
			SkipForwardCommand = new Command();

			_notifyProgress = true;

			_player = new AVPlayer();

			_player.AddPeriodicTimeObserver(
				CMTime.FromSeconds(1, 1),
				DispatchQueue.CurrentQueue,
				UpdateProgress);

			InitializeCommandCenter();
		}

		public void Load(ChapterViewModel chapter)
		{
			AVAudioSession audioSession = AVAudioSession.SharedInstance();

			NSError error = audioSession.SetCategory(AVAudioSessionCategory.Playback);

			if (error != null)
			{
				Debug.WriteLine("Error audioSession.SetCategory: " + error.Description);
				return;
			}

			error = audioSession.SetActive(true, AVAudioSessionSetActiveOptions.NotifyOthersOnDeactivation);

			if (error != null)
			{
				Debug.WriteLine("Error audioSession.SetActive: " + error.Description);
				return;
			}

			NSUrl url = NSUrl.FromString(chapter.Url.Value);
			AVPlayerItem playerItem = AVPlayerItem.FromUrl(url);

			Stop();
			_player.ReplaceCurrentItemWithPlayerItem(playerItem);

			InitializeNowPlayingInfo(chapter);
		}

		private void InitializeCommandCenter()
		{
			MPRemoteCommandCenter commandCenter = MPRemoteCommandCenter.Shared;

			void AddTarget(MPRemoteCommand remoteCommand, Command command)
			{
				remoteCommand.AddTarget(
					x =>
					{
						command.Execute();
						return MPRemoteCommandHandlerStatus.Success;
					});
			}

			AddTarget(commandCenter.PlayCommand, RemotePlayCommand);
			AddTarget(commandCenter.PauseCommand, RemotePauseCommand);
			AddTarget(commandCenter.StopCommand, RemoteStopCommand);
			AddTarget(commandCenter.SkipBackwardCommand, SkipBackwardCommand);
			AddTarget(commandCenter.SkipForwardCommand, SkipForwardCommand);

			commandCenter.SkipBackwardCommand.PreferredIntervals = new []{ 30d };
			commandCenter.SkipForwardCommand.PreferredIntervals = new []{ 30d };
		}

		private void InitializeNowPlayingInfo(ChapterViewModel chapter)
		{
			MPNowPlayingInfo nowPlayingInfo = new MPNowPlayingInfo
			{
				Title = chapter.Title.Value,
				AlbumTitle = chapter.Book.Title.Value,
				Artist = chapter.Book.Author.Value,
				PlaybackDuration = chapter.Duration.Value.TotalSeconds,
				PlaybackRate = _player.Rate
			};

			MPNowPlayingInfoCenter.DefaultCenter.NowPlaying = nowPlayingInfo;
		}

		public void Play()
		{
			_player.Play();
		}

		public void Pause()
		{
			_player.Pause();
		}

		public void Stop()
		{
			_player.Pause();
			_player.Seek(CMTime.Zero);

			_player.ReplaceCurrentItemWithPlayerItem(null);
		}

		public void Seek(TimeSpan time)
		{
			_notifyProgress = false;
			_player.Seek(
				CMTime.FromSeconds(time.TotalSeconds, 1),
				x => _notifyProgress = true);
		}

		private void UpdateProgress(CMTime time)
		{
			if (!_notifyProgress)
			{
				return;
			}

			Progress.Value = TimeSpan.FromSeconds(time.Seconds);
		}
	}
}