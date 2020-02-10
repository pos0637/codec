using Common;
using Devices;
using Miscs;
using Repository.Entities;
using System;
using static IRMonitor.Miscs.MethodUtils;

namespace IRMonitor2.Services.Cell
{
    /// <summary>
    /// 设备单元服务设备操作
    /// </summary>
    public partial class CellService
    {
        /// <summary>
        /// 加载设备列表
        /// </summary>
        /// <returns>是否成功</returns>
        [Synchronized("devices")]
        private bool LoadDevices()
        {
            foreach (var device in cell.devices) {
                var instance = DeviceFactory.Instance.GetDevice(UUID.GenerateUUID(), device.model, device.serialNumber);
                if (instance == null) {
                    Tracker.LogE($"Load device fail: {device.model}");
                    return false;
                }

                Tracker.LogI($"Load device succeed: {device.model}");

                if (!InitializeDevice(instance, device)) {
                    Tracker.LogE($"Initialize device fail: {device.model}");
                    return false;
                }

                Tracker.LogI($"Initialize device succeed: {device.model}");

                devices.Add(instance);
            }

            return true;
        }

        /// <summary>
        /// 初始化设备
        /// </summary>
        /// <param name="instance">设备</param>
        /// <param name="device">设备配置</param>
        /// <returns>是否成功</returns>
        private bool InitializeDevice(IDevice instance, Configuration.Device device)
        {
            try {
                var category = Enum.Parse(typeof(DeviceType), device.category);
                switch (category) {
                    case DeviceType.Camera:
                        // TODO: 启动服务线程并设置参数
                        break;
                    case DeviceType.IrCamera:
                        // TODO: 启动服务线程并设置参数
                        if (!instance.Write(WriteMode.IrCameraParameters, device.irCameraParameters)) {
                            Tracker.LogE($"Write parameters fail: {device.model}");
                            return false;
                        }
                        break;
                    default:
                        return false;
                }

                return true;
            }
            catch (Exception e) {
                Tracker.LogE($"Initialize device fail: {device.model}", e);
                return false;
            }
        }

        [Synchronized("devices")]
        private void CloseDevices()
        {
            foreach (var worker in workers) {
                worker.Dispose();
            }

            foreach (var device in devices) {
                device.Close();
                device.Dispose();
            }
        }
    }
}
