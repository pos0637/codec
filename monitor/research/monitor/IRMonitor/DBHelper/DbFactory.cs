using System;
using System.Data;
using DBHelper.ADO;

namespace DBHelper
{
    public class DbFactory
    {
        private static readonly DbFactory _Instance = new DbFactory();

        private DbFactory()
        {
        }

        public static DbFactory Instance
        {
            get
            {
                return _Instance;
            }
        }

        public IDbHelper GetDbHelper(String connectionString, DBType dbType)
        {
            switch (dbType) {
                case DBType.MsSql:
                case DBType.PostgreSql:
                    return null;

                case DBType.SQLite:
                    try {
                        return new SQLiteHelper(connectionString);
                    }
                    catch {
                        return null;
                    }

                case DBType.MsAccess:
                case DBType.Oracle:
                case DBType.OleDb:
                default:
                    return null;
            }
        }

        /// <summary>
        /// 创建一个自定义Sql对象
        /// </summary>
        /// <param name="dbHelper">IDbHelp</param>
        /// <param name="sql">要执行的Sql语句</param>
        /// <param name="commandType">执行命令类型，默认为Text</param>
        /// <returns>Sql对象</returns>
        public SqlHelper CreateSqlHelper(IDbHelper dbHelper, String sql,
            CommandType commandType = CommandType.Text)
        {
            return new SqlHelper(dbHelper, sql, commandType);
        }

        /// <summary>
        /// 创建一个 Select Sql 对象
        /// </summary>
        /// <param name="dbHelper">IDbHelp</param>
        /// <param name="tableName">要查询数据库记录所在的表名</param>
        /// <returns>Sql对象</returns>
        public SqlHelper CreateQueryHelper(IDbHelper dbHelper, String tableName)
        {
            return new SqlHelper(dbHelper, tableName, SqlExpression.Manipulation.Select);
        }

        /// <summary>
        /// 创建一个 Insert Sql 对象
        /// </summary>
        /// <param name="dbHelper">IDbHelp</param>
        /// <param name="tableName">要插入数据库记录所在的表名</param>
        /// <returns>Sql对象</returns>
        public SqlHelper CreateInsertHelper(IDbHelper dbHelper, String tableName)
        {
            return new SqlHelper(dbHelper, tableName, SqlExpression.Manipulation.Insert);
        }

        /// <summary>
        /// 创建一个 Update Sql 对象
        /// </summary>
        /// <param name="dbHelper">IDbHelp</param>
        /// <param name="tableName">要插入数据库记录所在的表名</param>
        /// <returns>Sql对象</returns>
        public SqlHelper CreateUpdateHelper(IDbHelper dbHelper, String tableName)
        {
            return new SqlHelper(dbHelper, tableName, SqlExpression.Manipulation.Update);
        }

        /// <summary>
        /// 创建一个 Delete Sql 对象
        /// </summary>
        /// <param name="dbHelper">IDbHelp</param>
        /// <param name="tableName">要插入数据库记录所在的表名</param>
        /// <returns>Sql对象</returns>
        public SqlHelper CreateDeleteHelper(IDbHelper dbHelper, String tableName)
        {
            return new SqlHelper(dbHelper, tableName, SqlExpression.Manipulation.Delete);
        }
    }
}
