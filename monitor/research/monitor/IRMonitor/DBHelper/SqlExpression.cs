using System;
using System.Collections.Generic;
using System.Data;

namespace DBHelper
{
    public class SqlExpression
    {
        /// <summary>
        /// 是否自定义
        /// </summary>
        public Boolean mIsCustom { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public List<IDataParameter> mParameters = new List<IDataParameter>();

        #region "非自定义Sql时属性"

        /// <summary>
        /// 操作语言类型
        /// </summary>
        public enum Manipulation
        {
            Select = 0,
            Insert,
            Update,
            Delete
        }

        /// <summary>
        /// 表名
        /// </summary>
        public String mTableName { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public Manipulation mManipulation { get; set; }

        /// <summary>
        /// 执行Update语句时要更新的字段参数，只在 Manipulation.Update 时有效
        /// </summary>
        public List<IDataParameter> mUpdateParameters = new List<IDataParameter>();

        /// <summary>
        /// 查询的列，只在 Manipulation.Select 时有效
        /// </summary>
        public List<String> mQueryColumns = new List<String>();

        /// <summary>
        /// 排序列，只在 Manipulation.Select 时有效
        /// </summary>
        public List<SqlOrder> mOrderColumns = new List<SqlOrder>();

        #endregion

        #region "自定义SQL时属性"

        /// <summary>
        /// 自定义SQL语句
        /// </summary>
        public String mSqlString { get; set; }

        /// <summary>
        /// CommandType
        /// </summary>
        public CommandType mCommandType { get; set; }

        #endregion

        #region "公有函数"

        #region "mParameters操作"

        public void AddParameter(IDataParameter parameter)
        {
            mParameters.Add(parameter);
        }

        public void RemoveParameter(String parameterName)
        {
            RemoveParameter(mParameters.Find(t => t.ParameterName.Equals(parameterName)));
        }

        public void RemoveParameter(IDataParameter parameter)
        {
            mParameters.Remove(parameter);
        }

        public void ClearParameter()
        {
            mParameters.Clear();
        }

        #endregion

        #region "mUpdateParameters操作"

        public void AddUpdateParameter(IDataParameter parameter)
        {
            mUpdateParameters.Add(parameter);
        }

        public void RemoveUpdateParameter(String parameterName)
        {
            RemoveUpdateParameter(mUpdateParameters.Find(t => t.ParameterName.Equals(parameterName)));
        }

        public void RemoveUpdateParameter(IDataParameter parameter)
        {
            mUpdateParameters.Remove(parameter);
        }

        public void ClearUpdateParameter()
        {
            mUpdateParameters.Clear();
        }

        #endregion

        #region "mQueryColumns操作"

        public void AddQueryColumns(String columnName)
        {
            mQueryColumns.Add(columnName);
        }

        public void RemoveQueryColumns(String columnName)
        {
            mQueryColumns.Remove(columnName);
        }

        public void ClearQueryColumns()
        {
            mQueryColumns.Clear();
        }

        #endregion

        #region "mOrderFields操作"

        public void AddOrderColumn(SqlOrder columnName)
        {
            mOrderColumns.Add(columnName);
        }

        public void RemoveOrderColumn(SqlOrder columnName)
        {
            mOrderColumns.Remove(columnName);
        }

        public void ClearOrderColumn()
        {
            mOrderColumns.Clear();
        }

        #endregion

        #endregion
    }

    public class SqlOrder
    {
        public enum OrderType
        {
            ASC,
            DESC
        }

        public String mOrderColumns { get; set; }

        public OrderType mType { get; set; }

        public override String ToString()
        {
            return mOrderColumns + " " + mType;
        }
    }
}
