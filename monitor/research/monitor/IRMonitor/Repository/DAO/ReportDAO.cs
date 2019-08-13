using Common;
using DBHelper;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Repository.DAO
{
    public class ReportDAO
    {
        /// <summary>
        /// 获取选区报表信息
        /// </summary>
        public static ReportInfo GetReportInfo(Int32 alarmId)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return null;

            try {
                String sqlString = @"Select a.startTime, a.selectionId, a.image, b.*, c.*,d.* from alarm a, 
                                userroutineconfig b,IrParam c, deviceinfo d where a.routineId = b.id and a.irParamId = c.id
                                and a.id = @alarmid";
                DataTable dt = DbFactory.Instance.CreateSqlHelper(connection, sqlString)
                    .SetParameter("alarmid", alarmId)
                    .ToDataTable();

                if ((dt == null) || (dt.Rows.Count <= 0))
                    return null;

                DataRow dr = dt.Rows[dt.Rows.Count - 1];
                ReportInfo info = new ReportInfo();
                info.StartTime = DateTime.Parse(dr["startTime"].ToString());
                info.DevicePosition = dr["devicePosition"].ToString();
                info.SubStation = dr["substation"].ToString();
                info.StandardTemp = 25.0;
                info.Emissivity = Convert.ToDouble(dr["emissivity"]);
                info.Distance = Convert.ToDouble(dr["distance"]);
                info.AmbientTemp = Convert.ToDouble(dr["AtmosphericTemperature"]);
                info.DeviceName = dr["equipmentmodel"].ToString();
                info.DeviceNum = dr["serialnumber"].ToString();
                info.Provincial = dr["province"].ToString();
                info.Projectleader = dr["projectleader"].ToString();
                info.City = dr["city"].ToString();
                info.Company = dr["company"].ToString();
                info.Testpersonnel = dr["testpersonnel"].ToString();

                String image = dr["image"].ToString();
                try {
                    FileStream fs = new FileStream(image, FileMode.Open, FileAccess.Read);
                    Byte[] byteData = new byte[fs.Length];
                    fs.Read(byteData, 0, byteData.Length);
                    fs.Close();
                    info.ViewImage = byteData;
                }
                catch {
                }

                Int32 selectionId = Convert.ToInt32(dr["selectionId"]);
                List<SelectionTempCurve> tempCurves = SelectionTemperatureDAO.GetSelectionTemperature(
                    selectionId, DateTime.Now.AddDays(-3), DateTime.Now);

                if (tempCurves != null) {
                    foreach (SelectionTempCurve item in tempCurves) {
                        ReportTemperature temp = new ReportTemperature();
                        temp.MaxTemperature = item.mMaxTemp;
                        temp.MinTemperature = item.mMinTemp;
                        temp.AvgTemperature = item.mAvgTemp;
                        temp.HistoryTime = item.mDateTime;
                        info.ReportTemperatures.Add(temp);
                    }
                }
                return info;
            }
            catch (Exception e){
                Tracker.LogE(e);
                return null;
            }
            finally {
                DBConnection.Instance.ReturnDBConnection(connection);
            }
        }

        /// <summary>
        /// 获取选区组报表信息
        /// </summary>
        public static ReportInfo GetReportGroupInfo(Int32 alarmId)
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return null;

            ReportInfo info = null;
            try {
                String sqlString = @"Select a.startTime, a.selectionId, a.image, b.*, c.*,d.* from alarm a,
                    userroutineconfig b, IrParam c, deviceinfo d where a.routineId = b.id and a.irParamId = c.id and
                    a.id = @alarmid";
                DataTable dt = DbFactory.Instance.CreateSqlHelper(connection, sqlString)
                    .SetParameter("alarmid", alarmId)
                    .ToDataTable();

                if ((dt == null) || (dt.Rows.Count <= 0))
                    return null;

                DataRow dr = dt.Rows[dt.Rows.Count - 1];
                info = new ReportInfo();
                info.StartTime = DateTime.Parse(dr["startTime"].ToString());
                info.DevicePosition = dr["devicePosition"].ToString();
                info.SubStation = dr["substation"].ToString();
                info.StandardTemp = 25.0;
                info.Emissivity = Convert.ToDouble(dr["emissivity"]);
                info.Distance = Convert.ToDouble(dr["distance"]);
                info.AmbientTemp = Convert.ToDouble(dr["AtmosphericTemperature"]);
                info.DeviceName = dr["equipmentmodel"].ToString();
                info.DeviceNum = dr["serialnumber"].ToString();
                info.Provincial = dr["province"].ToString();
                info.Projectleader = dr["projectleader"].ToString();
                info.City = dr["city"].ToString();
                info.Company = dr["company"].ToString();
                info.Testpersonnel = dr["testpersonnel"].ToString();

                String image = dr["image"].ToString();
                try {
                    FileStream fs = new FileStream(image, FileMode.Open, FileAccess.Read);
                    Byte[] byteData = new Byte[fs.Length];
                    fs.Read(byteData, 0, byteData.Length);
                    fs.Close();
                    info.ViewImage = byteData;
                }
                catch {
                }

                Int32 selectionId = Convert.ToInt32(dr["selectionId"]);
                List<GroupSelectionTempCurve> tempCurves = GroupSelectionTemperatureDAO.GetGroupSelectionTemperature(
                    selectionId, DateTime.Now.AddDays(-3), DateTime.Now);

                if (tempCurves != null) {
                    foreach (GroupSelectionTempCurve item in tempCurves) {
                        ReportGroupTemperature temp = new ReportGroupTemperature();
                        temp.MaxTemperature = item.mMaxTemp;
                        temp.TemperatureDifference = item.mTempDif;
                        temp.TemperatureRise = item.mTempRise;
                        temp.HistoryTime = item.mDateTime;
                        info.ReportGruopTemperatures.Add(temp);
                    }
                }

                return info;
            }
            catch (Exception e){
                Tracker.LogE(e);
                return null;
            }
            finally {
                DBConnection.Instance.ReturnDBConnection(connection);
            }
        }
    }
}
