using Common;
using DBHelper;
using Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace Repository.DAO
{
    public class CurveSampleDAO
    {
        public static List<TempCurveSample> GetCurveSample()
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return null;

            try {
                String sql = "SELECT * FROM TempCurveSample";
                DataTable dt = DbFactory.Instance.CreateSqlHelper(connection, sql)
                    .ToDataTable();

                if ((dt == null) || (dt.Rows.Count <= 0))
                    return null;

                List<TempCurveSample> sampleList = new List<TempCurveSample>();
                foreach (DataRow dr in dt.Rows) {
                    TempCurveSample sample = new TempCurveSample();
                    sample.mSelectionSample = Int32.Parse(dr["SelectionSample"].ToString());
                    sample.mGroupSelectionSample = Int32.Parse(dr["GroupSelectionSample"].ToString());
                    sampleList.Add(sample);
                }

                return sampleList;
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return null;
            }
            finally {
                DBConnection.Instance.ReturnDBConnection(connection);
            }
        }

        public static ARESULT UpateCurveSample(
            Int32 selectionSample,
            Int32 groupSelectionSample)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return ARESULT.E_FAIL;

            try {
                connection.BeginTransaction();

                String sql = @"UPDATE TempCurveSample SET SelectionSample=@SelectionSample,
                            GroupSelectionSample=@GroupSelectionSample";
                Int32 ret = DbFactory.Instance.CreateSqlHelper(connection, sql)
                    .SetParameter("SelectionSample", selectionSample)
                    .SetParameter("GroupSelectionSample", groupSelectionSample)
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
    }
}
