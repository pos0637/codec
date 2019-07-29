using System;
using System.Collections;
using System.Data;
using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace DBHelper.ADO
{
    internal class SQLiteHelper : IDbHelper
    {
        #region "私有成员"

        private SQLiteConnection mConnection;
        private Boolean mConnectionIsOpen;
        private SQLiteTransaction mTransaction;
        private TransactionType mTransactionState;

        #endregion

        #region "构造函数"

        public SQLiteHelper(String connectionString)
        {
            mConnection = new SQLiteConnection(connectionString);
            mConnectionIsOpen = false;
            mTransactionState = TransactionType.None;
        }

        public SQLiteHelper(SQLiteConnection connection)
        {
            mConnection = connection;
            mConnectionIsOpen = false;
            mTransactionState = TransactionType.None;
        }

        #endregion

        #region "IDbCommandHelp成员"

        public String SqlPlaceHolder {
            get { return "@"; }
        }

        public IDataParameter CreateParameter()
        {
            return new SQLiteParameter();
        }

        public IDataParameter CreateParameter(String parameterName, Object value)
        {
            return new SQLiteParameter(parameterName, value);
        }

        public IDataParameter CreateParameter(String parameterName, Type dataType)
        {
            return new SQLiteParameter(parameterName, GetSqlDbType(dataType));
        }

        public IDataParameter CreateParameter(String parameterName, ColumnType dataType)
        {
            return new SQLiteParameter(parameterName, GetSqlDbType(dataType));
        }

        public IDataParameter CreateParameter(String parameterName, Object value, ColumnType dataType)
        {
            IDataParameter parameter = CreateParameter(parameterName, dataType);
            parameter.Value = value;
            return parameter;
        }

        public IDataParameter CreateParameter(String parameterName, ColumnType dataType, Int32 size)
        {
            return new SQLiteParameter(parameterName, GetSqlDbType(dataType), size);
        }

        public IDataParameter CreateParameter(String parameterName, Object value, ColumnType dataType, Int32 size)
        {
            IDataParameter parameter = CreateParameter(parameterName, dataType, size);
            parameter.Value = value;
            return parameter;
        }

        public IDbCommand CreateDeleteCommand(String sourceTableName, params IDataParameter[] conditionParameters)
        {
            String condition = SqlBuilder.CreateWhereClause(conditionParameters, SqlPlaceHolder);
            String commandText = SqlBuilder.CreateDeleteSql(sourceTableName, condition);

            SQLiteCommand command = CreateDbCommand(commandText, CommandType.Text, new IDataParameter[0]);
            foreach (IDataParameter parameter in conditionParameters) {
                command.Parameters.Add(parameter);
            }

            return command;
        }

        public IDbCommand CreateInsertCommand(String sourceTableName, params IDataParameter[] parameters)
        {
            String commandText = SqlBuilder.CreateInsertSql(sourceTableName, GetColumns(parameters), SqlPlaceHolder);
            return CreateDbCommand(commandText, CommandType.Text, parameters);
        }

        public IDbCommand CreateUpdateCommand(String sourceTableName, IDataParameter[] whereParams,
            params IDataParameter[] cmdParams)
        {
            String where = SqlBuilder.CreateWhereClause(whereParams, SqlPlaceHolder);
            IDbCommand command = CreateUpdateCommand(sourceTableName, where, cmdParams);
            foreach (IDataParameter parameter in whereParams) {
                command.Parameters.Add(parameter);
            }
            return command;
        }

        public IDbCommand CreateUpdateCommand(String sourceTableName, String where, params IDataParameter[] cmdParams)
        {
            String commandText = SqlBuilder.CreateUpdateSql(sourceTableName, GetColumns(cmdParams), where, SqlPlaceHolder);
            return CreateDbCommand(commandText, CommandType.Text, cmdParams);
        }

        #endregion

        #region "IDbHelp成员"
        public Boolean ConnectionIsOpen {
            get { return mConnectionIsOpen; }
        }

        public TransactionType TransactionState {
            get { return mTransactionState; }
        }

        public void OpenConnection()
        {
            if (mTransactionState != TransactionType.Open) {
                if (mConnection.State == ConnectionState.Closed) {
                    try {
                        mConnection.Open();
                        mConnectionIsOpen = true;
                    }
                    catch (Exception exception) {
                        mConnectionIsOpen = false;
                        throw exception;
                    }
                }
            }
        }

        public void CloseConnection()
        {
            if (mTransactionState != TransactionType.Open) {
                if (mConnection.State != ConnectionState.Closed) {
                    try {
                        mConnection.Close();
                        mConnectionIsOpen = false;
                    }
                    catch (Exception exception) {
                        throw exception;
                    }
                }
            }
        }

        public void BeginTransaction()
        {
            OpenConnection();
            try {
                mTransaction = mConnection.BeginTransaction();
                mTransactionState = TransactionType.Open;
            }
            catch (Exception exception) {
                CloseConnection();
                throw exception;
            }
        }

        public void RollbackTransaction()
        {
            if (mTransaction != null) {
                try {
                    mTransaction.Rollback();
                    mTransaction = null;
                    mTransactionState = TransactionType.Rollback;
                }
                finally {
                    CloseConnection();
                }
            }
        }

        public void CommitTransaction()
        {
            if (mTransaction != null && mTransaction.Connection != null) {
                try {
                    mTransaction.Commit();
                    mTransaction = null;
                    mTransactionState = TransactionType.Commit;
                }
                finally {
                    CloseConnection();
                }
            }
        }

        public Int32 GetCurrentIdentity(String tableName)
        {
            try {
                Object id = ExecuteScalar("select IDENT_CURRENT('" + tableName + "')");
                return (id == null) ? 0 : Convert.ToInt32(id);
            }
            catch {
                return 0;
            }
        }

        public Int32 ExecuteNonQuery(IDbCommand cmd)
        {
            Int32 num;
            try {
                OpenConnection();
                num = cmd.ExecuteNonQuery();
            }
            catch (Exception exception) {
                throw new Exception(
                    (exception.Message + SqlBuilder.GetFormatedSql(cmd, SqlPlaceHolder))
                    , exception);
            }
            finally {
                CloseConnection();
            }
            return num;
        }

        public Int32 ExecuteNonQuery(String commandText)
        {
            return ExecuteNonQuery(commandText, CommandType.Text, null);
        }

        public Int32 ExecuteNonQuery(String commandText, CommandType cmdType, params IDataParameter[] cmdParams)
        {
            using (SQLiteCommand command = CreateDbCommand(commandText, cmdType, cmdParams)) {
                Int32 num = ExecuteNonQuery(command);
                for (Int32 i = 0; i < command.Parameters.Count; i++) {
                    if (!((command.Parameters[i].Direction == ParameterDirection.Input)
                        || (cmdParams[i] is SQLiteParameter))) {
                        cmdParams[i].Value = command.Parameters[i].Value;
                    }
                }
                return num;
            }
        }

        public Object ExecuteScalar(String commandText)
        {
            return ExecuteScalar(commandText, CommandType.Text, null);
        }

        public Object ExecuteScalar(String commandText, CommandType cmdType, params IDataParameter[] cmdParams)
        {
            Object value;
            using (SQLiteCommand command = CreateDbCommand(commandText, cmdType, cmdParams)) {
                try {
                    OpenConnection();
                    value = command.ExecuteScalar();
                }
                catch (Exception exception) {
                    throw new Exception(
                        exception.Message + SqlBuilder.GetFormatedSql(command, SqlPlaceHolder),
                        exception);
                }
                finally {
                    CloseConnection();
                }
            }
            return value;
        }

        public IDataReader GetDataReader(String selectSql)
        {
            return GetDataReader(selectSql, CommandType.Text, null);
        }

        public IDataReader GetDataReader(String commandText, CommandType cmdType, params IDataParameter[] cmdParams)
        {
            IDataReader reader;
            SQLiteCommand cmd = CreateDbCommand(commandText, cmdType, cmdParams);
            try {
                OpenConnection();
                reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception exception) {
                CloseConnection();
                throw new Exception(
                    (exception.Message + SqlBuilder.GetFormatedSql(cmd, SqlPlaceHolder)),
                    exception);
            }
            finally {
                if (cmd != null) {
                    cmd.Dispose();
                }
            }
            return reader;
        }

        public DataSet GetDataSet(String selectSql)
        {
            return GetDataSet(selectSql, CommandType.Text, null);
        }

        public DataSet GetDataSet(String commandText, CommandType cmdType, params IDataParameter[] cmdParams)
        {
            DataSet dataSet = new DataSet();
            using (SQLiteDataAdapter adapter = new SQLiteDataAdapter()) {
                try {
                    OpenConnection();
                    adapter.SelectCommand = CreateDbCommand(commandText, cmdType, cmdParams);
                    adapter.Fill(dataSet, SqlBuilder.GetTableNameFromSQL(commandText));
                }
                catch (Exception exception) {
                    CloseConnection();
                    if (adapter.SelectCommand == null) {
                        throw new Exception(
                            (exception.Message + commandText),
                            exception);
                    }
                    throw new Exception(
                        (exception.Message + SqlBuilder.GetFormatedSql(adapter.SelectCommand, SqlPlaceHolder)),
                        exception);
                }
                finally {
                    adapter.TableMappings.Clear();
                }
            }
            return dataSet;
        }

        public DataTable GetDataTable(String selectSql)
        {
            return GetDataTable(selectSql, CommandType.Text, null);
        }

        public DataTable GetDataTable(String commandText, CommandType cmdType, params IDataParameter[] cmdParams)
        {
            return GetDataSet(commandText, cmdType, cmdParams).Tables[0];
        }

        public DataView GetDataView(String selectSql)
        {
            return GetDataView(selectSql, CommandType.Text, null);
        }

        public DataView GetDataView(String commandText, CommandType cmdType, params IDataParameter[] cmdParams)
        {
            return GetDataTable(commandText, cmdType, cmdParams).DefaultView;
        }

        #endregion

        #region "私有函数"

        private DbType GetSqlDbType(Type dataType)
        {
            switch (dataType.ToString()) {
                case "System.String":
                    return DbType.String;
                case "System.Decimal":
                    return DbType.Decimal;
                case "System.DateTime":
                    return DbType.DateTime;
                case "System.Byte[]":
                    return DbType.Binary;
            }
            return DbType.String;
        }

        private DbType GetSqlDbType(ColumnType dataType)
        {
            switch (dataType) {
                case ColumnType.Decimal:
                    return DbType.Decimal;
                case ColumnType.Binary:
                    return DbType.Binary;
                case ColumnType.VarBinary:
                    return DbType.Binary;
                case ColumnType.Date:
                    return DbType.DateTime;
                case ColumnType.DateTime:
                    return DbType.Date;
                case ColumnType.Char:
                    return DbType.AnsiStringFixedLength;
                case ColumnType.VarChar:
                    return DbType.AnsiString;
                case ColumnType.Text:
                    return DbType.AnsiString;
                case ColumnType.Blob:
                    return DbType.Binary;
                case ColumnType.Double:
                    return DbType.Double;
            }
            return DbType.String;
        }

        private DbType GetSqlDbType(DbType dataType)
        {
            return dataType;
        }

        private SQLiteCommand CreateDbCommand(String commandText, CommandType cmdType, params IDataParameter[] parameters)
        {
            SQLiteCommand command = new SQLiteCommand() {
                CommandType = cmdType,
                Connection = mConnection,
                CommandText = commandText
            };

            if (mTransaction != null)
                command.Transaction = mTransaction;

            if (parameters != null) {
                if (command.CommandText.IndexOf("?") > 0) {
                    Regex regex = new Regex(@"\?");
                    foreach (IDataParameter parameter in parameters) {
                        command.CommandText = regex.Replace(
                            command.CommandText
                            , SqlBuilder.GetSqlParameterName(parameter.ParameterName, SqlPlaceHolder)
                            , 1);

                        SQLiteParameter param = ConvertToSqlParameter(parameter);
                        if (param.Value == null) {
                            param.IsNullable = true;
                            param.Value = DBNull.Value;
                        }

                        command.Parameters.Add(param);
                    }
                }
                else {
                    foreach (IDataParameter parameter in parameters) {
                        SQLiteParameter param = ConvertToSqlParameter(parameter);
                        if (param.Value == null) {
                            param.IsNullable = true;
                            param.Value = DBNull.Value;
                        }

                        command.Parameters.Add(param);
                    }
                }
            }

            return command;
        }

        private SQLiteParameter ConvertToSqlParameter(IDataParameter param)
        {
            if (param is SQLiteParameter)
                return (SQLiteParameter)param;

            SQLiteParameter parameter = new SQLiteParameter(param.ParameterName, param.Value) {
                Direction = param.Direction
            };

            if (parameter.Direction != ParameterDirection.Input)
                parameter.DbType = GetSqlDbType(param.DbType);

            return parameter;
        }

        private String[] GetColumns(params IDataParameter[] parameters)
        {
            ArrayList list = new ArrayList();
            foreach (IDataParameter parameter in parameters) {
                list.Add(parameter.ParameterName);
            }
            return (String[])list.ToArray(typeof(String));
        }

        #endregion
    }
}
