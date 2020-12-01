using System;
using Blastic.Commanding;
using Blastic.Forms.Sample.UserInterface;
using Blastic.Reactive;

namespace Blastic.Forms.Sample.Media
{
	public interface IAudioPlayer
	{
		IReactiveProperty<TimeSpan> Progress { get; }

		Command RemotePlayCommand { get; }
		Command RemotePauseCommand { get; }
		Command RemoteStopCommand { get; }

		void Load(ChapterViewModel chapter);
		void Play();
		void Pause();
		void Stop();

		void Seek(TimeSpan time);
	}
}