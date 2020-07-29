using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Blastic.Forms.Sample.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Blastic.Forms.Sample.EfCoreMigrations
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			// Example Package Manager Console command:
			// Add-Migration Initial -o Data/Migrations

			IHost host = CreateHostBuilder(args).Build();

			HttpClient httpClient = new HttpClient();
			HtmlParser htmlParser = new HtmlParser();

			string url = "https://archive.org/services/collection-rss.php?collection=librivoxaudio";
			string result = await httpClient.GetStringAsync(url);

			IHtmlDocument htmlDocument = htmlParser.ParseDocument(result);

			foreach (IElement element in htmlDocument.QuerySelectorAll("item"))
			{
				IElement description = element.Children.FirstOrDefault(x => x.LocalName == "description");

				if (description == null)
				{
					Console.WriteLine($"Item has no description. {element.InnerHtml}");
					continue;
				}

				IHtmlDocument descriptionDocument = htmlParser.ParseDocument(description.TextContent);

				string imageUrl = descriptionDocument.DescendentsAndSelf()
					.OfType<IHtmlImageElement>()
					.FirstOrDefault()
					?.Source;

				string bookDescription = descriptionDocument.DescendentsAndSelf()
					.OfType<IHtmlParagraphElement>()
					.FirstOrDefault()
					?.TextContent;

				Console.WriteLine($"Image      : {imageUrl}");
				Console.WriteLine($"Description: {bookDescription}");
			}
		}

		// EF Core uses this method at design time to access the DbContext
		public static IHostBuilder CreateHostBuilder(string[] args)
		{
			return Host.CreateDefaultBuilder(args)
				.ConfigureServices(
					x =>
					{
						x.AddDbContext<SampleDbContext>(y => y.UseSqlite("Data Source=Database.sqlite;"));
					});
		}
	}
}
