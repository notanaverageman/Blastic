namespace Blastic.Data.ProviderSpecific
{
	public class SqlServerProviderSpecifics : ProviderSpecifics
	{
		protected override string GetColumnsQueryColumnName => "COLUMN_NAME";
		protected override int MaximumQueryParameter => 999;

		public override string IdentityColumn => "IDENTITY(1,1)";
		public override string NVarCharMaxColumn => "NVARCHAR(MAX)";
		public override string BlobColumn => "VARBINARY(MAX)";

		public override string TurkishCaseSensitiveCollation => "COLLATE Turkish_CS_AS";
		public override string TurkishCaseInsensitiveCollation => "COLLATE Turkish_CI_AI";

		public override string IgnoreDuplicatesOnIndex => "WITH IGNORE_DUP_KEY";
		public override string InsertIgnoringDuplicates => "INSERT INTO";

		public SqlServerProviderSpecifics(Connection connection) : base(connection)
		{
		}

		protected override string GetTableExistsQuery(string tableName)
		{
			return $"SELECT 1 WHERE OBJECT_ID('{tableName}', 'U') IS NOT NULL ";
		}

		protected override string GetAlterCollationQuery(string tableName, string columnName, string dataType, string collation)
		{
			return $"ALTER TABLE {tableName} ALTER COLUMN {columnName} {dataType} {collation}";
		}

		protected override string GetCopyTableQuery(string source, string destination, string columnNames)
		{
			return $"SELECT {columnNames} INTO {destination} FROM {source}";
		}

		protected override string GetColumnNamesQuery(string tableName)
		{
			return $"SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}'";
		}

		protected override string GetDropIndexQuery(string tableName, string indexName)
		{
			return $"DROP INDEX {tableName}.{indexName}";
		}

		protected override string GetPaginationQuery(int skip, int take)
		{
			return $" OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";
		}

		protected override string GetInsertedRowIdQuery(string tableName)
		{
			return "SELECT SCOPE_IDENTITY()";
		}
	}
}