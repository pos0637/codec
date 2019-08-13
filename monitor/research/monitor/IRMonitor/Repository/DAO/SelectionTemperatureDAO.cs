using Common;
using DBHelper;
using Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace Repository.DAO
{
    public class SelectionTemperatureDAO
    {
        public static ARESULT AddSelectionTemperature(
            Int64 cellId,
            Int64 selectionId,
            DateTime recordTime,
            Single maxTemperature,
            Single minTemperature,
            Single aveTemperature)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return ARESULT.E_FAIL;

            try {
                connection.BeginTransaction();

                Int32 ret = DbFactory.Instance.CreateInsertHelper(connection, "selectiontemperature")
                    .SetParameter("cellid", cellId)
                    .SetParameter("selectionid", selectionId)
                    .SetParameter("maxtemperature", maxTemperature)
                    .SetParameter("mintemperature", minTemperature)
                    .SetParameter("avgtemperature", aveTemperature)
                    .SetParameter("recordTime", recordTime.ToString("yyyy-MM-dd HH:mm:ss"))
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

        public static List<SelectionTempCurve> GetSelectionTemperature(
            Int64 selectionId,
            DateTime startTime,
            DateTime endTime)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return null;

            try {
                String queryString = @"SELECT * FROM selectiontemperature WHERE selectionId=@selectionId
                                      AND recordTime>=@startTime AND recordTime<@endTime ORDER BY id ASC";
                DataTable dt = DbFactory.Instance.CreateSqlHelper(connection, queryString)
                    .SetParameter("selectionId", selectionId)
                    .SetParameter("startTime", startTime)
                    .SetParameter("endTime", endTime)
                    .ToDataTable();

                if ((dt == null) || (dt.Rows.Count <= 0))
                    return null;

                List<SelectionTempCurve> curveList = new List<SelectionTempCurve>();
                foreach (DataRow dr in dt.Rows) {
                    SelectionTempCurve curve = new SelectionTempCurve();
                    curve.mSelectionId = Int64.Parse(dr["id"].ToString());
                    curve.mDateTime = DateTime.Parse(dr["recordTime"].ToString());
                    curve.mMaxTemp = Single.Parse(dr["maxtemperature"].ToString());
                    curve.mMinTemp = Single.Parse(dr["mintemperature"].ToString());
                    curve.mAvgTemp = Single.Parse(dr["avgtemperature"].ToString());
                    curveList.Add(curve);
                }

                return curveList;
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
