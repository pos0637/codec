using Common;
using DBHelper;
using Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace Repository.DAO
{
    public class AlarmDAO
    {
        /// <summary>
        /// 获取未结束的告警
        /// </summary>
        public static List<UnresolvedAlarm> GetUnresolvedAlarms(Int64 cellId)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return null;

            try {
                String sqlString = @"SELECT * FROM alarm WHERE endTime IS NULL";
                DataTable dt = DbFactory.Instance.CreateSqlHelper(connection, sqlString)
                    .ToDataTable();
                List<UnresolvedAlarm> alarmList = new List<UnresolvedAlarm>();
                if ((dt != null) && (dt.Rows.Count > 0)) {
                    foreach (DataRow dr in dt.Rows) {
                        UnresolvedAlarm alarm = new UnresolvedAlarm();
                        alarm.mCellId = Int64.Parse(dr["cellId"].ToString());
                        alarm.mMode = Int32.Parse(dr["Mode"].ToString());
                        alarm.mId = Int64.Parse(dr["selectionId"].ToString());
                        alarm.mType = Int32.Parse(dr["type"].ToString());
                        alarm.mAlarmLevel = Int32.Parse(dr["level"].ToString());
                        alarmList.Add(alarm);
                    }
                }
                return alarmList;
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
        /// 添加告警
        /// </summary>
        public static ARESULT AddAlarmInfo(
            Int64 cellId,
            Int32 alarmMode,
            Int32 alarmType,
            Int32 alarmReason,
            Int32 alarmLevel,
            String alarmDatail,
            Int32 stride,
            Int32 height,
            Int64 selectionId,
            String selectionData,
            String temperatureInfo,
            String imagePath,
            String alarmTemp,
            Single resultTemp,
            Int64 recordId,
            Int32 routineId,
            Int64 irParamId,
            Boolean status)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return ARESULT.E_FAIL;

            try {
                connection.BeginTransaction();

                DateTime time = DateTime.Now;

                Int32 ret = DbFactory.Instance.CreateInsertHelper(connection, "alarm")
                    .SetParameter("cellId", cellId)
                    .SetParameter("name", time.ToString("yyyyMMddHHmmssfff"))
                    .SetParameter("selectionId", selectionId)
                    .SetParameter("mode", alarmMode)
                    .SetParameter("type", alarmType)
                    .SetParameter("reason", alarmReason)
                    .SetParameter("level", alarmLevel)
                    .SetParameter("condition", alarmTemp)
                    .SetParameter("result", resultTemp)
                    .SetParameter("detail", alarmDatail)
                    .SetParameter("selectionData", selectionData)
                    .SetParameter("temperatureInfo", temperatureInfo)
                    .SetParameter("startTime", time)
                    .SetParameter("stride", stride)
                    .SetParameter("height", height)
                    .SetParameter("image", imagePath)
                    .SetParameter("startRecordIndex", recordId)
                    .SetParameter("status", (status ? 1 : 0))
                    .SetParameter("routineId", routineId)
                    .SetParameter("irParamId", irParamId)
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
        /// 结束告警
        /// </summary>
        public static ARESULT UpdateAlarmInfo(
            Int64 cellId,
            Int64 selectionId,
            Int32 alarmMode,
            Int32 alarmType)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return ARESULT.E_FAIL;

            try {
                connection.BeginTransaction();
                String sqlString = @"UPDATE alarm SET endTime=@endTime WHERE id IN (SELECT id FROM alarm WHERE cellId=@cellId AND
                                    selectionId=@selectionId AND type=@type AND mode=@mode AND endTime IS NULL)";

                Int32 ret = DbFactory.Instance.CreateSqlHelper(connection, sqlString)
                        .SetParameter("cellId", cellId)
                        .SetParameter("selectionId", selectionId)
                        .SetParameter("mode", alarmMode)
                        .SetParameter("type", alarmType)
                        .SetParameter("endTime", DateTime.Now)
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

        /// <summary>
        /// 结束监控单元所有未结束的告警
        /// </summary>
        public static ARESULT UpdateAlarmInfo(
            Int64 cellId)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return ARESULT.E_FAIL;

            try {
                connection.BeginTransaction();
                String sql = @"UPDATE alarm SET endTime=@endTime, endRecordIndex=startRecordIndex WHERE
                             cellId= @cellId AND endTime IS NULL";
                Int32 ret = DbFactory.Instance.CreateSqlHelper(connection, sql)
                   .SetParameter("cellId", cellId)
                   .SetParameter("endTime", DateTime.Now)
                   .ExecuteNonQuery();

                connection.CommitTransaction();
                return (ret >= 0 ? ARESULT.S_OK : ARESULT.E_FAIL);
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
        /// 结束告警录像
        /// </summary>
        public static ARESULT UpdateAlarmRecord(
            Int64 cellId,
            Int64 selectionId,
            Int64 recordId,
            Int32 alarmMode,
            Int32 alarmType)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return ARESULT.E_FAIL;

            try {
                connection.BeginTransaction();
                String sqlString = @"UPDATE alarm SET endRecordIndex=@endRecordIndex WHERE id IN (SELECT id FROM
                                    alarm WHERE cellId=@cellId AND selectionId=@selectionId AND type=@type AND mode=@mode
                                    AND endTime IS NULL)";

                Int32 ret = DbFactory.Instance.CreateSqlHelper(connection, sqlString)
                        .SetParameter("cellId", cellId)
                        .SetParameter("selectionId", selectionId)
                        .SetParameter("mode", alarmMode)
                        .SetParameter("type", alarmType)
                        .SetParameter("endRecordIndex", recordId)
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

        /// <summary>
        /// 删除告警记录
        /// </summary>
        public static ARESULT RemoveAlarmInfo(
            List<Int64> alarmIdList)
        {
            if (alarmIdList == null || alarmIdList.Count <= 0)
                return ARESULT.E_FAIL;

            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return ARESULT.E_FAIL;

            try {
                connection.BeginTransaction();

                String sqlString = @"DELETE FROM alarm WHERE id IN (";
                foreach (Int64 alarmId in alarmIdList)
                    sqlString += alarmId.ToString() + ",";
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

        /// <summary>
        /// 根据时间获取告警记录
        /// </summary>
        public static List<Alarm> GetAlarmInfos(
            Int32 mode,
            Int32 type,
            Int32 reason,
            DateTime startTime,
            DateTime? endTime)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return null;

            try {
                String sqlString = @"SELECT a.id alarmId, * FROM alarm a INNER JOIN userroutineconfig b ON a.routineId=b.id 
                                    WHERE (startTime>=@startTime)";
                List<IDataParameter> parameters = new List<IDataParameter>();

                parameters.Add(connection.CreateParameter("startTime", startTime));

                if (endTime != null) {
                    sqlString += " AND (endTime<=@endTime)";
                    parameters.Add(connection.CreateParameter("endTime", endTime.Value));
                }

                if (mode != -1) {
                    sqlString += " AND (mode=@mode)";
                    parameters.Add(connection.CreateParameter("mode", mode));
                }

                if (type != -1) {
                    sqlString += " AND (type=@type)";
                    parameters.Add(connection.CreateParameter("type", type));
                }

                if (reason != -1) {
                    sqlString += " AND (reason=@reason)";
                    parameters.Add(connection.CreateParameter("reason", reason));
                }

                sqlString += " ORDER BY a.id DESC";

                DataTable dt = DbFactory.Instance.CreateSqlHelper(connection, sqlString)
                    .SetParameter(parameters)
                    .ToDataTable();

                if ((dt == null) || (dt.Rows.Count <= 0))
                    return null;

                List<Alarm> alarmList = new List<Alarm>();
                foreach (DataRow dr in dt.Rows) {
                    Alarm alarm = new Alarm();
                    alarm.mAlarmId = Int64.Parse(dr["alarmId"].ToString());
                    alarm.mDeviceName = dr["deviceposition"].ToString();
                    alarm.mStartTime = dr["starttime"].ToString();
                    if (dr["endtime"] != DBNull.Value)
                        alarm.mEndTime = dr["endtime"].ToString();

                    alarm.mReason = dr["detail"].ToString();
                    alarm.mImagePath = dr["image"].ToString();
                    alarmList.Add(alarm);
                }
                return alarmList;
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
        /// 更新告警录像状态
        /// </summary>
        public static ARESULT UpdateAlarmStatus(
            List<Int64> alarmIdLis)
        {
            if((alarmIdLis== null) || (alarmIdLis.Count <= 0))
                return ARESULT.E_FAIL;

            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return ARESULT.E_FAIL;

            try {
                connection.BeginTransaction();

                String sql = @"UPDATE alarm SET status=@status, startRecordIndex=@startRecordIndex,
                            endRecordIndex=@endRecordIndex WHERE id IN (";
                foreach (Int64 alarmId in alarmIdLis)
                    sql += alarmId.ToString() + ",";
                sql = sql.Substring(0, sql.Length - 1);
                sql += ")";

                DbFactory.Instance.CreateSqlHelper(connection, sql)
                    .SetParameter("status", 0)
                    .SetParameter("startRecordIndex", -1)
                    .SetParameter("endRecordIndex", -1)
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
        /// 获取录像信息
        /// </summary>
        public static List<Record> GetAlarmRecordList(
            DateTime startTime,
            DateTime endTime,
            String name,
            Int32 offset,
            Int32 count)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return null;

            try {
                String sqlString = @"SELECT id, name, startTime, startRecordIndex, endRecordIndex FROM alarm
                                    WHERE startTime>=@startTime AND endTime<=@endTime
                                    AND startRecordIndex<>-1 AND endRecordIndex<>-1
                                    AND (endTime IS NOT NULL) AND status=@status AND name LIKE @name
                                    ORDER BY id ASC LIMIT @count OFFSET @offset";

                // 模糊查询条件
                String fuzzy = null;
                if (String.IsNullOrEmpty(name))
                    fuzzy = "%%";
                else
                    fuzzy = String.Format("%{0}%", name);

                DataTable dt = DbFactory.Instance.CreateSqlHelper(connection, sqlString)
                    .SetParameter("startTime", startTime)
                    .SetParameter("endTime", endTime)
                    .SetParameter("status", 1)
                    .SetParameter("name", fuzzy)
                    .SetParameter("count", count)
                    .SetParameter("offset", offset)
                    .ToDataTable();

                if ((dt == null) || (dt.Rows.Count <= 0))
                    return null;

                List<Record> recordList = new List<Record>();

                // 拼装SQL,一次性查询, 获取所有的record数据
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

                // 放入字典,便于Id查询
                Dictionary<Int64, String> dict = new Dictionary<Int64, String>();
                foreach (DataRow dr in dt.Rows) {
                    Int64 id = Int64.Parse(dr["id"].ToString());
                    String fileName = dr["infoFileName"].ToString();
                    dict.Add(id, fileName);
                }

                for (Int32 i = 0; i < recordList.Count; i++) {
                    Record item = recordList[i];
                    List<String> files = new List<String>();
                    // 查找所需的文件列表
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
        /// 获取普通图片
        /// </summary>
        public static List<AlarmImage> GetAlarmCommonImage(List<Int64> ids)
        {
            if ((ids == null) || (ids.Count <= 0))
                return null;

            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return null;

            String sqlString = @"SELECT id as alarmId, image as imagePath FROM alarm WHERE ";
            foreach (Int64 id in ids) {
                sqlString = String.Format("{0} id={1} OR", sqlString, id);
            }

            sqlString = sqlString.Substring(0, sqlString.Length - 2);

            try {
                DataTable dt = DbFactory.Instance.CreateSqlHelper(connection, sqlString)
                    .ToDataTable();

                if ((dt == null) || (dt.Rows.Count <= 0))
                    return null;

                List<AlarmImage> imageList = new List<AlarmImage>();
                foreach (DataRow dr in dt.Rows) {
                    Int64 alarmId = Int64.Parse(dr["alarmId"].ToString());
                    String path = dr["imagePath"].ToString();

                    // 取出图片所有数据
                    Byte[] bytes = Utils.GetBytesByImagePath(path);
                    if (bytes == null)
                        continue;

                    // 截取图片数据,放弃后面的二次解析数据
                    bytes = Utils.CutImageBytes(bytes);
                    if (bytes == null)
                        continue;

                    AlarmImage image = new AlarmImage();
                    image.mImage = Convert.ToBase64String(bytes);
                    image.mAlarmId = alarmId;
                    imageList.Add(image);
                }

                return imageList;
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
        /// 获取二次解析图片
        /// </summary>
        public static List<AlarmImage> GetSecondAnalysisImage(List<Int64> ids)
        {
            if ((ids == null) && (ids.Count <= 0))
                return null;

            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return null;

            String sqlString = @"SELECT id as alarmId, image as imagePath FROM alarm WHERE ";
            foreach (Int64 id in ids) {
                sqlString = String.Format("{0} id={1} OR", sqlString, id);
            }

            sqlString = sqlString.Substring(0, sqlString.Length - 2);

            try {
                DataTable dt = DbFactory.Instance.CreateSqlHelper(connection, sqlString)
                    .ToDataTable();

                if ((dt == null) || (dt.Rows.Count <= 0))
                    return null;

                List<AlarmImage> imageList = new List<AlarmImage>();
                foreach (DataRow dr in dt.Rows) {
                    Int64 alarmId = Int64.Parse(dr["alarmId"].ToString());
                    String path = dr["imagePath"].ToString();

                    // 取出图片所有数据
                    Byte[] bytes = Utils.GetBytesByImagePath(path);
                    if (bytes == null)
                        continue;

                    AlarmImage image = new AlarmImage();
                    image.mImage = Convert.ToBase64String(bytes);
                    image.mAlarmId = alarmId;
                    imageList.Add(image);
                }

                return imageList;
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
        /// 获取图片路径
        /// </summary>
        public static List<String> GetAlarmImagePathList(List<Int64> alarmIdList)
        {
            if ((alarmIdList == null) && (alarmIdList.Count <= 0))
                return null;

            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return null;

            String sqlString = @"SELECT * FROM alarm WHERE id IN (";
            foreach (Int64 id in alarmIdList)
                sqlString += id.ToString() + ",";

            sqlString = sqlString.Substring(0, sqlString.Length - 1);
            sqlString += ")";

            List<String> strList = new List<String>();
            try {
                DataTable dt = DbFactory.Instance.CreateSqlHelper(connection, sqlString)
                    .ToDataTable();

                if ((dt == null) || (dt.Rows.Count <= 0))
                    return null;

                foreach (DataRow dr in dt.Rows) {
                    if (dr["image"] != DBNull.Value)
                        strList.Add(dr["image"].ToString());
                }

                return strList;
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
