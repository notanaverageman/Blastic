using System.Diagnostics;
using AVFoundation;
using Blastic.Forms.Sample.Media;
using CoreMedia;
using Foundation;
using MediaPlayer;

namespace Blastic.Forms.Sample.iOS.Media
{
	public class AudioPlayer : IAudioPlayer
	{
		private AVPlayer _player;

		public void Load(string url)
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

			NSUrl nsUrl = NSUrl.FromString(url);
			AVPlayerItem playerItem = AVPlayerItem.FromUrl(nsUrl);

			if (_player == null)
			{
				_player = AVPlayer.FromPlayerItem(playerItem);

				InitializeCommandCenter();
			}
			else
			{
				Pause();
				_player.ReplaceCurrentItemWithPlayerItem(playerItem);
			}

			MPNowPlayingInfo nowPlayingInfo = new MPNowPlayingInfo
			{
				Title = "Test",
				AlbumTitle = "Test Album",
			};

			MPNowPlayingInfoCenter.DefaultCenter.NowPlaying = nowPlayingInfo;
		}

		private void InitializeCommandCenter()
		{
			MPRemoteCommandCenter commandCenter = MPRemoteCommandCenter.Shared;

			commandCenter.PlayCommand.AddTarget(
				x =>
				{
					Debug.WriteLine("Play command");
					Play();
					return MPRemoteCommandHandlerStatus.Success;
				});

			commandCenter.PauseCommand.AddTarget(
				x =>
				{
					Debug.WriteLine("Pause command");
					Pause();
					return MPRemoteCommandHandlerStatus.Success;
				});

			commandCenter.StopCommand.AddTarget(
				x =>
				{
					Debug.WriteLine("Stop command");
					Stop();
					return MPRemoteCommandHandlerStatus.Success;
				});
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
	}
}