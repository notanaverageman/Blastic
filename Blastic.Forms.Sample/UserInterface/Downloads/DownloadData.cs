using System.Threading;
using Blastic.Reactive;

namespace Blastic.Forms.Sample.UserInterface.Downloads
{
	public class DownloadData
	{
		private readonly CancellationTokenSource _cancellationTokenSource;
		
		public IReadOnlyReactiveProperty<string> Title { get; }
		public string Url { get; }
		public string FilePath { get; }
		
		public IReactiveProperty<string> Size { get; set; }
		public IReactiveProperty<double> Progress { get; set; }

		public DownloadStatusListener StatusListener { get; }
		public CancellationToken CancellationToken => _cancellationTokenSource.Token;

		public DownloadData(
			IReadOnlyReactiveProperty<string> title,
			string url,
			string filePath,
			DownloadStatusListener statusListener)
		{
			Title = title;
			Url = url;
			FilePath = filePath;
			StatusListener = statusListener;

			_cancellationTokenSource = new CancellationTokenSource();

			Size = new ReactiveProperty<string>("");
			Progress = new ReactiveProperty<double>(0);
		}

		public void Cancel()
		{
			_cancellationTokenSource.Cancel();
			StatusListener.Cancelled?.Invoke();
		}
	}
}