using Microsoft.EntityFrameworkCore;

namespace Blastic.Wpf.Sample.Data
{
	public class SampleContext : DbContext
	{
		public DbSet<Customer> Customers { get; set; }

		public SampleContext(DbContextOptions<SampleContext> options) : base(options)
		{
		}

		//protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		//{
		//	optionsBuilder.UseSqlite("Data Source=Database.sqlite");
		//}
	}
}