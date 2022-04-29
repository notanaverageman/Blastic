using System.Collections.Generic;
using Blastic.Data;
using Blastic.Data.Tables;

namespace Blastic.Forms.Sample.Data.Tables
{
	public class BooksTable : TableBase
	{
		public BooksTable(Connection connection) : base(connection)
		{
		}

		public List<Book> GetAll()
		{
			using Command command = Connection.CreateCommand();

			command.CommandText = "SELECT * FROM Books";

			List<Book> books = new();

			using DataReader reader = command.ExecuteReader();

			while (reader.Read())
			{
				Book book = CreateBook(reader);
				books.Add(book);
			}

			return books;
		}

		public Book? Get(string archiveOrgId)
		{
			using Command command = Connection.CreateCommand();

			command.CommandText = "SELECT * FROM Books WHERE ArchiveOrgId=@ArchiveOrgId";
			command.AddParameterWithValue("@ArchiveOrgId", archiveOrgId);

			using DataReader reader = command.ExecuteReader();

			if (!reader.Read())
			{
				return null;
			}

			return CreateBook(reader);
		}

		private Book CreateBook(DataReader reader)
		{
			int id = reader.Get<int>("Id");
			string archiveOrgId = reader.Get<string>("ArchiveOrgId");
			string name = reader.Get<string>("Title");
			string description = reader.Get<string>("Description");

			Book book = new Book
			{
				Id = id,
				ArchiveOrgId = archiveOrgId,
				Title = name,
				Description = description
			};

			return book;
		}

		public void PutAll(IEnumerable<Book> books)
		{
			foreach (Book book in books)
			{
				Put(book);
			}
		}

		public void Put(Book book)
		{
			using Command command = Connection.CreateCommand();

			command.CommandText = @"INSERT OR REPLACE INTO Books (ArchiveOrgId, Title, Description)
									VALUES (@ArchiveOrgId, @Title, @Description);
									SELECT last_insert_rowid();";

			command.AddParameterWithValue("@ArchiveOrgId", book.ArchiveOrgId);
			command.AddParameterWithValue("@Title", book.Title);
			command.AddParameterWithValue("@Description", book.Description);

			book.Id = command.ExecuteScalar<int>();
		}
		
		public void Delete(int archiveOrgId)
		{
			using Command command = Connection.CreateCommand();

			command.CommandText = @"DELETE FROM Books WHERE ArchiveOrgId=@ArchiveOrgId";
			command.AddParameterWithValue("@ArchiveOrgId", archiveOrgId);

			command.ExecuteNonQuery();
		}
	}
}