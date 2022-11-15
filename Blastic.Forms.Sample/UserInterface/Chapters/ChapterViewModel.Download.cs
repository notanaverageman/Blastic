using System;
using System.IO;
using Blastic.Commanding;
using Blastic.Forms.Sample.Data;
using Blastic.Forms.Sample.UserInterface.Downloads;
using Blastic.Reactive;

namespace Blastic.Forms.Sample.UserInterface.Chapters
{
	public partial class ChapterViewModel
	{
		public class DownloadPart
		{
			private readonly Chapter _chapter;
			private readonly ChapterViewModel _parent;
			private readonly DownloadsViewModel _downloads;

			public IReactiveProperty<bool> IsDownloading { get; }
			public IReactiveProperty<bool> IsDownloaded { get; }
			public IReactiveProperty<double> DownloadProgress { get; }
			
			public AsyncCommand DownloadCommand { get; }
			public Command DeleteDownloadedFileCommand { get; }

			public DownloadPart(
				Chapter chapter,
				ChapterViewModel parent,
				DownloadsViewModel downloads,
				MediaPart mediaPart)
			{
				_chapter = chapter;
				_parent = parent;
				_downloads = downloads;

				IsDownloading = new ReactiveProperty<bool>(false);
				IsDownloaded = new ReactiveProperty<bool>(File.Exists(GetDownloadedFilePath()));
				DownloadProgress = new ReactiveProperty<double>(0);

				IObservable<bool> downloaded = IsDownloaded;
				IObservable<bool> notDownloaded = IsDownloaded.Negate();
				IObservable<bool> notDownloading = IsDownloading.Negate();
				IObservable<bool> notPlaying = mediaPart.IsPlaying.Negate();

				DownloadCommand = notDownloaded.And(notDownloading)
					.ToAsyncCommand()
					.WithSubscribe(Download);

				DeleteDownloadedFileCommand = downloaded.And(notDownloading).And(notPlaying)
					.ToCommand()
					.WithSubscribe(DeleteDownloadedFile);
			}

			private void Download()
			{
				void Queued() => IsDownloading.Value = true;
				void Completed() => IsDownloading.Value = false;
				void Succeeded()
				{
					Completed();
					IsDownloaded.Value = true;
				}
				void Cancelled()
				{
					Completed();
					IsDownloaded.Value = false;
				}

				// TODO: Show error message.
				DownloadStatusListener statusListener = new(
					queued: Queued,
					succeeded: Succeeded,
					cancelled: Cancelled,
					threwException: _ => Cancelled());

				_downloads.Queue(_parent, statusListener);
			}

			private void DeleteDownloadedFile()
			{
				string filePath = GetDownloadedFilePath();

				if (!File.Exists(filePath))
				{
					return;
				}

				File.Delete(filePath);

				IsDownloaded.Value = false;
			}

			private string GetDownloadedFilePath()
			{
				return Path.Combine(
					Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
					"Chapters",
					_parent.Book.Book.ArchiveOrgId,
					_chapter.FileName);
			}
		}
	}
}