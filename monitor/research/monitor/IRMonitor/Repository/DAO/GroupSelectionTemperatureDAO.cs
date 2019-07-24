using Common;
using DBHelper;
using Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace Repository.DAO
{
    public class GroupSelectionTemperatureDAO
    {
        /// <summary>
        /// 添加选区组历史温度
        /// </summary>
        public static ARESULT AddGroupSelectionTemperature(
            Int64 cellid,
            Int64 groupSelectionId,
            DateTime recordTime,
            Single maxTemperature,
            Single temperaturedifference,
            Single temperaturerise)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return ARESULT.E_FAIL;

            try {
                connection.BeginTransaction();

                Int32 ret = DbFactory.Instance.CreateInsertHelper(connection, "groupselectiontemperature")
                    .SetParameter("cellid", cellid)
                    .SetParameter("groupid", groupSelectionId)
                    .SetParameter("recordTime", recordTime.ToString("yyyy-MM-dd HH:mm:ss"))
                    .SetParameter("maxtemperature", maxTemperature)
                    .SetParameter("temperaturedifference", temperaturedifference)
                    .SetParameter("temperaturerise", temperaturerise)
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
        /// 获取选区组历史温度
        /// </summary>
        public static List<GroupSelectionTempCurve> GetGroupSelectionTemperature(
            Int64 groupSelectionId,
            DateTime startTime,
            DateTime endTime)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return null;

            try {
                String queryString = @"SELECT * FROM groupselectiontemperature WHERE groupId=@groupId
                                      AND recordTime>=@startTime AND recordTime<@endTime ORDER BY id ASC";

                DataTable dt = DbFactory.Instance.CreateSqlHelper(connection, queryString)
                    .SetParameter("groupId", groupSelectionId)
                    .SetParameter("startTime", startTime)
                    .SetParameter("endTime", endTime)
                    .ToDataTable();

                if ((dt == null) || (dt.Rows.Count <= 0))
                    return null;

                List<GroupSelectionTempCurve> curveList = new List<GroupSelectionTempCurve>();
                foreach (DataRow dr in dt.Rows) {
                    GroupSelectionTempCurve curve = new GroupSelectionTempCurve();
                    curve.mGroupSelectionId = Int64.Parse(dr["id"].ToString());
                    curve.mDateTime = DateTime.Parse(dr["recordTime"].ToString());
                    curve.mMaxTemp = Single.Parse(dr["maxtemperature"].ToString());
                    curve.mTempDif = Single.Parse(dr["temperaturedifference"].ToString());
                    curve.mTempRise = Single.Parse(dr["temperaturerise"].ToString());

                    curveList.Add(curve);
                }

                return curveList;
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
