using Common;
using DBHelper;
using Models;
using System;
using System.Data;

namespace Repository.DAO
{
    public class UserRotineDAO
    {
        /// <summary>
        /// 获取常规设置
        /// </summary>
        public static UserRoutineConfig GetUserRotineInfo()
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return null;

            try {
                DataTable dt = DbFactory.Instance.CreateQueryHelper(connection, "userroutineconfig")
                    .OrderBy("id")
                    .ToDataTable();

                if ((dt == null) || (dt.Rows.Count <= 0))
                    return null;

                // 取最新的一条
                DataRow dr = dt.Rows[dt.Rows.Count - 1];
                UserRoutineConfig routine = new UserRoutineConfig();
                routine.mId = Convert.ToInt32(dr["id"]);
                routine.mProvince = dr["province"].ToString();
                routine.mCity = dr["city"].ToString();
                routine.mCompany = dr["company"].ToString();
                routine.mProjectLeader = dr["projectleader"].ToString();
                routine.mTestPersonnel = dr["testpersonnel"].ToString();
                routine.mSubstation = dr["substation"].ToString();
                routine.mDevicePosition = dr["deviceposition"].ToString();
                return routine;
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
        /// 更新常规设置
        /// </summary>
        public static ARESULT UpdateUserRoutineInfo(
            String province,
            String city,
            String company,
            String projectLeader,
            String testPersonnel,
            String substation,
            String deviceposition,
            ref Int32 id)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return ARESULT.E_FAIL;

            try {
                connection.BeginTransaction();

                String sql = @"INSERT INTO userroutineconfig(province, city, company, projectleader
                        ,testpersonnel, substation, deviceposition) VALUES(@province, @city, @company, @projectleader
                        ,@testpersonnel, @substation, @deviceposition); SELECT Max(id) from userroutineconfig";

                Object obj = DbFactory.Instance.CreateSqlHelper(connection, sql)
                    .SetParameter("province", province)
                    .SetParameter("city", city)
                    .SetParameter("company", company)
                    .SetParameter("projectleader", projectLeader)
                    .SetParameter("testpersonnel", testPersonnel)
                    .SetParameter("substation", substation)
                    .SetParameter("deviceposition", deviceposition)
                    .ToScalar<Object>();

                if (obj == null) {
                    connection.RollbackTransaction();
                    return ARESULT.E_FAIL;
                }
                else {
                    id = Convert.ToInt32(obj);
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
    }
}
