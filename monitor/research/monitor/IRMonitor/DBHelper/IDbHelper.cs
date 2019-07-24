using System;
using System.Data;

namespace DBHelper
{
    public interface IDbHelper : IDbCommandHelp
    {
        /// <summary>
        /// 连接是否打开
        /// </summary>
        Boolean ConnectionIsOpen { get; }

        /// <summary>
        /// 事务状态
        /// </summary>
        TransactionType TransactionState { get; }

        /// <summary>
        /// 打开数据库连接
        /// </summary>
        /// <exception cref="System.InvalidOperationException">未指定数据源或服务器，不能打开连接。 或 连接已打开。</exception>
        /// <exception cref="System.Exception">在打开连接时出现连接级别的错误。</exception>
        void OpenConnection();

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        /// <exception cref="System.Exception">在打开连接时出现连接级别的错误。</exception>
        void CloseConnection();

        /// <summary>
        /// 开始数据库事务。
        /// </summary>
        /// <exception cref="System.InvalidOperationException">不支持并行事务。</exception>
        /// <exception cref="System.Exception">不允许执行并行事务。</exception>
        void BeginTransaction();

        /// <summary>
        /// 从挂起状态回滚事务。
        /// </summary>
        /// <exception cref="System.InvalidOperationException">事务已提交或回滚。或连接已断开。</exception>
        /// <exception cref="System.Exception">尝试提交事务时出错。</exception>
        void RollbackTransaction();

        /// <summary>
        /// 提交数据库事务。
        /// </summary>
        /// <exception cref="System.InvalidOperationException">事务已提交或回滚。或连接已断开。</exception>
        /// <exception cref="System.Exception">尝试提交事务时出错。</exception>
        void CommitTransaction();

        /// <summary>
        /// 获取指定表插入的最后一个标识列的值
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>标识列的值</returns>
        Int32 GetCurrentIdentity(String tableName);

        /// <summary>
        /// 执行 SQL 语句，并返回受影响的行数。
        /// </summary>
        /// <param name="command">IDbCommand</param>
        /// <exception cref="System.InvalidOperationException">连接不存在。或连接未打开。</exception>
        /// <returns>受影响的行数。</returns>
        Int32 ExecuteNonQuery(IDbCommand command);

        /// <summary>
        /// 执行 SQL 语句，并返回受影响的行数。
        /// </summary>
        /// <param name="commandText">获取或设置要对数据源执行的 Transact-SQL 语句。</param>
        /// <exception cref="System.InvalidOperationException">连接不存在。或连接未打开。</exception>
        /// <returns>受影响的行数。</returns>
        Int32 ExecuteNonQuery(String commandText);

        /// <summary>
        /// 执行 SQL 语句，并返回受影响的行数。
        /// </summary>
        /// <param name="commandText">获取或设置要对数据源执行的 Transact-SQL 语句、表名或存储过程。</param>
        /// <param name="commandType">CommandType。</param>
        /// <param name="parameters">IDataParameter 集合。</param>
        /// <exception cref="System.InvalidOperationException">连接不存在。或连接未打开。</exception>
        /// <exception cref="System.InvalidCastException">传递的参数不是 ADO中相应Helper的 IDataParameter。</exception>
        /// <exception cref="System.ArgumentNullException">parameters 中有参数为null。</exception>
        /// <exception cref="System.Exception">其他异常。</exception>
        /// <returns>受影响的行数。</returns>
        Int32 ExecuteNonQuery(String commandText, CommandType commandType, params IDataParameter[] parameters);

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。 忽略其他列或行。
        /// </summary>
        /// <param name="commandText">查询 SQL 语句。</param>
        /// <exception cref="System.Exception">异常。</exception>
        /// <returns>结果集中第一行的第一列；如果结果集为空，则为空引用。</returns>
        Object ExecuteScalar(String commandText);

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。 忽略其他列或行。
        /// </summary>
        /// <param name="commandText">查询 SQL 语句。</param>
        /// <param name="commandType">CommandType</param>
        /// <param name="parameters">IDataParameter 集合。</param>
        /// <exception cref="System.Exception">异常。</exception>
        /// <returns>结果集中第一行的第一列；如果结果集为空，则为空引用。</returns>
        Object ExecuteScalar(String commandText, CommandType commandType, params IDataParameter[] parameters);

        /// <summary>
        /// 执行查询，并返回 IDataReader。
        /// </summary>
        /// <param name="commandText">查询 SQL 语句。</param>
        /// <exception cref="System.Exception">异常。</exception>
        /// <returns>IDataReader</returns>
        [Obsolete("请谨慎使用此方法，以防止忘记关闭数据库连接造成连接池满的问题！")]
        IDataReader GetDataReader(String commandText);

        /// <summary>
        /// 执行查询，并返回 IDataReader。
        /// </summary>
        /// <param name="commandText">查询 SQL 语句、表名或存储过程。</param>
        /// <param name="commandType">CommandType</param>
        /// <param name="parameters">IDataParameter 集合。</param>
        /// <exception cref="System.Exception">异常。</exception>
        /// <returns>IDataReader</returns>
        [Obsolete("请谨慎使用此方法，以防止忘记关闭数据库连接造成连接池满的问题！")]
        IDataReader GetDataReader(String commandText, CommandType commandType, params IDataParameter[] parameters);

        /// <summary>
        /// 执行查询，并返回 DataSet。
        /// </summary>
        /// <param name="commandText">查询 SQL 语句。</param>
        /// <exception cref="System.Exception">异常。</exception>
        /// <returns>DataSet</returns>
        DataSet GetDataSet(String commandText);

        /// <summary>
        /// 执行查询，并返回 DataSet。
        /// </summary>
        /// <param name="commandText">查询 SQL 语句、表名或存储过程。</param>
        /// <param name="commandType">CommandType</param>
        /// <param name="parameters">IDataParameter 集合。</param>
        /// <exception cref="System.Exception">异常。</exception>
        /// <returns>DataSet</returns>
        DataSet GetDataSet(String commandText, CommandType commandType, params IDataParameter[] parameters);

        /// <summary>
        /// 执行查询，并返回 DataTable。
        /// </summary>
        /// <param name="commandText">查询 SQL 语句。</param>
        /// <exception cref="System.Exception">异常。</exception>
        /// <returns>DataTable</returns>
        DataTable GetDataTable(String commandText);

        /// <summary>
        /// 执行查询，并返回 DataTable。
        /// </summary>
        /// <param name="commandText">查询 SQL 语句、表名或存储过程。</param>
        /// <param name="commandType">CommandType</param>
        /// <param name="parameters">IDataParameter 集合。</param>
        /// <exception cref="System.Exception">异常。</exception>
        /// <returns>DataTable</returns>
        DataTable GetDataTable(String commandText, CommandType commandType, params IDataParameter[] parameters);

        /// <summary>
        /// 执行查询，并返回 DataView。
        /// </summary>
        /// <param name="commandText">查询 SQL 语句。</param>
        /// <exception cref="System.Exception">异常。</exception>
        /// <returns>DataView</returns>
        DataView GetDataView(String commandText);

        /// <summary>
        /// 执行查询，并返回 DataView。
        /// </summary>
        /// <param name="commandText">查询 SQL 语句、表名或存储过程。</param>
        /// <param name="commandType">CommandType</param>
        /// <param name="parameters">IDataParameter 集合。</param>
        /// <exception cref="System.Exception">异常。</exception>
        /// <returns>DataView</returns>
        DataView GetDataView(String commandText, CommandType commandType, params IDataParameter[] parameters);
    }
}
