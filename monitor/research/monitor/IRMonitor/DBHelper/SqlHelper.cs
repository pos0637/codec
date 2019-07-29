using Fg.DBHelper.Utilities;
using System;
using System.Collections.Generic;
using System.Data;

namespace DBHelper
{
    public class SqlHelper
    {
        #region "私有成员"

        private IDbHelper mDbHelper;
        private SqlExpression mSqlExpression = new SqlExpression();

        #endregion

        #region "公有成员"

        /// <summary>
        /// 获取Sql字符串
        /// </summary>
        public String SqlString {
            get { return mSqlExpression.mSqlString; }
        }

        /// <summary>
        /// 获取当前语句中的参数数组
        /// </summary>
        public IDataParameter[] Parameters {
            get { return mSqlExpression.mParameters.ToArray(); }
        }

        #endregion

        #region "构造函数"

        private SqlHelper(IDbHelper dbHelper)
        {
            mDbHelper = dbHelper;
        }

        public SqlHelper(IDbHelper dbHelper, String tableName, SqlExpression.Manipulation manipulation)
            : this(dbHelper)
        {
            mSqlExpression.mIsCustom = false;
            mSqlExpression.mTableName = tableName;
            mSqlExpression.mManipulation = manipulation;
        }

        public SqlHelper(IDbHelper dbHelper, String sqlString, CommandType commandType)
            : this(dbHelper)
        {
            mSqlExpression.mIsCustom = true;
            mSqlExpression.mSqlString = sqlString;
            mSqlExpression.mCommandType = commandType;
        }

        #endregion

        #region "公有函数"

        #region "参数设置"

        public SqlHelper SetParameter(String parameterName, Object value)
        {
            IDataParameter parameter = mDbHelper.CreateParameter(parameterName, value);
            SetParameter(parameter);
            return this;
        }

        public SqlHelper SetParameter(String parameterName, Object value, ColumnType dataType)
        {
            IDataParameter parameter = mDbHelper.CreateParameter(parameterName, value, dataType);
            SetParameter(parameter);
            return this;
        }

        public SqlHelper SetParameter(String parameterName, Object value, ColumnType dataType, Int32 size)
        {
            IDataParameter parameter = mDbHelper.CreateParameter(parameterName, value, dataType, size);
            SetParameter(parameter);
            return this;
        }

        public SqlHelper SetParameter(List<IDataParameter> parameters)
        {
            foreach (IDataParameter parameter in parameters)
                SetParameter(parameter);
            return this;
        }

        public SqlHelper SetParameter(IDataParameter parameter)
        {
            mSqlExpression.AddParameter(parameter);
            return this;
        }

        public SqlHelper RemoveParameter(String parameterName)
        {
            mSqlExpression.RemoveParameter(parameterName);
            return this;
        }

        public SqlHelper RemoveParameter(IDataParameter parameter)
        {
            mSqlExpression.RemoveParameter(parameter);
            return this;
        }

        public SqlHelper ClearParameter()
        {
            mSqlExpression.ClearParameter();
            return this;
        }

        public SqlHelper SetUpdateParameter(String parameterName, Object value)
        {
            IDataParameter parameter = mDbHelper.CreateParameter(parameterName, value);
            SetUpdateParameter(parameter);
            return this;
        }

        public SqlHelper SetUpdateParameter(String parameterName, Object value, ColumnType dataType)
        {
            IDataParameter parameter = mDbHelper.CreateParameter(parameterName, value, dataType);
            SetUpdateParameter(parameter);
            return this;
        }

        public SqlHelper SetUpdateParameter(String parameterName, Object value, ColumnType dataType, Int32 size)
        {
            IDataParameter parameter = mDbHelper.CreateParameter(parameterName, value, dataType, size);
            SetUpdateParameter(parameter);
            return this;
        }

        public SqlHelper SetUpdateParameter(IDataParameter parameter)
        {
            mSqlExpression.AddUpdateParameter(parameter);
            return this;
        }

        public SqlHelper RemoveUpdateParameter(String parameterName)
        {
            mSqlExpression.RemoveUpdateParameter(parameterName);
            return this;
        }

        public SqlHelper RemoveUpdateParameter(IDataParameter parameter)
        {
            mSqlExpression.RemoveUpdateParameter(parameter);
            return this;
        }

        public SqlHelper ClearUpdateParameter()
        {
            mSqlExpression.ClearUpdateParameter();
            return this;
        }

        #endregion

        #region "排序设置"

        public SqlHelper OrderBy(String columnName,
            SqlOrder.OrderType orderType = SqlOrder.OrderType.ASC)
        {
            mSqlExpression.AddOrderColumn(new SqlOrder() {
                mOrderColumns = columnName,
                mType = orderType
            });

            return this;
        }

        #endregion

        #region "查询列设置"

        public SqlHelper Query(String columnName)
        {
            mSqlExpression.AddQueryColumns(columnName);
            return this;
        }

        #endregion

        public T ToScalar<T>(T defaultValue = default(T))
        {
            Object result = ToScalar();
            if (result == null)
                return defaultValue;

            return ConvertUtils.ChangeType<T>(result, defaultValue);
        }

        public Object ToScalar()
        {
            CreateSelectSql();

            return mDbHelper.ExecuteScalar(
                mSqlExpression.mSqlString, CommandType.Text, mSqlExpression.mParameters.ToArray());
        }

        public List<T> ToList<T>()
        {
            CreateSelectSql();

            //FillDataUtils 中包含reader 关闭的代码
            IDataReader reader = mDbHelper.GetDataReader(
                mSqlExpression.mSqlString, CommandType.Text, mSqlExpression.mParameters.ToArray());

            return FillDataUtils.FillCollection<T>(reader);
        }

        public T ToObject<T>()
        {
            CreateSelectSql();

            //FillDataUtils 中包含reader 关闭的代码
            IDataReader reader = mDbHelper.GetDataReader(
                mSqlExpression.mSqlString, CommandType.Text, mSqlExpression.mParameters.ToArray());

            return FillDataUtils.FillObject<T>(reader);
        }

        public DataTable ToDataTable()
        {
            CreateSelectSql();

            return mDbHelper.GetDataTable(
                mSqlExpression.mSqlString, CommandType.Text, mSqlExpression.mParameters.ToArray());
        }

        public DataRow ToDataRow()
        {
            CreateSelectSql();

            DataTable resultTable = mDbHelper.GetDataTable(
                mSqlExpression.mSqlString, CommandType.Text, mSqlExpression.mParameters.ToArray());

            if (resultTable.Rows.Count > 0)
                return resultTable.Rows[0];

            return null;
        }

        public Int32 ExecuteNonQuery()
        {
            if (mSqlExpression.mIsCustom)
                return mDbHelper.ExecuteNonQuery(
                    mSqlExpression.mSqlString, CommandType.Text, mSqlExpression.mParameters.ToArray());
            else {
                IDbCommand command = null;

                switch (mSqlExpression.mManipulation) {
                    case SqlExpression.Manipulation.Insert:
                        command = mDbHelper.CreateInsertCommand(
                            mSqlExpression.mTableName, mSqlExpression.mParameters.ToArray());
                        break;
                    case SqlExpression.Manipulation.Update:
                        if (mSqlExpression.mUpdateParameters.Count > 0) {
                            command = mDbHelper.CreateUpdateCommand(
                                 mSqlExpression.mTableName,
                                 mSqlExpression.mParameters.ToArray(),
                                 mSqlExpression.mUpdateParameters.ToArray());
                        }
                        break;
                    case SqlExpression.Manipulation.Delete:
                        command = mDbHelper.CreateDeleteCommand(
                            mSqlExpression.mTableName, mSqlExpression.mParameters.ToArray());
                        break;
                }

                if (command == null)
                    return 0;

                mSqlExpression.mSqlString = command.CommandText;

                return mDbHelper.ExecuteNonQuery(command);
            }
        }

        #endregion

        #region "私有函数"

        private void CreateSelectSql()
        {
            if ((!mSqlExpression.mIsCustom)
                && (mSqlExpression.mManipulation == SqlExpression.Manipulation.Select)) {
                String where = SqlBuilder.CreateWhereClause(
                    mSqlExpression.mParameters.ToArray(),
                    mDbHelper.SqlPlaceHolder);

                if (mSqlExpression.mQueryColumns.Count == 0)
                    mSqlExpression.mQueryColumns.Add("*");

                mSqlExpression.mCommandType = CommandType.Text;
                mSqlExpression.mSqlString = SqlBuilder.CreateSelectSql(
                    mSqlExpression.mTableName,
                    mSqlExpression.mQueryColumns.ToArray(),
                    where,
                    mSqlExpression.mOrderColumns);
            }
        }

        #endregion
    }
}
