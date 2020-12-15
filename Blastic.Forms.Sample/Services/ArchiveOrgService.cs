using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Forms.Sample.ArchiveOrg;

namespace Blastic.Forms.Sample.Services
{
	// TODO: Debug.WriteLine to proper handling.
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

		public ArchiveOrgService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<ArchiveOrgQueryResult> GetAudioBookList(
			int page = 1,
			int numberOfItemsInPage = 20,
			CancellationToken cancellationToken = default)
		{
			string url = $"{AudioBookSearchUrl}";
			url += @"&sort[]=downloads+desc";
			
			return await GetAudioBookList(url , page, numberOfItemsInPage, cancellationToken);
		}

		public async Task<ArchiveOrgQueryResult> Search(
			string title,
			int page = 1,
			int numberOfItemsInPage = 20,
			CancellationToken cancellationToken = default)
		{
			string url = $"{AudioBookSearchUrl} AND title:({title})";
			return await GetAudioBookList(url, page, numberOfItemsInPage, cancellationToken);
		}

		public async Task<ArchiveOrgMetadata?> GetAudioBookMetadata(
			string archiveOrgId,
			CancellationToken cancellationToken)
		{
			string url = AudioBookMetadataUrl + "/" + archiveOrgId;
			
			HttpResponseMessage responseMessage = await _httpClient.GetAsync(url, cancellationToken);
			string result = await responseMessage.Content.ReadAsStringAsync();

			using JsonDocument document = JsonDocument.Parse(result);

			ArchiveOrgMetadata metadata = new()
			{
				Description = GetStringProperty(document.RootElement.GetProperty("metadata"), "description")
			};
			
			ParseFiles(document, metadata, Mp3Format1);

			if (!metadata.Chapters.Any())
			{
				ParseFiles(document, metadata, Mp3Format2);
			}

			return metadata;
		}

		public async Task<ArchiveOrgQueryResult> GetAudioBookList(
			string url,
			int page = 1,
			int numberOfItemsInPage = 20,
			CancellationToken cancellationToken = default)
		{
			url += @"&fl[]=identifier";
			url += @"&fl[]=title";
			url += @"&fl[]=creator";
			url += @"&fl[]=description";
			url += @"&fl[]=downloads";
			url += @"&fl[]=avg_rating";
			url += @"&fl[]=subject";
			url += $@"&rows={numberOfItemsInPage}";
			url += $@"&page={page}";
			url += @"&output=json";

			HttpResponseMessage responseMessage = await _httpClient.GetAsync(url, cancellationToken);

			string result = await responseMessage.Content.ReadAsStringAsync();

			ArchiveOrgQueryResult queryResult = new();

			using JsonDocument document = JsonDocument.Parse(result);
			JsonElement response = document.RootElement.GetProperty("response");

			foreach (JsonElement doc in response.GetProperty("docs").EnumerateArray())
			{
				ArchiveOrgDocument archiveOrgDocument = new()
				{
					Identifier = GetStringProperty(doc, "identifier"),
					Title = GetStringProperty(doc, "title"),
					Creator = GetStringProperty(doc, "creator"),
					Description = GetStringProperty(doc, "description"),
					Rating = GetDoublePropertyFromString(doc, "avg_rating"),
					Downloads = GetIntProperty(doc, "downloads")
				};

				queryResult.Documents.Add(archiveOrgDocument);
			}

			return queryResult;
		}

		private void ParseFiles(
			JsonDocument document,
			ArchiveOrgMetadata metadata,
			string fileFormat)
		{
			foreach (JsonElement file in document.RootElement.GetProperty("files").EnumerateArray())
			{
				string format = GetStringProperty(file, "format");

				if (!string.Equals(format, fileFormat, StringComparison.InvariantCultureIgnoreCase))
				{
					continue;
				}

				ArchiveOrgChapterMetadata chapterMetadata = new()
				{
					Title = GetStringProperty(file, "title"),
					FileName = GetStringProperty(file, "name"),
					Sha1 = GetStringProperty(file, "sha1"),
					SizeInBytes = GetIntPropertyFromString(file, "size"),
					Duration = ParseLength(file.GetProperty("length").GetString()),
				};
				
				string trackString = GetStringProperty(file, "track");

				if (!string.IsNullOrEmpty(trackString))
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

		private string GetStringProperty(JsonElement element, string property)
		{
			if (!element.TryGetProperty(property, out JsonElement propertyElement))
			{
				Debug.WriteLine($"{property} does not exist");
				return "";
			}
			
			string? value = propertyElement.GetString();

			if (value == null)
			{
				Debug.WriteLine($"{property} is null");
				return "";
			}

			return value;
		}

		private int GetIntProperty(JsonElement element, string property)
		{
			if (!element.TryGetProperty(property, out JsonElement propertyElement))
			{
				Debug.WriteLine($"{property} does not exist");
				return 0;
			}

			if (!propertyElement.TryGetInt32(out int intValue))
			{
				Debug.WriteLine($"Cannot parse {property}");
			}

			return intValue;
		}

		private int GetIntPropertyFromString(JsonElement element, string property)
		{
			if (!element.TryGetProperty(property, out JsonElement propertyElement))
			{
				Debug.WriteLine($"{property} does not exist");
				return 0;
			}

			string? value = propertyElement.GetString();

			if (value == null)
			{
				Debug.WriteLine($"{property} is null");
				return 0;
			}

			if (!int.TryParse(value, out int intValue))
			{
				Debug.WriteLine($"{property} is not in expected form: " + value);
			}
			
			return intValue;
		}

		private double GetDoublePropertyFromString(JsonElement element, string property)
		{
			if (!element.TryGetProperty(property, out JsonElement propertyElement))
			{
				Debug.WriteLine($"{property} does not exist");
				return 0;
			}
			
			string? value = propertyElement.GetString();

			if (value == null)
			{
				Debug.WriteLine($"{property} is null");
				return 0;
			}

			if (!double.TryParse(value, out double doubleValue))
			{
				Debug.WriteLine($"{property} is not in expected form: " + value);
			}
			
			return doubleValue;
		}

		private void ParseTags(in JsonElement element, ArchiveOrgDocument archiveOrgDocument)
		{
			if (!element.TryGetProperty("subject", out JsonElement tagsElement))
			{
				Debug.WriteLine("subject does not exist");
				return;
			}

			if (tagsElement.GetArrayLength() <= 0)
			{
				Debug.WriteLine("Empty subject array.");
				return;
			}

			foreach (JsonElement tagElement in tagsElement.EnumerateArray())
			{
				string? tag = tagElement.GetString();

				if (string.IsNullOrEmpty(tag))
				{
					Debug.WriteLine($"Tag is empty or null: {tag}");
					continue;
				}

				archiveOrgDocument.Tags.Add(tag!);
			}
		}

		private TimeSpan ParseLength(string? length)
		{
			void Error()
			{
				Debug.WriteLine("Length is not in expected form: " + length);
			}

			if (length == null)
			{
				Debug.WriteLine("Length is null");
				return TimeSpan.Zero;
			}

			if (!length.Contains(":"))
			{
				if (double.TryParse(length, out double totalSeconds))
				{
					return TimeSpan.FromSeconds(totalSeconds);
				}

				Error();
				return TimeSpan.Zero;
			}

			string[] tokens = length.Split(':');

			if (tokens.Length != 2)
			{
				Error();
				return TimeSpan.Zero;
			}

			if (!int.TryParse(tokens[0], out int minutes))
			{
				Error();
				return TimeSpan.Zero;
			}

			if (!int.TryParse(tokens[1], out int seconds))
			{
				Error();
				return TimeSpan.Zero;
			}

			return TimeSpan.FromSeconds(minutes * 60 + seconds);
		}
	}
}