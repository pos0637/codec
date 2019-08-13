using Common;
using DBHelper;
using System;
using System.Collections.Generic;
using System.Data;

namespace Repository.DAO
{
    public class SelectionDAO
    {
        /// <summary>
        /// 添加选区
        /// </summary>
        public static ARESULT AddSelection(
            Int64 cellId,
            String data,
            Boolean isGlobalSelection,
            ref Int64 id)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return ARESULT.E_FAIL;

            String sqlString = @"INSERT INTO selection(cellId, data, IsGlobalSelection) VALUES 
                                (@cellId, @data, @IsGlobalSelection);
                                SELECT last_insert_rowid();";

            try {
                connection.BeginTransaction();
                Object obj = DbFactory.Instance.CreateSqlHelper(connection, sqlString)
                                .SetParameter("cellId", cellId)
                                .SetParameter("data", data)
                                .SetParameter("IsGlobalSelection", (isGlobalSelection ? 1 : 0))
                                .ToScalar<Object>();

                if (obj == null) {
                    id = -1;
                    connection.RollbackTransaction();
                    return ARESULT.E_FAIL;
                }
                else {
                    id = Int64.Parse(obj.ToString());
                    connection.CommitTransaction();
                    return ARESULT.S_OK;
                }
            }
            catch (Exception e) {
                Tracker.LogE(e);
                connection.RollbackTransaction();
                return ARESULT.E_FAIL;
            }
            finally {
                DBConnection.Instance.ReturnDBConnection(connection);
            }
        }

        /// <summary>
        /// 更新选区
        /// </summary>
        public static ARESULT UpdateSelection(
            Int64 selectionId,
            String selectionInfo)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return ARESULT.E_FAIL;

            String queryString = "UPDATE selection SET data=@data WHERE id=@id";

            try {
                connection.BeginTransaction();
                Int32 ret = DbFactory.Instance.CreateSqlHelper(connection, queryString)
                                .SetParameter("id", selectionId)
                                .SetParameter("data", selectionInfo)
                                .ExecuteNonQuery();
                connection.CommitTransaction();
                return (ret == 1 ? ARESULT.S_OK : ARESULT.E_FAIL);
            }
            catch (Exception e) {
                Tracker.LogE(e);
                connection.RollbackTransaction();
                return ARESULT.E_FAIL;
            }
            finally {
                DBConnection.Instance.ReturnDBConnection(connection);
            }
        }

        /// <summary>
        /// 删除选区
        /// </summary>
        public static ARESULT RemoveSelection(
            Int64 selectionId)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return ARESULT.E_FAIL;

            try {
                connection.BeginTransaction();
                Int32 ret = DbFactory.Instance.CreateDeleteHelper(connection, "selection")
                    .SetParameter("id", selectionId)
                    .ExecuteNonQuery();

                connection.CommitTransaction();
                return (ret == 1 ? ARESULT.S_OK : ARESULT.E_FAIL);
            }
            catch (Exception e) {
                Tracker.LogE(e);
                connection.RollbackTransaction();
                return ARESULT.E_FAIL;
            }
            finally {
                DBConnection.Instance.ReturnDBConnection(connection);
            }
        }

        /// <summary>
        /// 删除全部选区
        /// </summary>
        public static ARESULT RemoveAllSelection(
            Int64 cellId)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return ARESULT.E_FAIL;

            try {
                connection.BeginTransaction();
                String queryString = "DELETE FROM selection WHERE cellId=@cellId";
                DbFactory.Instance.CreateSqlHelper(connection, queryString)
                    .SetParameter("cellId", cellId)
                    .ExecuteNonQuery();
                connection.CommitTransaction();

                return ARESULT.S_OK;
            }
            catch (Exception e) {
                Tracker.LogE(e);
                connection.RollbackTransaction();
                return ARESULT.E_FAIL;
            }
            finally {
                DBConnection.Instance.ReturnDBConnection(connection);
            }
        }

        /// <summary>
        /// 获取全局选区数据
        /// </summary>
        public static String GetGlobalSelectionData(
            Int64 cellId)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return null;

            try {
                String queryString = "SELECT data FROM selection WHERE cellId=@cellId AND IsGlobalSelection=@IsGlobalSelection";
                String global = DbFactory.Instance.CreateSqlHelper(connection, queryString)
                    .SetParameter("cellId", cellId)
                    .SetParameter("IsGlobalSelection", 1)
                    .ToScalar<String>();
                return global;
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return null;
            }
            finally {
                DBConnection.Instance.ReturnDBConnection(connection);
            }
        }

        /// <summary>
        /// 获取选区链表
        /// </summary>
        public static List<String> GetSelectionList(
            Int64 cellId)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return null;

            try {
                String queryString = "SELECT * FROM selection WHERE cellId=@cellId ORDER BY id ASC";
                DataTable dt = DbFactory.Instance.CreateSqlHelper(connection, queryString)
                    .SetParameter("cellId", cellId)
                    .ToDataTable();
                if ((dt == null) || (dt.Rows.Count <= 0))
                    return null;

                List<String> strList = new List<String>();
                foreach (DataRow dr in dt.Rows) {
                    String str = dr["id"].ToString() + "," + dr["data"].ToString();
                    if (!String.IsNullOrEmpty(str))
                        strList.Add(str);
                }
                return strList;
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return null;
            }
            finally {
                DBConnection.Instance.ReturnDBConnection(connection);
            }
        }

        /// <summary>
        /// 修正选区链表
        /// </summary>
        public static ARESULT FixSelectionList(Int64 cellId)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return ARESULT.E_FAIL;

            try {
                String queryString = "SELECT id FROM selection WHERE cellId=@cellId AND IsGlobalSelection=@IsGlobalSelection ORDER BY id ASC";
                DataTable dt = DbFactory.Instance.CreateSqlHelper(connection, queryString)
                    .SetParameter("cellId", cellId)
                    .SetParameter("IsGlobalSelection", 0)
                    .ToDataTable();
                if ((dt != null) && (dt.Rows.Count > 0)) {
                    List<String> strList = new List<String>();
                    foreach (DataRow dr in dt.Rows) {
                        String str = dr["id"].ToString();
                        if (!String.IsNullOrEmpty(str))
                            strList.Add(str);
                    }

                    queryString = @"INSERT INTO selection(cellId, data, IsGlobalSelection) 
                                SELECT cellId, data, IsGlobalSelection FROM selection WHERE cellId=@cellId AND IsGlobalSelection=0
                                ORDER BY id ASC";

                    connection.BeginTransaction();
                    Int32 ret = DbFactory.Instance.CreateSqlHelper(connection, queryString)
                                    .SetParameter("cellId", cellId)
                                    .ExecuteNonQuery();
                    connection.CommitTransaction();
                    if ((ret > 0) && (strList.Count > 0)) {
                        String str = String.Join(",", strList);
                        connection.BeginTransaction();
                        queryString = "DELETE FROM selection WHERE id IN (" + str + ")";
                        DbFactory.Instance.CreateSqlHelper(connection, queryString)
                            .ExecuteNonQuery();
                        connection.CommitTransaction();
                    }
                }

                return ARESULT.S_OK;
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return ARESULT.E_FAIL; ;
            }
            finally {
                DBConnection.Instance.ReturnDBConnection(connection);
            }
        }
    }
}
