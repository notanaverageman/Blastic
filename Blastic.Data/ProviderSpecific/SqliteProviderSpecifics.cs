namespace Blastic.Data.ProviderSpecific
{
	public class SqliteProviderSpecifics : ProviderSpecifics
	{
		protected override string GetColumnsQueryColumnName => "name";
		protected override int MaximumQueryParameter => 999;

		public override string IdentityColumn => "AUTOINCREMENT";
		public override string NVarCharMaxColumn => "TEXT";
		public override string BlobColumn => "BLOB";

		public override string TurkishCaseSensitiveCollation => "COLLATE BINARY";
		public override string TurkishCaseInsensitiveCollation => "COLLATE NOCASE";

		public override string IgnoreDuplicatesOnIndex => "";
		public override string InsertIgnoringDuplicates => "INSERT OR IGNORE INTO";

		public SqliteProviderSpecifics(Connection connection) : base(connection)
		{
		}

		protected override string GetTableExistsQuery(string tableName)
		{
			return $"SELECT 1 FROM sqlite_master WHERE type='table' AND name='{tableName}'";
		}

		protected override string GetAlterCollationQuery(string tableName, string columnName, string dataType, string collation)
		{
			// SQLite does not support altering collation. The default
			// collation of SQLite is case sensitive.
			return "PRAGMA Noop";
		}

		protected override string GetCopyTableQuery(string source, string destination, string columnNames)
		{
			return $"INSERT INTO {destination} SELECT {columnNames} FROM {source}";
		}

		protected override string GetColumnNamesQuery(string tableName)
		{
			return $"PRAGMA table_info({tableName});";
		}

		protected override string GetDropIndexQuery(string tableName, string indexName)
		{
			return $"DROP INDEX {indexName}";
		}

		protected override string GetPaginationQuery(int skip, int take)
		{
			return $" LIMIT {take} OFFSET {skip}";
		}

		protected override string GetInsertedRowIdQuery(string tableName)
		{
			return $"SELECT seq FROM sqlite_sequence WHERE name=\"{tableName}\"";
		}
	}
}