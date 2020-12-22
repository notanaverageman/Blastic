using System;
using System.Threading;
using Blastic.Reactive;

namespace Blastic.Forms.Sample.UserInterface
{
	public class DownloadData
	{
		private readonly CancellationTokenSource _cancellationTokenSource;
		
		public IReadOnlyReactiveProperty<string> Title { get; }
		public string Url { get; }
		public string FilePath { get; }
		
		public IReactiveProperty<string> Size { get; set; }
		public IReactiveProperty<double> Progress { get; }

		public CancellationToken CancellationToken => _cancellationTokenSource.Token;
		public Action? CompletedAction { get; set; }

		public DownloadData(IReadOnlyReactiveProperty<string> title, string url, string filePath)
		{
			Title = title;
			Url = url;
			FilePath = filePath;

			_cancellationTokenSource = new CancellationTokenSource();

			Size = new ReactiveProperty<string>();
			Progress = new ReactiveProperty<double>();
		}

		public void Cancel()
		{
			_cancellationTokenSource.Cancel();
		}
	}
}