using Common;
using Repository.Entities;
using System;
using System.Collections.Generic;
using System.IO;

namespace IRMonitor2.Services.Cell
{
    /// <summary>
    /// 设备单元服务管理器
    /// </summary>
    public class _CellServiceManager : ServiceManager
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public ARESULT Initialize()
        {
            // 读取所有设备单元信息
            Configuration configuration = Repository.Repository.LoadConfiguation();
            if (configuration == null) {
                Tracker.LogI("LoadConfiguration FAIL");
                return ARESULT.E_FAIL;
            }

            if (ARESULT.AFAILED(CheckConfiguration(configuration))) {
                Tracker.LogI("CheckConfiguration FAIL");
                return ARESULT.E_FAIL;
            }

            Tracker.LogI($"LoadConfiguration SUCCEED, Cell Count: {configuration.cells.Length}");

            // 创建服务
            foreach (var cell in configuration.cells) {
                var service = new CellService();

                // 初始化设备单元服务
                service.Initialize(new Dictionary<string, object>() { ["cell"] = cell });

                // 开启设备单元服务
                if (ARESULT.AFAILED(service.Start())) {
                    Tracker.LogI($"CellService: {cell.name} Start FAIL");
                    return ARESULT.E_FAIL;
                }

                AddService(cell.name, service);
                Tracker.LogI($"CellService: {cell.name} Start SUCCEED");
            }

            return ARESULT.S_OK;
        }

        /// <summary>
        /// 检测配置
        /// </summary>
        /// <param name="configuration">配置</param>
        /// <returns>是否成功</returns>
        private ARESULT CheckConfiguration(Configuration configuration)
        {
            try {
                if (!Directory.Exists(configuration.information.saveVideoPath)) {
                    Directory.CreateDirectory(configuration.information.saveVideoPath);
                }

                if (!Directory.Exists(configuration.information.saveImagePath)) {
                    Directory.CreateDirectory(configuration.information.saveImagePath);
                }
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return ARESULT.E_FAIL;
            }

            return ARESULT.S_OK;
        }
    }

    /// <summary>
    /// 设备单元服务管理器
    /// </summary>
    public class CellServiceManager : Singleton<_CellServiceManager> { }
}
