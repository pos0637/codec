using Common;
using DBHelper;
using Models;
using System;
using System.Data;

namespace Repository.DAO
{
    public class DeviceDAO
    {
        public static DeviceInfo GetDeviceInfo()
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return null;

            try {
                String sql = "SELECT * FROM deviceinfo";
                DataTable dt = DbFactory.Instance.CreateSqlHelper(connection, sql)
                    .ToDataTable();

                if ((dt == null) || (dt.Rows.Count <= 0))
                    return null;

                DataRow dr = dt.Rows[0];
                DeviceInfo device = new DeviceInfo();
                device.mEquipmentModel = dr["equipmentmodel"].ToString();
                device.mSerialNumber = dr["serialnumber"].ToString();
                device.mVersion = dr["version"].ToString();
                device.mResolution = dr["resolution"].ToString();
                device.mDetectorType = dr["detectortype"].ToString();

                return device;
            }
            catch (Exception ex){
                Tracker.LogE(ex);
                return null;
            }
            finally {
                DBConnection.Instance.ReturnDBConnection(connection);
            }
        }
    }
}
