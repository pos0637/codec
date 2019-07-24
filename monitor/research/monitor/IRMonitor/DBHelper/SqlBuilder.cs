using System;
using System.Collections.Generic;
using System.Data;

namespace DBHelper
{
    public class SqlBuilder
    {
        private SqlBuilder()
        {
        }

        public static String CreateDeleteSql(String sourceTableName, String where)
        {
            return String.Format("DELETE FROM {0} {1}"
                , sourceTableName
                , where);
        }

        public static String CreateInsertSql(String sourceTableName, String[] sourceColumns,
            String sqlParamPlaceHolder)
        {
            String[] sqlParameterName = GetSqlParameterName(sourceColumns, sqlParamPlaceHolder);
            return String.Format("INSERT INTO {0}({1}) VALUES({2}) "
                , sourceTableName
                , String.Join(", ", sourceColumns)
                , String.Join(", ", sqlParameterName));
        }

        public static String CreateSelectSql(String sourceTableName, String[] sourceColumns,
            String where, List<SqlOrder> orders)
        {
            return String.Format("SELECT {0} FROM {1} {2} {3}"
                , String.Join(", ", sourceColumns)
                , sourceTableName
                , where
                , ((orders.Count == 0)
                    ? String.Empty
                    : ("ORDER BY " + String.Join(", ", orders))));
        }

        public static String CreateUpdateSql(String sourceTableName, String[] sourceColumns,
            String where, String sqlParamPlaceHolder)
        {
            String[] sqlParamName = GetSqlParameterName(sourceColumns, sqlParamPlaceHolder);
            String[] strArray2 = new String[sourceColumns.Length];

            for (Int32 i = 0; i < sourceColumns.Length; i++) {
                strArray2[i] = sourceColumns[i] + " = " + sqlParamName[i];
            }
            return String.Format("UPDATE {0} SET {1} {2} "
                , sourceTableName
                , String.Join(", ", strArray2)
                , where);
        }

        public static String CreateWhereClause(IDataParameter[] whereParams, String sqlParamPlaceHolder)
        {
            Int32 paramsLength = whereParams.Length;
            if (paramsLength == 0)
                return String.Empty;

            String[] strArray = new String[paramsLength];
            for (Int32 i = 0; i < paramsLength; i++) {
                if (whereParams[i].SourceVersion == DataRowVersion.Original) {
                    strArray[i] = whereParams[i].ParameterName.Substring("Original_".Length)
                        + " = "
                        + GetSqlParameterName(whereParams[i].ParameterName, sqlParamPlaceHolder);
                }
                else {
                    strArray[i] = whereParams[i].ParameterName
                        + " = "
                        + GetSqlParameterName(whereParams[i].ParameterName, sqlParamPlaceHolder);
                }
            }

            return ("WHERE " + String.Join(" AND ", strArray));
        }

        public static String GetFormatedSql(IDbCommand cmd, String sqlParamPlaceHolder)
        {
            if (cmd == null)
                return String.Empty;

            String commandText = cmd.CommandText;
            try {
                String[] strArray;
                Int32 num;
                if ((cmd.Parameters == null) || (cmd.Parameters.Count == 0))
                    return commandText;

                if (cmd.CommandType == CommandType.Text) {
                    strArray = commandText.Split(sqlParamPlaceHolder.ToCharArray());
                    if (sqlParamPlaceHolder == "?") {
                        for (num = 0; num < cmd.Parameters.Count; num++) {
                            strArray[num + 1] = String.Format("{0}{1}"
                                , GetFormatParamValue((IDataParameter)cmd.Parameters[num])
                                , strArray[num + 1]);
                        }
                    }
                    else {
                        num = 0;
                        while (num < cmd.Parameters.Count) {
                            strArray[num + 1] = (sqlParamPlaceHolder + strArray[num + 1]).Replace(
                                sqlParamPlaceHolder + ((IDataParameter)cmd.Parameters[num]).ParameterName
                                , GetFormatParamValue((IDataParameter)cmd.Parameters[num]));
                            num++;
                        }
                    }
                    commandText = String.Join("", strArray);
                    Int32 index = commandText.IndexOf(" where ");
                    if (index > 0)
                        commandText = commandText.Substring(0, index)
                            + commandText.Substring(index).Replace("= ''", " is null ");

                    return commandText;
                }

                strArray = new String[cmd.Parameters.Count];
                for (num = 0; num < cmd.Parameters.Count; num++) {
                    strArray[num] = String.Format("'{0}'", Convert.ToString(((IDataParameter)cmd.Parameters[num]).Value));
                }
                if (((IDataParameter)cmd.Parameters[0]).Direction == ParameterDirection.ReturnValue) {
                    commandText = String.Format("{0}={1}({2})", strArray[0], commandText, String.Join(",", strArray, 1, strArray.Length - 1));
                }
                else {
                    commandText = String.Format("{0}({1})", commandText, String.Join(",", strArray));
                }
            }
            catch {
            }

            return commandText;
        }

        private static String GetFormatParamValue(IDataParameter parameter)
        {
            return String.Format("'{0}'", Convert.ToString(parameter.Value));
        }

        public static String GetSqlParameterName(String paramName, String sqlParamPlaceHolder)
        {
            if (sqlParamPlaceHolder == "?")
                return sqlParamPlaceHolder;

            return (sqlParamPlaceHolder + paramName);
        }

        public static String[] GetSqlParameterName(String[] sourceColumns, String sqlParamPlaceHolder)
        {
            Int32 num;
            String[] strArray = new String[sourceColumns.Length];

            if (sqlParamPlaceHolder == "?") {
                for (num = 0; num < sourceColumns.Length; num++) {
                    strArray[num] = "?";
                }
            }
            else {
                for (num = 0; num < sourceColumns.Length; num++) {
                    strArray[num] = sqlParamPlaceHolder + sourceColumns[num];
                }
            }

            return strArray;
        }

        public static String GetTableNameFromSQL(String sql)
        {
            Int32 index = sql.ToUpper().IndexOf(" FROM ");
            String str = sql.Substring(index + 6);
            index = str.IndexOf(" ");
            if (index >= 0) {
                str = str.Substring(0, index);
            }
            else {
                index = str.IndexOf(",");
                if (index >= 0) {
                    str = str.Substring(0, index);
                }
            }
            str = str.Trim();
            if (String.IsNullOrEmpty(str)) {
                str = "Table";
            }
            return str;
        }
    }
}
