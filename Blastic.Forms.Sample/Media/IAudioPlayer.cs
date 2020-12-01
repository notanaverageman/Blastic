namespace Blastic.Forms.Sample.Media
{
	public interface IAudioPlayer
	{
		void Load(string url);
		void Play();
		void Pause();
		void Stop();
	}
}