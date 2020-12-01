using System;
using Blastic.Forms.Sample.Data;
using Blastic.Forms.Sample.Services;
using Blastic.Reactive;

namespace Blastic.Forms.Sample.UserInterface
{
	public class ChapterViewModel
	{
		public BookViewModel Book { get; }

		public IReactiveProperty<string> Title { get; }
		public IReactiveProperty<string> Url { get; }
		public IReactiveProperty<TimeSpan> Duration { get; }
		public IReactiveProperty<string> SizeInBytes { get; }

		public ChapterViewModel(BookViewModel book, Chapter chapter)
		{
			Book = book;

			Title = new ReactiveProperty<string>(chapter.Title);
			Url = new ReactiveProperty<string>(ToUrl(chapter.FileName));
			Duration = new ReactiveProperty<TimeSpan>(chapter.Duration);
			SizeInBytes = new ReactiveProperty<string>(ToReadableString(chapter.SizeInBytes));
		}

		private string ToUrl(string fileName)
		{
			return ArchiveOrgService.AudioBookChapterUrl + "/" + Book.Book.ArchiveOrgId + "/" + fileName;
		}

		private string ToReadableString(int size)
		{
			const int kb = 1024;
			const int mb = 1024 * kb;

			if (size < kb)
			{
				return "1 KB";
			}

			if (size < mb)
			{
				return size / kb + " KB";
			}

			return size / mb + " MB";
		}
	}
}