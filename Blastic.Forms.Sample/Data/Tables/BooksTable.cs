using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Data;
using Blastic.Data.ProviderSpecific;
using Blastic.Data.Tables;
using Blastic.Exceptions;

namespace Blastic.Forms.Sample.Data.Tables
{
	public class BooksTable : TableBase
	{
		public BooksTable(ConnectionFactory connectionFactory) : base(connectionFactory)
		{
		}

		public async Task<List<Book>> GetAll(CancellationToken cancellationToken)
		{
			using Connection connection = ConnectionFactory.CreateConnection();
			using Command command = connection.CreateCommand();

			command.CommandText = "SELECT * FROM Books";

			List<Book> books = new List<Book>();

			using DataReader reader = await command.ExecuteReader(cancellationToken);

			while (reader.Read())
			{
				Book book = CreateBook(reader);
				books.Add(book);
			}

			return books;
		}

		public async Task<Book> Get(int archiveOrgId, CancellationToken cancellationToken)
		{
			using Connection connection = ConnectionFactory.CreateConnection();
			using Command command = connection.CreateCommand();

			command.CommandText = "SELECT * FROM Books WHERE ArchiveOrgId=@ArchiveOrgId";
			command.AddParameterWithValue("@ArchiveOrgId", archiveOrgId);

			using DataReader reader = await command.ExecuteReader(cancellationToken);

			if (!reader.Read())
			{
				throw new NotFoundException($"Book with id {archiveOrgId} not found.");
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

		public async Task PutAll(IEnumerable<Book> books, CancellationToken cancellationToken)
		{
			using Connection connection = ConnectionFactory.CreateConnection();

			foreach (Book book in books)
			{
				await Put(connection, book, cancellationToken);
			}
		}

		public async Task Put(Book book, CancellationToken cancellationToken)
		{
			using Connection connection = ConnectionFactory.CreateConnection();
			await Put(connection, book, cancellationToken);
		}

		private async Task Put(Connection connection, Book book, CancellationToken cancellationToken)
		{
			if (await Contains(connection, book.ArchiveOrgId, cancellationToken))
			{
				await Update(connection, book, cancellationToken);
			}
			else
			{
				await Insert(connection, book, cancellationToken);
			}
		}

		private async Task<bool> Contains(Connection connection, string archiveOrgId, CancellationToken cancellationToken)
		{
			using Command command = connection.CreateCommand();

			command.CommandText = "SELECT 1 FROM Books WHERE ArchiveOrgId=@ArchiveOrgId";
			command.AddParameterWithValue("@ArchiveOrgId", archiveOrgId);

			using DataReader reader = await command.ExecuteReader(cancellationToken);
			return reader.HasRows;
		}

		public async Task Delete(int archiveOrgId, CancellationToken cancellationToken)
		{
			using Connection connection = ConnectionFactory.CreateConnection();
			using Command command = connection.CreateCommand();

			command.CommandText = @"DELETE FROM Books WHERE ArchiveOrgId=@ArchiveOrgId";
			command.AddParameterWithValue("@ArchiveOrgId", archiveOrgId);

			await command.ExecuteNonQuery(cancellationToken);
		}

		private async Task Insert(Connection connection, Book book, CancellationToken cancellationToken)
		{
			using Command command = connection.CreateCommand();

			command.CommandText = @"INSERT INTO Books (ArchiveOrgId, Title, Description)
									VALUES (@ArchiveOrgId, @Title, @Description)";

			command.AddParameterWithValue("@ArchiveOrgId", book.ArchiveOrgId);
			command.AddParameterWithValue("@Title", book.Title);
			command.AddParameterWithValue("@Description", book.Description);

			ProviderSpecifics providerSpecifics = connection.ProviderSpecifics;

			book.Id = await providerSpecifics.ExecuteAndGetInsertedRowId(command, "Books", cancellationToken);
		}

		private async Task Update(Connection connection, Book book, CancellationToken cancellationToken)
		{
			using Command command = connection.CreateCommand();

			command.CommandText = @"UPDATE Books SET
										Title=@Title,
										Description=@Description
									WHERE ArchiveOrgId=@ArchiveOrgId";

			command.AddParameterWithValue("@Title", book.Title);
			command.AddParameterWithValue("@Description", book.Description);
			command.AddParameterWithValue("@ArchiveOrgId", book.ArchiveOrgId);

			await command.ExecuteNonQuery(cancellationToken);
		}
	}
}