using Microsoft.EntityFrameworkCore;

namespace Blastic.Forms.Sample.Data
{
	public class SampleDbContext : DbContext
	{
		public DbSet<Author> Authors { get; set; }
		public DbSet<Book> Books { get; set; }
		public DbSet<AuthorBookMapping> AuthorBookMappings { get; set; }

		public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			AuthorBook(modelBuilder);
		}

		private static void AuthorBook(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<AuthorBookMapping>()
				.HasKey(
					x => new
					{
						x.AuthorId,
						x.BookId
					});

			modelBuilder.Entity<AuthorBookMapping>()
				.HasOne(x => x.Book)
				.WithMany(x => x.AuthorBookMappings)
				.HasForeignKey(x => x.BookId);

			modelBuilder.Entity<AuthorBookMapping>()
				.HasOne(x => x.Author)
				.WithMany(x => x.AuthorBookMappings)
				.HasForeignKey(x => x.AuthorId);
		}
	}
}