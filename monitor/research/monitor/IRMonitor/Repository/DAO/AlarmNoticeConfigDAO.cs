using Common;
using DBHelper;
using Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace Repository.DAO
{
    public class AlarmNoticeConfigDAO
    {
        /// <summary>
        /// 获取告警通知设置
        /// </summary>
        public static AlarmNoticeConfig GetAlarmNoticeConfigInfo()
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return null;

            try {
                String sql = "SELECT * FROM AlarmNoticeConfig";
                DataTable dt = DbFactory.Instance.CreateSqlHelper(connection, sql)
                    .ToDataTable();

                if ((dt == null) || (dt.Rows.Count <= 0))
                    return null;

                List<AlarmNoticeConfig> alarmNoticeConfigList = new List<AlarmNoticeConfig>();
                foreach (DataRow dr in dt.Rows) {
                    AlarmNoticeConfig alarmConfig = new AlarmNoticeConfig();

                    if (dr["senduser"] != DBNull.Value) {
                        String temp = dr["senduser"].ToString();
                        String[] var = temp.Split(new char[] { ',' },
                            StringSplitOptions.RemoveEmptyEntries);
                        alarmConfig.mSendUser = new List<String>(var);
                    }

                    alarmConfig.mIsAlarmSend = (Boolean)dr["isalarmsend"];
                    alarmConfig.mIsHourSend = (Boolean)dr["ishoursend"];
                    alarmConfig.mIsRegularTimeSend = (Boolean)dr["isregulartimesend"];

                    if (dr["regulartime"] != DBNull.Value) {
                        String temp = dr["regulartime"].ToString();
                        String[] var = temp.Split(new char[] { ',' },
                            StringSplitOptions.RemoveEmptyEntries);

                        foreach (String item in var) {
                            TimeSpan span;
                            if (TimeSpan.TryParse(item, out span))
                                alarmConfig.mRegularTime.Add(span);
                        }
                    }

                    alarmConfig.mIsAutoReply = (Boolean)dr["isautoreply"];
                    alarmConfig.mIsSelectionRecord = (Boolean)dr["IsSelectionRecord"];
                    alarmConfig.mIsGroupSelectionRecord = (Boolean)dr["IsGroupSelectionRecord"];
                    alarmNoticeConfigList.Add(alarmConfig);
                }

                return alarmNoticeConfigList[0];
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
        /// 更新告警设置配置
        /// </summary>
        /// <param name="sendUser">通讯录</param>
        /// <param name="isAlarmSend">告警短信</param>
        /// <param name="isHourSend">整点通知</param>
        /// <param name="isRegularTimeSend">定时通知</param>
        /// <param name="regularTime">定时通知时间集</param>
        /// <param name="isAutoReply">自动回复</param>
        /// <param name="isSelectionRecord">普通选区录像</param>
        /// <param name="isGroupSelectionRecord">组选区录像</param>
        /// <returns></returns>
        public static ARESULT UpdateAlarmNoticeConfigInfo(
            List<String> sendUser,
            Boolean isAlarmSend,
            Boolean isHourSend,
            Boolean isRegularTimeSend,
            List<TimeSpan> regularTime,
            Boolean isAutoReply,
            Boolean isSelectionRecord,
            Boolean isGroupSelectionRecord)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return ARESULT.E_FAIL;

            try {
                connection.BeginTransaction();

                String sendUserStr = "";
                sendUser.ForEach(i => sendUserStr = sendUserStr + i + ",");

                String regularTimeStr = "";
                regularTime.ForEach(i => regularTimeStr = regularTimeStr + i.ToString() + ",");

                String sqlStr = @"UPDATE AlarmNoticeConfig SET senduser=@senduser,
                                  isalarmsend=@isalarmsend, ishoursend=@ishoursend, isregulartimesend=@isregulartimesend,
                                  regulartime=@regulartime, isautoreply=@isautoreply, IsSelectionRecord=@IsSelectionRecord,
                                  IsGroupSelectionRecord=@IsGroupSelectionRecord";

                Int32 ret = DbFactory.Instance.CreateSqlHelper(connection, sqlStr)
                    .SetParameter("senduser", sendUserStr)
                    .SetParameter("isalarmsend", (isAlarmSend ? 1 : 0))
                    .SetParameter("ishoursend", (isHourSend ? 1 : 0))
                    .SetParameter("isregulartimesend", (isRegularTimeSend ? 1 : 0))
                    .SetParameter("regulartime", regularTimeStr)
                    .SetParameter("isautoreply", (isAutoReply ? 1 : 0))
                    .SetParameter("IsSelectionRecord", (isSelectionRecord ? 1 : 0))
                    .SetParameter("IsGroupSelectionRecord", (isGroupSelectionRecord ? 1 : 0))
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
    }
}
