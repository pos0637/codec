﻿using Common;
using Repository.Entities;
using System;
using System.Collections.Generic;
using System.IO;

namespace IRService.Services.Cell
{
    /// <summary>
    /// 设备单元服务管理器
    /// </summary>
    public class _CellServiceManager : ServiceManager
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns>是否成功</returns>
        public bool Initialize()
        {
            // 读取所有设备单元信息
            Configuration configuration = Repository.Repository.LoadConfiguation();
            if (configuration == null) {
                Tracker.LogI("LoadConfiguration fail");
                return false;
            }

            if (!CheckConfiguration(configuration)) {
                Tracker.LogI("CheckConfiguration fail");
                return false;
            }

            Tracker.LogI($"LoadConfiguration succeed, cell count: {configuration.cells.Length}");

            // 创建服务
            foreach (var cell in configuration.cells) {
                var service = new CellService();

                // 初始化设备单元服务
                if (!service.Initialize(new Dictionary<string, object>() { ["cell"] = cell })) {
                    Tracker.LogE($"CellService: {cell.name} initialize fail");
                    return false;
                }

                // 开启设备单元服务
                service.Start();

                AddService(cell.name, service);
                Tracker.LogI($"CellService: {cell.name} start succeed");
            }

            return true;
        }

        /// <summary>
        /// 检测配置
        /// </summary>
        /// <param name="configuration">配置</param>
        /// <returns>是否成功</returns>
        private bool CheckConfiguration(Configuration configuration)
        {
            try {
                if (!Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}/{configuration.information.saveVideoPath}")) {
                    Directory.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}/{configuration.information.saveVideoPath}");
                }

                if (!Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}/{configuration.information.saveImagePath}")) {
                    Directory.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}/{configuration.information.saveImagePath}");
                }
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// 设备单元服务管理器
    /// </summary>
    public class CellServiceManager : Singleton<_CellServiceManager> { }
}
