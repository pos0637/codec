using Common;
using IRMonitor.Common;
using Repository.DAO;
using System;
using System.Collections.Generic;
using System.IO;

namespace IRMonitor.Services.Cell
{
    /// <summary>
    /// 设备单元服务管理器
    /// </summary>
    public class CellServiceManager : Singleton<CellServiceManager>
    {
        /// <summary>
        /// 底层服务链表
        /// </summary>
        public static List<CellService> gIRServiceList = new List<CellService>();

        private CellServiceManager() { }

        /// <summary>
        /// 读取所有Cell数据
        /// </summary>
        public ARESULT LoadConfig()
        {
            List<Models.Cell> cellList = CellDAO.GetAllCells();
            if ((cellList == null) || (cellList.Count <= 0)) {
                Tracker.LogI("LoadConfig FAILED");
                return ARESULT.E_FAIL;
            }

            // 保存到全局上
            Global.gCellList = cellList;

            if (ARESULT.AFAILED(CheckConfig())) {
                Tracker.LogI("CheckOutConfig FAILED");
                return ARESULT.E_FAIL;
            }

            Tracker.LogI(String.Format("LoadConfig SUCCEED, Cell Count Is ({0})", cellList.Count));

            foreach (Models.Cell item in cellList) {
                var service = new CellService();

                // 初始化设备单元服务
                if (ARESULT.AFAILED(service.Initialize(item))) {
                    Tracker.LogI(String.Format("IRService({0}) Initialize FAILED", item.mCellId));
                    return ARESULT.E_FAIL;
                }

                Tracker.LogI(String.Format("IRService({0}) Initialize SUCCEED", item.mCellId));

                // 开启设备单元服务
                if (ARESULT.AFAILED(service.Start())) {
                    Tracker.LogI(String.Format("IRService({0}) Start FAILED", item.mCellId));
                    return ARESULT.E_FAIL;
                }

                gIRServiceList.Add(service);
                Tracker.LogI(String.Format("IRService({0}) Start SUCCEED", item.mCellId));
            }

            return ARESULT.S_OK;
        }

        /// <summary>
        /// 检测配置是否正常
        /// </summary>
        private ARESULT CheckConfig()
        {
            foreach (var item in Global.gCellList) {
                if ((item.mIRCameraTemperatureFrameRate <= 0) || (item.mIRCameraTemperatureFrameRate >= 4))
                    return ARESULT.E_FAIL;

                if ((item.mIRCameraVideoFrameRate <= 0) || (item.mIRCameraVideoFrameRate >= 60))
                    return ARESULT.E_FAIL;

                try {
                    if (!Directory.Exists(item.mIRCameraImageFolder))
                        Directory.CreateDirectory(item.mIRCameraImageFolder);

                    if (!Directory.Exists(item.mIRCameraVideoFolder))
                        Directory.CreateDirectory(item.mIRCameraVideoFolder);
                }
                catch {
                    return ARESULT.E_FAIL;
                }
            }

            return ARESULT.S_OK;
        }
    }
}
