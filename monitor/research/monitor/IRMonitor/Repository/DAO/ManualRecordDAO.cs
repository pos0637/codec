using Common;
using DBHelper;
using Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace Repository.DAO
{
    public class ManualRecordDAO
    {
        /// <summary>
        /// 添加手动录像记录
        /// </summary>
        public static ARESULT AddManualRecord(
            Int64 cellId,
            Int64 recordId,
            ref Int64 id)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return ARESULT.E_FAIL;

            try {
                connection.BeginTransaction();

                DateTime time = DateTime.Now;
                String queryString = @"INSERT INTO manualrecord (cellId, name, startTime, startRecordIndex)
                                       VALUES (@cellId, @name, @startTime, @startRecordIndex);
                                       SELECT last_insert_rowid();";
                Object obj = DbFactory.Instance.CreateSqlHelper(connection, queryString)
                    .SetParameter("cellId", cellId)
                    .SetParameter("name", time.ToString("yyyyMMddHHmmssfff"))
                    .SetParameter("startTime", time)
                    .SetParameter("startRecordIndex", recordId)
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
        /// 更新手动录像记录
        /// </summary>
        public static ARESULT UpdateManualRecord(
            Int64 manualRecordId,
            Int64 recordId)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return ARESULT.E_FAIL;

            try {
                connection.BeginTransaction();

                String queryString = "UPDATE manualrecord SET endRecordIndex=@endRecordIndex, endTime=@endTime WHERE id=@id";
                Int32 ret  = DbFactory.Instance.CreateSqlHelper(connection, queryString)
                    .SetParameter("endRecordIndex", recordId)
                    .SetParameter("endTime", DateTime.Now)
                    .SetParameter("id", manualRecordId)
                    .ExecuteNonQuery();

                connection.CommitTransaction();
                return (ret == 1 ? ARESULT.S_OK : ARESULT.E_FAIL);
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
        /// 获取手动录像记录
        /// </summary>
        public static List<Record> GetManualRecordList(
            DateTime startTime,
            DateTime endTime,
            String recordName,
            Int32 offset,
            Int32 count)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return null;

            try {
                String sqlString = @"SELECT id, name, startTime, startRecordIndex, endRecordIndex FROM manualrecord WHERE
                                    startTime>=@startTime AND endTime<=@endTime
                                    and startRecordIndex<>-1 and endRecordIndex<>-1
                                    and (endTime IS NOT NULL) AND name LIKE @name
                                    ORDER BY id ASC LIMIT @count OFFSET @offset";

                // 模糊查询条件
                String fuzzy = null;
                if (String.IsNullOrEmpty(recordName))
                    fuzzy = "%%";
                else
                    fuzzy = String.Format("%{0}%", recordName);

                DataTable dt = DbFactory.Instance.CreateSqlHelper(connection, sqlString)
                    .SetParameter("startTime", startTime)
                    .SetParameter("endTime", endTime)
                    .SetParameter("name", fuzzy)
                    .SetParameter("count", count)
                    .SetParameter("offset", offset)
                    .ToDataTable();

                if ((dt == null) || (dt.Rows.Count <= 0))
                    return null;

                List<Record> recordList = new List<Record>();

                sqlString = "SElECT * FROM record WHERE";
                foreach (DataRow dr in dt.Rows) {
                    Record info = new Record();
                    info.mType = 0;
                    info.mId = Int64.Parse(dr["id"].ToString());
                    info.mStartTime = Convert.ToDateTime(dr["startTime"]);
                    info.mStartId = Convert.ToInt32(dr["startRecordIndex"]);
                    info.mEndId = Convert.ToInt32(dr["endRecordIndex"]);
                    info.mName = dr["name"].ToString();
                    if (info.mStartId > info.mEndId)
                        continue;

                    String condition = String.Format("(id BETWEEN {0} AND {1})", info.mStartId, info.mEndId);
                    sqlString = String.Format("{0} {1} OR", sqlString, condition);
                    recordList.Add(info);
                }

                sqlString = sqlString.Substring(0, sqlString.Length - 2);
                dt = DbFactory.Instance.CreateSqlHelper(connection, sqlString)
                    .ToDataTable();

                if ((dt == null) || (dt.Rows.Count <= 0))
                    return null;

                Dictionary<Int64, String> dict = new Dictionary<Int64, String>();
                foreach (DataRow dr in dt.Rows) {
                    Int64 id = Int64.Parse(dr["id"].ToString());
                    String fileName = dr["infoFileName"].ToString();
                    dict.Add(id, fileName);
                }

                for (Int32 i = 0; i < recordList.Count; i++) {
                    Record item = recordList[i];
                    List<String> files = new List<String>();
                    for (Int64 j = item.mStartId; j <= item.mEndId; j++) {
                        if (dict.ContainsKey(j) && (dict[j] != null))
                            files.Add(dict[j]);
                    }

                    if (ARESULT.AFAILED(RecordDAO.GetRecordMessage(files,
                        ref item.mRecordTime, ref item.mImageData))) {
                        recordList.RemoveAt(i);
                        i--;
                        continue;
                    }
                }

                return recordList;
            }
            catch (Exception ex) {
                Tracker.LogE(ex);
                return null;
            }
            finally {
                DBConnection.Instance.ReturnDBConnection(connection);
            }
        }

        /// <summary>
        /// 删除手动录像记录
        /// </summary>
        public static ARESULT RemovetManualRecord(
            List<Int64> manualRecordIdList)
        {
            if ((manualRecordIdList == null) || (manualRecordIdList.Count <= 0))
                return ARESULT.E_FAIL;

            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return ARESULT.E_FAIL;

            try {
                connection.BeginTransaction();

                String sqlString = @"DELETE FROM manualrecord WHERE id IN (";

                foreach (Int64 manualRecordId in manualRecordIdList)
                    sqlString += manualRecordId.ToString() + ",";
                sqlString = sqlString.Substring(0, sqlString.Length - 1);
                sqlString += ")";

                Int32 ret = DbFactory.Instance.CreateSqlHelper(connection, sqlString)
                      .ExecuteNonQuery();

                connection.CommitTransaction();
                return (ret > 0 ? ARESULT.S_OK : ARESULT.E_FAIL);
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
    }
}
