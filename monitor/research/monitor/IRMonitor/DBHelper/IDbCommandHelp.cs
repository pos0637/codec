using System;
using System.Data;

namespace DBHelper
{
    public interface IDbCommandHelp
    {
        /// <summary>
        /// SQL占位符
        /// </summary>
        String SqlPlaceHolder { get; }

        /// <summary>
        /// 初始化 System.Data.IDataParameter 类的新实例
        /// </summary>
        /// <exception cref="System.Exception">其他异常。</exception>
        /// <returns></returns>
        IDataParameter CreateParameter();

        /// <summary>
        /// 用参数名称和新 System.Data.IDataParameter 的一个值初始化 System.Data.IDataParameter 类的新实例。
        /// </summary>
        /// <param name="parameterName">要映射的参数的名称</param>
        /// <param name="value">一个 System.Object，它是 System.Data.IDataParameter 的值。</param>
        /// <exception cref="System.Exception">其他异常。</exception>
        /// <returns></returns>
        IDataParameter CreateParameter(String parameterName, Object value);

        /// <summary>
        /// 用参数名称和数据类型初始化 System.Data.IDataParameter 类的新实例。
        /// </summary>
        /// <param name="parameterName">要映射的参数的名称。</param>
        /// <param name="dataType">数据类型</param>
        /// <exception cref="System.ArgumentException">dataType 参数中提供的值为无效的后端数据类型。</exception>
        /// <exception cref="System.Exception">其他异常。</exception>
        /// <returns></returns>
        IDataParameter CreateParameter(String parameterName, Type dataType);

        /// <summary>
        /// 用参数名称和数据类型初始化 System.Data.IDataParameter 类的新实例。
        /// </summary>
        /// <param name="parameterName">要映射的参数的名称。</param>
        /// <param name="dataType">数据类型。</param>
        /// <exception cref="System.ArgumentException">dataType 参数中提供的值为无效的后端数据类型。</exception>
        /// <exception cref="System.Exception">其他异常。</exception>
        /// <returns></returns>
        IDataParameter CreateParameter(String parameterName, ColumnType dataType);

        /// <summary>
        /// 用参数名称、新 System.Data.IDataParameter 的一个值和数据类型初始化 System.Data.IDataParameter 类的新实例。
        /// </summary>
        /// <param name="parameterName">要映射的参数的名称。</param>
        /// <param name="value">一个 System.Object，它是 System.Data.IDataParameter 的值。</param>
        /// <param name="dataType">数据类型。</param>
        /// <exception cref="System.ArgumentException">dataType 参数中提供的值为无效的后端数据类型。</exception>
        /// <exception cref="System.Exception">其他异常。</exception>
        /// <returns></returns>
        IDataParameter CreateParameter(String parameterName, Object value, ColumnType dataType);

        /// <summary>
        /// 用参数名称、数据类型和大小初始化 System.Data.IDataParameter 类的新实例。
        /// </summary>
        /// <param name="parameterName">要映射的参数的名称。</param>
        /// <param name="dataType">数据类型。</param>
        /// <param name="size">数据大小</param>
        /// <exception cref="System.ArgumentException">dataType 参数中提供的值为无效的后端数据类型。</exception>
        /// <exception cref="System.Exception">其他异常。</exception>
        /// <returns></returns>
        IDataParameter CreateParameter(String parameterName, ColumnType dataType, Int32 size);

        /// <summary>
        /// 用参数名称、新 System.Data.IDataParameter 的一个值、数据类型和大小初始化 System.Data.IDataParameter 类的新实例。
        /// </summary>
        /// <param name="parameterName">要映射的参数的名称。</param>
        /// <param name="value">一个 System.Object，它是 System.Data.IDataParameter 的值。</param>
        /// <param name="dataType">数据类型。</param>
        /// <param name="size">数据大小</param>
        /// <exception cref="System.ArgumentException">dataType 参数中提供的值为无效的后端数据类型。</exception>
        /// <exception cref="System.Exception">其他异常。</exception>
        /// <returns></returns>
        IDataParameter CreateParameter(String parameterName, Object value, ColumnType dataType, Int32 size);

        /// <summary>
        /// 创建 Delete 操作的 DbCommand。
        /// </summary>
        /// <param name="sourceTableName">要删除记录的表的名称。</param>
        /// <param name="conditionParameters">要删除的记录的参数。</param>
        /// <exception cref="System.InvalidCastException">传递的参数不是 ADO中相应Helper的 IDataParameter。</exception>
        /// <exception cref="System.ArgumentNullException">conditionParameters 中有参数为null。</exception>
        /// <exception cref="System.Exception">其他异常。</exception>
        /// <returns></returns>
        IDbCommand CreateDeleteCommand(String sourceTableName, params IDataParameter[] conditionParameters);

        /// <summary>
        /// 创建 Insert 操作的 DbCommand。
        /// </summary>
        /// <param name="sourceTableName">要插入记录的表的名称。</param>
        /// <param name="parameters">要插入的记录的参数。</param>
        /// <exception cref="System.InvalidCastException">传递的参数不是 ADO中相应Helper的 IDataParameter。</exception>
        /// <exception cref="System.ArgumentNullException">parameters 中有参数为null。</exception>
        /// <exception cref="System.Exception">其他异常。</exception>
        /// <returns></returns>
        IDbCommand CreateInsertCommand(String sourceTableName, params IDataParameter[] parameters);

        /// <summary>
        /// 创建 Update 操作的 DbCommand。
        /// </summary>
        /// <param name="sourceTableName">要更新记录的表的名称。</param>
        /// <param name="conditionParameters">要更新的记录的条件参数。</param>
        /// <param name="parameters">要更新的记录的参数。</param>
        /// <exception cref="System.InvalidCastException">传递的参数不是 ADO中相应Helper的 IDataParameter。</exception>
        /// <exception cref="System.ArgumentNullException">conditionParameters/parameters 中有参数为null。</exception>
        /// <exception cref="System.Exception">其他异常。</exception>
        /// <returns></returns>
        IDbCommand CreateUpdateCommand(String sourceTableName, IDataParameter[] conditionParameters
            , params IDataParameter[] parameters);

        /// <summary>
        /// 创建 Update 操作的DbCommand。
        /// </summary>
        /// <param name="sourceTableName">要更新记录的表的名称。</param>
        /// <param name="condition">要更新记录的条件。</param>
        /// <param name="parameters">要更新的记录的参数。</param>
        /// <exception cref="System.InvalidCastException">传递的参数不是 ADO中相应Helper的 IDataParameter。</exception>
        /// <exception cref="System.ArgumentNullException">conditionParameters/parameters 中有参数为null。</exception>
        /// <exception cref="System.Exception">其他异常。</exception>
        /// <returns></returns>
        IDbCommand CreateUpdateCommand(String sourceTableName, String condition, params IDataParameter[] parameters);
    }
}
