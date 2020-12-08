using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Blastic.Forms.Sample.Services
{
	public class DownloadService
	{
		private readonly HttpClient _httpClient;

		public DownloadService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task Download(
			string url,
			Stream destination,
			IProgress<double>? progress = null,
			CancellationToken cancellationToken = default)
		{
			using HttpResponseMessage response = await _httpClient.GetAsync(
				url,
				HttpCompletionOption.ResponseHeadersRead,
				cancellationToken);

			response.EnsureSuccessStatusCode();

			using Stream contentStream = await response.Content.ReadAsStreamAsync();
			double contentLength = response.Content.Headers.ContentLength ?? 1;

			long totalRead = 0L;
			byte[] buffer = new byte[8192];

			do
			{
				int read = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);

				if (read == 0)
				{
					break;
				}

				await destination.WriteAsync(buffer, 0, read, cancellationToken);

				totalRead += read;

				progress?.Report(totalRead / contentLength);

				if (cancellationToken.IsCancellationRequested)
				{
					break;
				}
			}
			while (true);
		}
	}
}