using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Forms.Sample.Librivox;

namespace Blastic.Forms.Sample.Services
{
	public class ArchiveOrgService
	{
		private const string Mp3Format1 = "64Kbps MP3";
		private const string Mp3Format2 = "128Kbps MP3";

		public const string BaseUrl = "https://archive.org";
		public const string AudioBookSearchUrl = BaseUrl + "/advancedsearch.php?q=collection:librivoxaudio";
		public const string AudioBookMetadataUrl = BaseUrl + "/metadata";
		public const string AudioBookChapterUrl = BaseUrl + "/download";

		public const string ArchiveOrgImageUrlPrefix = "https://archive.org/services/img";

		private readonly HttpClient _httpClient;
		private readonly JsonSerializerOptions _jsonSerializerOptions;

		public ArchiveOrgService(HttpClient httpClient)
		{
			_httpClient = httpClient;

			_jsonSerializerOptions = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
		}

		public async Task<ArchiveOrgQueryResult?> GetAudioBookList(
			int page = 1,
			int numberOfItemsInPage = 20,
			CancellationToken cancellationToken = default)
		{
			string url = AudioBookSearchUrl;

			url += @"&fl[]=identifier";
			url += @"&fl[]=title";
			url += @"&fl[]=creator";
			url += @"&fl[]=description";
			url += $@"&rows={numberOfItemsInPage}";
			url += $@"&page={page}";
			url += @"&output=json";

			HttpResponseMessage responseMessage = await _httpClient.GetAsync(url, cancellationToken);

			string result = await responseMessage.Content.ReadAsStringAsync();

			ArchiveOrgQueryResult? queryResult = JsonSerializer.Deserialize<ArchiveOrgQueryResult>(
				result,
				_jsonSerializerOptions);

			return queryResult;
		}

		public async Task<ArchiveOrgMetadata?> GetAudioBookMetadata(
			string archiveOrgId,
			CancellationToken cancellationToken)
		{
			// TODO: Debug.WriteLine to proper handling.

			string url = AudioBookMetadataUrl + "/" + archiveOrgId;

			Debug.WriteLine("Id: " + archiveOrgId);

			HttpResponseMessage responseMessage = await _httpClient.GetAsync(url, cancellationToken);

			string result = await responseMessage.Content.ReadAsStringAsync();

			using JsonDocument document = JsonDocument.Parse(result);

			ArchiveOrgMetadata metadata = new ArchiveOrgMetadata();

			string? description = document.RootElement
				.GetProperty("metadata")
				.GetProperty("description")
				.GetString();

			if (description == null)
			{
				Debug.WriteLine("Description is null.");
			}
			else
			{
				metadata.Description = description;
			}

			ParseFiles(document, metadata, Mp3Format1);

			if (!metadata.Chapters.Any())
			{
				ParseFiles(document, metadata, Mp3Format2);
			}

			return metadata;
		}

		private static void ParseFiles(
			JsonDocument document,
			ArchiveOrgMetadata metadata,
			string fileFormat)
		{
			foreach (JsonElement file in document.RootElement.GetProperty("files").EnumerateArray())
			{
				string? format = file.GetProperty("format").GetString();

				if (format == null)
				{
					Debug.WriteLine("Format is null.");

					continue;
				}

				if (!string.Equals(format, fileFormat, StringComparison.InvariantCultureIgnoreCase))
				{
					continue;
				}

				ArchiveOrgChapterMetadata chapterMetadata = new ArchiveOrgChapterMetadata();

				string? title = file.GetProperty("title").GetString();

				if (title == null)
				{
					Debug.WriteLine("Title is null");
				}
				else
				{
					chapterMetadata.Title = title;
				}

				string? fileName = file.GetProperty("name").GetString();

				if (fileName == null)
				{
					Debug.WriteLine("Name is null");
				}
				else
				{
					chapterMetadata.FileName = fileName;
				}

				string? length = file.GetProperty("length").GetString();

				if (length == null)
				{
					Debug.WriteLine("Length is null");
				}
				else
				{
					if (!length.Contains(":"))
					{
						if (!double.TryParse(length, out double seconds))
						{
							Debug.WriteLine("Length is not in expected form: " + length);
						}
						else
						{
							chapterMetadata.Duration = TimeSpan.FromSeconds(seconds);
						}
					}
					else
					{
						if (!TimeSpan.TryParse(length, out TimeSpan duration))
						{
							Debug.WriteLine("Length is not in expected form: " + length);
						}
						else
						{
							chapterMetadata.Duration = duration;
						}
					}
				}

				string? size = file.GetProperty("size").GetString();

				if (size == null)
				{
					Debug.WriteLine("Size is null");
				}
				else
				{
					if (!int.TryParse(size, out int sizeInBytes))
					{
						Debug.WriteLine("Size is not in expected form: " + size);
					}
					else
					{
						chapterMetadata.SizeInBytes = sizeInBytes;
					}
				}

				string? sha1 = file.GetProperty("sha1").GetString();

				if (sha1 == null)
				{
					Debug.WriteLine("Sha1 is null");
				}
				else
				{
					chapterMetadata.Sha1 = sha1;
				}

				string? trackString = file.GetProperty("track").GetString();

				if (trackString == null)
				{
					Debug.WriteLine("Track is null");
				}
				else
				{
					int index = trackString.IndexOf('/');

					if (index < 0)
					{
						index = trackString.Length;
					}

					if (!int.TryParse(trackString.Substring(0, index), out int trackNumber))
					{
						Debug.WriteLine("Track is not in expected form: " + trackString);
					}
					else
					{
						chapterMetadata.Track = trackNumber;
					}
				}

				metadata.Chapters.Add(chapterMetadata);
			}
		}
	}
}