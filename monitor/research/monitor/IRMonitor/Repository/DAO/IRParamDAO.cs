using Common;
using DBHelper;
using Models;
using System;
using System.Data;

namespace Repository.DAO
{
    public class IRParamDAO
    {
        /// <summary>
        /// 获取红外预设参数
        /// </summary>
        /// <returns></returns>
        public static IRParam GetIRParam()
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return null;

            try {
                DataTable dt = DbFactory.Instance.CreateQueryHelper(connection, "IrParam")
                    .OrderBy("id")
                    .ToDataTable();

                if ((dt == null) || (dt.Rows.Count <= 0))
                    return null;

                DataRow dr = dt.Rows[dt.Rows.Count - 1];
                IRParam param = new IRParam();
                param.mId = Int64.Parse(dr["id"].ToString());
                param.mEmissivity = Single.Parse(dr["emissivity"].ToString());
                param.mReflectedTemperature = Single.Parse(dr["reflectedTemperature"].ToString());
                param.mRelativeHumidity = Single.Parse(dr["relativeHumidity"].ToString());
                param.mTransmission = Single.Parse(dr["transmission"].ToString());
                param.mAtmosphericTemperature = Single.Parse(dr["atmosphericTemperature"].ToString());
                param.mDistance = Single.Parse(dr["distance"].ToString());
                return param;
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
        /// 更新红外参数
        /// </summary>
        public static ARESULT UpateIRParam(
            Single emissivity,
            Single reflectedTemperature,
            Single transmission,
            Single atmosphericTemperature,
            Single relativeHumidity,
            Single distance,
            ref Int32 id)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return ARESULT.E_FAIL;

            ARESULT result = ARESULT.E_FAIL;
            try {
                connection.BeginTransaction();

                Int32 ret = DbFactory.Instance.CreateInsertHelper(connection, "IrParam")
                    .SetParameter("Emissivity", emissivity)
                    .SetParameter("ReflectedTemperature", reflectedTemperature)
                    .SetParameter("Transmission", transmission)
                    .SetParameter("AtmosphericTemperature", atmosphericTemperature)
                    .SetParameter("RelativeHumidity", relativeHumidity)
                    .SetParameter("Distance", distance)
                    .ExecuteNonQuery();
                if (ret != 1)
                    return ARESULT.E_FAIL;

                DataTable dt = DbFactory.Instance.CreateQueryHelper(connection, "IrParam")
                    .Query("id")
                    .OrderBy("id")
                    .ToDataTable();
                if ((dt == null) || (dt.Rows.Count <= 0))
                    return ARESULT.E_FAIL;

                DataRow row = dt.Rows[dt.Rows.Count - 1];
                id = Convert.ToInt32(row["id"]);
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
    }
}
