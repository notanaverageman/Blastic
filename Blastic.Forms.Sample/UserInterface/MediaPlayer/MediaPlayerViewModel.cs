using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Blastic.Commanding;
using Blastic.Forms.Sample.Controls.Overlay;
using Blastic.Forms.Sample.Media;
using Blastic.Reactive;

namespace Blastic.Forms.Sample.UserInterface.MediaPlayer
{
	public class MediaPlayerViewModel
	{
		private readonly HttpClient _httpClient;
		private readonly IAudioPlayer _audioPlayer;

		public IReactiveProperty<OverlayState> OverlayState { get; }
		public IReactiveProperty<ChapterViewModel> CurrentChapter { get; }

		public IReactiveProperty<TimeSpan> Position { get; }
		public IReadOnlyReactiveProperty<double> PositionPercent { get; }

		public Command<OverlayState> ChangeOverlayStateCommand { get; }

		public MediaPlayerViewModel(
			HttpClient httpClient,
			IAudioPlayer audioPlayer)
		{
			_httpClient = httpClient;
			_audioPlayer = audioPlayer;

			OverlayState = new ReactiveProperty<OverlayState>();
			CurrentChapter = new ReactiveProperty<ChapterViewModel>();

			Position = new ReactiveProperty<TimeSpan>();

			PositionPercent = Position
				.Select(ToPercent)
				.ToReadOnlyReactiveProperty();

			ChangeOverlayStateCommand = new Command<OverlayState>(
				x =>
				{
					OverlayState.Value = x;
				});

			Observable
				.Interval(TimeSpan.FromSeconds(1))
				.Subscribe(
					x =>
					{
						//Position.Value = TimeSpan.FromSeconds(_audioPlayer.CurrentPosition);
					});
		}

		public async Task PlayChapter(ChapterViewModel chapter)
		{
			CurrentChapter.Value = chapter;

			string url = chapter.Url.Value;

			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, url);
			HttpResponseMessage response = await _httpClient.SendAsync(request);

			if (!response.IsSuccessStatusCode)
			{
				// TODO: Error handling.
				Debug.WriteLine("Chapter URL " + url + " error: " + response.StatusCode);
				return;
			}

			_audioPlayer.Load(url);
			_audioPlayer.Play();
		}

		private double ToPercent(TimeSpan x)
		{
			TimeSpan? duration = CurrentChapter.Value?.Duration.Value;

			if (duration == null)
			{
				return 0;
			}

			return x.TotalSeconds / duration.Value.TotalSeconds;
		}
	}
}