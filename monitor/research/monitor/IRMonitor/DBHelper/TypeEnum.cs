
namespace DBHelper
{
    public enum DBType
    {
        /// <summary>
        /// Microsoft Access
        /// </summary>
        MsAccess = 0,

        /// <summary>
        /// Microsoft SQL Server
        /// </summary>
        MsSql,

        /// <summary>
        /// SQLite
        /// </summary>
        SQLite,

        /// <summary>
        /// Oracle
        /// </summary>
        Oracle,

        /// <summary>
        /// Object Linking and Embedding,Database
        /// </summary>
        OleDb,

        /// <summary>
        /// Postgre Sql
        /// </summary>
        PostgreSql
    }

    public enum TransactionType
    {
        None = 0,
        Open,
        Commit,
        Rollback
    }

    public enum ColumnType
    {
        Decimal = 0,
        Boolean,
        Int32,
        Int64,
        Binary,
        VarBinary,
        Date,
        Time,
        DateTime,
        Char,
        VarChar,
        Text,
        Blob,
        Double,
    }
}
