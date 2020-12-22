using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Blastic.Commanding;
using Blastic.Forms.Sample.Controls.Overlay;
using Blastic.Forms.Sample.Resources;
using Blastic.Forms.Sample.Services;
using Blastic.Platform;
using Blastic.Reactive;
using DynamicData;

namespace Blastic.Forms.Sample.UserInterface
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

		public void Queue(ChapterViewModel chapter)
		{
			DownloadData downloadData = new(
				chapter.Title,
				chapter.Url.Value,
				GetFilePath(chapter))
			{
				Size = chapter.SizeInBytes,
				CompletedAction = () => chapter.IsDownloaded.Value = true
			};

			_downloadsSource.AddOrUpdate(downloadData);
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

			void CleanUp()
			{
				try
				{
					File.Delete(downloadData.FilePath);
				}
				catch (Exception e)
				{
					Debug.WriteLine(e);
				}
			}

			if (downloadData.CancellationToken.IsCancellationRequested)
			{
				Completed();
				return;
			}
			
			string url = downloadData.Url;
			IReactiveProperty<double> progress = downloadData.Progress;

			Directory.CreateDirectory(Path.GetDirectoryName(downloadData.FilePath));

			progress.Value = 0;

			try
			{
				using FileStream stream = File.Create(downloadData.FilePath);

				await _downloadService.Download(
					url,
					stream,
					new Progress<double>(x => progress.Value = x),
					downloadData.CancellationToken);

				downloadData.CompletedAction?.Invoke();
			}
			catch (Exception exception)
				when(
					exception is OperationCanceledException ||
					exception is TimeoutException)
			{
				CleanUp();
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