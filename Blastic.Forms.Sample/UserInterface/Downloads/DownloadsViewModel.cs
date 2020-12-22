using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Blastic.Commanding;
using Blastic.Forms.Sample.Controls.Overlay;
using Blastic.Forms.Sample.Resources;
using Blastic.Forms.Sample.Services;
using Blastic.Forms.Sample.UserInterface.Chapters;
using Blastic.Platform;
using Blastic.Reactive;
using DynamicData;

namespace Blastic.Forms.Sample.UserInterface.Downloads
{
	public class DownloadsViewModel
	{
		private readonly DownloadService _downloadService;

		private readonly ReplaySubject<Unit> _available;
		private readonly SourceCache<DownloadData, string> _downloadsSource;
		private readonly ReadOnlyObservableCollection<DownloadData> _downloads;

		public LocalizableProperties LocalizableProperties { get; }

		public ReadOnlyObservableCollection<DownloadData> Downloads => _downloads;

		public IReactiveProperty<OverlayState> State { get; }

		public Command HideCommand { get; }
		public Command<DownloadData> RemoveCommand { get; }

		public DownloadsViewModel(
			DownloadService downloadService,
			LocalizableProperties localizableProperties)
		{
			_downloadService = downloadService;
			LocalizableProperties = localizableProperties;

			_available = new ReplaySubject<Unit>();

			const int concurrentDownloads = 2;

			for (int i = 0; i < concurrentDownloads; i++)
			{
				_available.OnNext(Unit.Default);
			}

			_downloadsSource = new SourceCache<DownloadData, string>(x => x.Url);
			_downloadsSource
				.Connect()
				.ObserveOnUI()
				.Bind(out _downloads)
				.Subscribe();

			_downloadsSource
				.Connect()
				.SelectMany(x => x)
				.Where(x => x.Reason == ChangeReason.Add)
				.Zip(_available, (x, _) => x.Current)
				.Subscribe(async x => await Start(x));

			State = new ReactiveProperty<OverlayState>();

			HideCommand = new Command(Hide);
			RemoveCommand = new Command<DownloadData>(Remove);
		}

		public void Queue(ChapterViewModel chapter, DownloadStatusListener statusListener)
		{
			DownloadData downloadData = new(
				chapter.Title,
				chapter.Url.Value,
				GetFilePath(chapter),
				statusListener)
			{
				Size = chapter.SizeInBytes,
				Progress = chapter.DownloadProgress
			};

			_downloadsSource.AddOrUpdate(downloadData);

			statusListener.Queued?.Invoke();
		}

		public void Show()
		{
			State.Value = OverlayState.Expanded;
		}

		public void Hide()
		{
			State.Value = OverlayState.Invisible;
		}

		private void Remove(DownloadData downloadData)
		{
			_downloadsSource.Remove(downloadData);
			downloadData.Cancel();
		}

		private async Task Start(DownloadData downloadData)
		{
			void Completed()
			{
				_downloadsSource.Remove(downloadData);
				_available.OnNext(Unit.Default);
			}
			
			if (downloadData.CancellationToken.IsCancellationRequested)
			{
				Completed();
				return;
			}
			
			try
			{
				Directory.CreateDirectory(Path.GetDirectoryName(downloadData.FilePath));

				downloadData.Progress.Value = 0;

				using FileStream stream = File.Create(downloadData.FilePath);

				await _downloadService.Download(
					downloadData.Url,
					stream,
					new Progress<double>(x => downloadData.Progress.Value = x),
					downloadData.CancellationToken);

				downloadData.StatusListener.Succeeded?.Invoke();
			}
			catch (Exception exception)
			{
				if (exception is OperationCanceledException || exception is TimeoutException)
				{
					downloadData.StatusListener.Cancelled?.Invoke();
				}
				else
				{
					downloadData.StatusListener.ThrewException?.Invoke(exception);
				}
				
				try
				{
					File.Delete(downloadData.FilePath);
				}
				catch (Exception e)
				{
					downloadData.StatusListener.ThrewException?.Invoke(e);
				}
			}

			Completed();
		}

		private string GetFilePath(ChapterViewModel chapter)
		{
			return Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				"Chapters",
				chapter.Book.Book.ArchiveOrgId,
				chapter.FileName);
		}
	}
}