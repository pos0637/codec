using Common;
using DBHelper;
using Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace Repository.DAO
{
    public class CellDAO
    {
        /// <summary>
        /// 获取所有的监控单元
        /// </summary>
        public static List<Cell> GetAllCells()
        {
            IDbHelper connection = DBConnection.Instance.GetConnection();
            if (connection == null)
                return null;

            try {
                string queryString = "SELECT * FROM cell";
                DataTable dt = DbFactory.Instance.CreateSqlHelper(connection, queryString).ToDataTable();
                if ((dt == null) || (dt.Rows.Count <= 0)) {
                    return null;
                }

                List<Cell> cellList = new List<Cell>();
                foreach (DataRow dr in dt.Rows) {
                    Cell cell = new Cell();
                    cell.mCellId = Int64.Parse(dr["id"].ToString());
                    cell.mCellName = dr["name"].ToString();
                    cell.mIRCameraIp = dr["ircamera_ip"].ToString();
                    cell.mIRCameraWidth = Int32.Parse(dr["ircamera_width"].ToString());
                    cell.mIRCameraStride = Int32.Parse(dr["ircamera_stride"].ToString());
                    cell.mIRCameraHeight = Int32.Parse(dr["ircamera_height"].ToString());
                    cell.mIRCameraBitsPerPixel = Int32.Parse(dr["ircamera_bitsPerPixel"].ToString());
                    cell.mIRCameraVideoFrameRate = Int32.Parse(dr["ircamera_videoFrameRate"].ToString());
                    cell.mIRCameraTemperatureFrameRate = Int32.Parse(dr["ircamera_tempframeRate"].ToString());
                    cell.mIRCameraVideoDuration = Int32.Parse(dr["ircamera_duration"].ToString());
                    cell.mIRCameraVideoFolder = dr["ircamera_videoFolder"].ToString();
                    cell.mIRCameraImageFolder = dr["ircamera_imageFolder"].ToString();
                    cell.mIRCameraType = dr["ircamera_type"].ToString();
                    cellList.Add(cell);
                }
                return cellList;
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
