using Common;
using DBHelper;
using System;
using System.Collections.Generic;
using System.Data;

namespace Repository.DAO
{
    public class GroupSelectionDAO
    {
        /// <summary>
        /// 添加组选区
        /// </summary>
        public static ARESULT AddNewGroupSelection(
            Int64 cellId,
            String data,
            ref Int64 id)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return ARESULT.E_FAIL;

            String sqlString = @"INSERT INTO groupselection (cellId, data) VALUES (@cellId, @data);
                                SELECT last_insert_rowid();";
            try {
                connection.BeginTransaction();
                Object obj = DbFactory.Instance.CreateSqlHelper(connection, sqlString)
                                .SetParameter("cellId", cellId)
                                .SetParameter("data", data)
                                .ToScalar<Object>();

                if (obj == null) {
                    id = -1;
                    connection.RollbackTransaction();
                    return ARESULT.E_FAIL;
                }
                else {
                    connection.CommitTransaction();
                    id = Int64.Parse(obj.ToString());
                    return ARESULT.S_OK;
                }
            }
            catch (Exception ex) {
                Tracker.LogE(ex);
                connection.RollbackTransaction();
                return ARESULT.E_FAIL;
            }
            finally {
                DBConnection.Instance.ReturnDBConnection(connection);
            }
        }

        /// <summary>
        /// 更新组选区
        /// </summary>
        public static ARESULT UpdateGroupSelection(
            Int64 groupSelectionId,
            String data)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return ARESULT.E_FAIL;

            String queryString = "UPDATE groupselection SET data=@data WHERE id=@id";

            try {
                connection.BeginTransaction();
                Int32 ret = DbFactory.Instance.CreateSqlHelper(connection, queryString)
                                .SetParameter("id", groupSelectionId)
                                .SetParameter("data", data)
                                .ExecuteNonQuery();
                connection.CommitTransaction();

                return (ret >= 1 ? ARESULT.S_OK : ARESULT.E_FAIL);
            }
            catch (Exception ex) {
                Tracker.LogE(ex);
                connection.RollbackTransaction();
                return ARESULT.E_FAIL;
            }
            finally {
                DBConnection.Instance.ReturnDBConnection(connection);
            }
        }

        /// <summary>
        /// 删除组选区
        /// </summary>
        public static ARESULT RemoveGroupSelection(
            Int64 groupSelectionId)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return ARESULT.E_FAIL;

            try {
                connection.BeginTransaction();
                Int32 ret = DbFactory.Instance.CreateDeleteHelper(connection, "groupselection")
                  .SetParameter("id", groupSelectionId)
                  .ExecuteNonQuery();
                connection.CommitTransaction();

                return (ret >= 1 ? ARESULT.S_OK : ARESULT.E_FAIL);
            }
            catch (Exception ex) {
                connection.RollbackTransaction();
                Tracker.LogE(ex);
                return ARESULT.E_FAIL;
            }
            finally {
                DBConnection.Instance.ReturnDBConnection(connection);
            }
        }

        /// <summary>
        /// 删除全部组选区
        /// </summary>
        public static ARESULT RemoveAllGroupSelection(
            Int64 cellId)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return ARESULT.E_FAIL;

            try {
                connection.BeginTransaction();

                String queryString = "DELETE FROM groupselection WHERE cellId=@cellId";
                Int32 ret = DbFactory.Instance.CreateSqlHelper(connection, queryString)
                    .SetParameter("cellId", cellId)
                    .ExecuteNonQuery();

                connection.CommitTransaction();
                return ARESULT.S_OK;
            }
            catch (Exception ex) {
                connection.RollbackTransaction();
                Tracker.LogE(ex);
                return ARESULT.E_FAIL;
            }
            finally {
                DBConnection.Instance.ReturnDBConnection(connection);
            }
        }

        /// <summary>
        /// 获取所有选区组
        /// </summary>
        public static List<String> GetGroupSelectionList(
            Int64 cellId)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return null;

            try {
                String queryString = "SELECT * FROM groupselection WHERE cellId=@cellId ORDER BY id ASC";
                DataTable dt = DbFactory.Instance.CreateSqlHelper(connection, queryString)
                    .SetParameter("cellId", cellId)
                    .ToDataTable();

                if ((dt == null) || (dt.Rows.Count <= 0))
                    return null;

                List<String> groupList = new List<String>();
                foreach (DataRow dr in dt.Rows) {
                    String str = dr["id"].ToString() + "," + dr["data"].ToString();
                    if (!String.IsNullOrEmpty(str))
                        groupList.Add(str);
                }

                return groupList;
            }
            catch (Exception ex) {
                Tracker.LogE(ex);
                return null;
            }
            finally {
                 DBConnection.Instance.ReturnDBConnection(connection);
            }
        }
    }
}
