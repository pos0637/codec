using Common;
using DBHelper;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Repository.DAO
{
    public class RecordDAO
    {
        /// <summary>
        /// 添加录像
        /// </summary>
        public static ARESULT AddRecord(
            Int64 cellId,
            String videoFileName,
            String selectionFileName,
            ref Int64 id)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return ARESULT.E_FAIL;

            try {
                String queryString = @"INSERT INTO record (cellId, date, videoFileName, infoFileName)
                                       VALUES (@cellId, @date, @videoFileName, @irFileName);
                                       SELECT last_insert_rowid();";
                connection.BeginTransaction();

                Object obj = DbFactory.Instance.CreateSqlHelper(connection, queryString)
                    .SetParameter("cellId", cellId)
                    .SetParameter("date", DateTime.Now)
                    .SetParameter("videoFileName", videoFileName)
                    .SetParameter("irFileName", selectionFileName)
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
                connection.RollbackTransaction();
                Tracker.LogE(e);
                return ARESULT.E_FAIL;
            }
            finally {
                DBConnection.Instance.ReturnDBConnection(connection);
            }
        }

        /// <summary>
        /// 删除录像
        /// eg: 调用此函数时, 应该保证录像文件没有其他记录引用
        /// </summary>
        public static ARESULT RemovetRecord(
            List<Int64> recordIdList)
        {
            if ((recordIdList == null) || (recordIdList.Count <= 0))
                return ARESULT.E_FAIL;

            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return ARESULT.E_FAIL;

            try {
                connection.BeginTransaction();

                String sqlString = @"DELETE FROM record WHERE id IN (";
                foreach (Int64 recordId in recordIdList)
                    sqlString += recordId.ToString() + ",";
                sqlString = sqlString.Substring(0, sqlString.Length - 1);
                sqlString += ")";

                Int32 ret =  DbFactory.Instance.CreateSqlHelper(connection, sqlString)
                    .ExecuteNonQuery();

                connection.CommitTransaction();
                return (ret > 0 ? ARESULT.S_OK : ARESULT.E_FAIL);
            }
            catch (Exception e) {
                connection.RollbackTransaction();
                Tracker.LogE(e);
                return ARESULT.E_FAIL;
            }
            finally {
                DBConnection.Instance.ReturnDBConnection(connection);
            }
        }

        /// <summary>
        /// 获取告警文件名称
        /// </summary>
        public static List<String> GetRecordFileListByAlarmId(
            Int64 alarmId)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return null;

            try {
                String sqlString = @"SELECT b.* FROM alarm a, record b WHERE a.id= @id and a.startRecordIndex <> -1
                                    and a.endRecordIndex <> -1 and a.startRecordIndex IS NOT NULL and a.endRecordIndex IS NOT NULL
                                    and b.id>=a.startRecordIndex AND b.id<=a.endRecordIndex";

                DataTable dt = DbFactory.Instance.CreateSqlHelper(connection, sqlString)
                    .SetParameter("id", alarmId)
                    .ToDataTable();
                if ((dt == null) || (dt.Rows.Count <= 0))
                    return null;

                List<String> recordList = new List<String>();
                foreach (DataRow item in dt.Rows) {
                    if (item["infoFileName"] != DBNull.Value)
                        recordList.Add(item["infoFileName"].ToString());
                }
                return recordList;
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
        /// 获取手动录像文件名称
        /// </summary>
        public static List<String> GetRecordFileListByManualRecordId(
            Int64 manualRecordId)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return null;

            try {
                String sqlString = @"SELECT b.* FROM manualrecord a, record b WHERE a.id= @id and a.startRecordIndex <> -1
                                    and a.endRecordIndex <> -1 and a.startRecordIndex IS NOT NULL and a.endRecordIndex IS NOT NULL
                                    and b.id>=a.startRecordIndex AND b.id<=a.endRecordIndex";

                DataTable dt = DbFactory.Instance.CreateSqlHelper(connection, sqlString)
                    .SetParameter("id", manualRecordId)
                    .ToDataTable();
                if ((dt == null) || (dt.Rows.Count <= 0))
                    return null;

                List<String> recordList = new List<String>();
                foreach (DataRow item in dt.Rows) {
                    if (item["infoFileName"] != DBNull.Value)
                        recordList.Add(item["infoFileName"].ToString());
                }
                return recordList;
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
        /// 获取录像信息(录像时间, 缩略图)
        /// </summary>
        public static ARESULT GetRecordMessage(List<String> infos,
            ref Double recordTime, ref String thumbnail)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return ARESULT.E_FAIL;

            try {
                Int32 frameNum = 0;
                Int32 frameRate = 1;
                Byte[] image = null;
                String filename = null;

                foreach (String item in infos) {
                    if (!File.Exists(item))
                        continue;

                    FileStream fs = null;
                    BinaryReader reader = null;
                    try {
                        fs = new FileStream(item, FileMode.Open, FileAccess.Read);
                        reader = new BinaryReader(fs);

                        // 将文件指针设置到文件开始
                        reader.BaseStream.Seek(8, SeekOrigin.Begin);
                        frameNum += reader.ReadInt32();
                        frameRate = reader.ReadInt32();
                        Int32 imageLen = reader.ReadInt32();
                        if (image == null)
                            image = reader.ReadBytes(imageLen);

                        if (filename == null)
                            filename = Path.GetFileNameWithoutExtension(item);
                    }
                    catch (Exception e) {
                        Tracker.LogE(e);
                    }
                    finally {
                        if (reader != null)
                            reader.Close();

                        if (fs != null)
                             fs.Close();
                    }
                }

                if ((image == null) || (filename == null))
                    return ARESULT.E_FAIL;

                if (frameNum == 0) {
                    foreach (String item in infos) {
                        if (!File.Exists(item))
                            continue;

                        FileStream fs = null;
                        BinaryReader reader = null;
                        try {
                            fs = new FileStream(item, FileMode.Open, FileAccess.Read);
                            reader = new BinaryReader(fs);

                            // 将文件指针设置到文件开始
                            reader.BaseStream.Seek(16, SeekOrigin.Begin);
                            Int32 imageLen = reader.ReadInt32();
                            reader.BaseStream.Seek(imageLen, SeekOrigin.Current);
                            while (reader.BaseStream.Position < reader.BaseStream.Length) {
                                Int32 len = reader.ReadInt32();
                                Byte[] data = reader.ReadBytes(len);
                                Int64 selectionDatalength = BitConverter.ToInt64(data, 60);
                                if (selectionDatalength != 0)
                                    reader.BaseStream.Seek(selectionDatalength, SeekOrigin.Current);

                                frameNum++;
                            }
                        }
                        catch {
                        }
                        finally {
                            if (reader != null)
                                reader.Close();

                            if (fs != null)
                                fs.Close();
                        }
                    }
                }

                recordTime = frameNum * 1.0 / frameRate;
                thumbnail = Convert.ToBase64String(image);
                return ARESULT.S_OK;
            }
            catch (Exception e){
                Tracker.LogE(e);
                return ARESULT.E_FAIL;
            }
            finally {
                DBConnection.Instance.ReturnDBConnection(connection);
            }
        }

        /// <summary>
        /// 获取闲置的录像文件
        /// </summary>
        public static List<RecoreInfo> GetIdleRecord()
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return null;

            try {
                String sqlString = @"SELECT * FROM record d WHERE d.id NOT IN (
                                    SELECT b.id as id FROM record b,
                                    (SELECT a.startRecordIndex, a.endRecordIndex FROM alarm a WHERE a.status = 1
                                        UNION
                                     SELECT b.startRecordIndex, b.endRecordIndex FROM manualrecord b
                                    ) c WHERE (c.startRecordIndex <> -1 AND c.startRecordIndex <> -1) AND
                                    ((b.id BETWEEN c.startRecordIndex AND c.endRecordIndex) OR (b.id >= c.startRecordIndex
                                    AND c.endRecordIndex IS NULL)) GROUP BY b.id)";

                DataTable dt = DbFactory.Instance.CreateSqlHelper(connection, sqlString)
                    .ToDataTable();
                if (dt == null)
                    return null;

                List<RecoreInfo> recordList = new List<RecoreInfo>();
                foreach (DataRow item in dt.Rows) {
                    RecoreInfo info = new RecoreInfo();
                    info.mId = Convert.ToInt32(item["id"]);
                    info.mInfoFileName = item["videoFileName"].ToString();
                    info.mVideoFileName = item["infoFileName"].ToString();
                    recordList.Add(info);
                }
                return recordList;
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return null;
            }
            finally {
                DBConnection.Instance.ReturnDBConnection(connection);
            }
        }
    }
}
