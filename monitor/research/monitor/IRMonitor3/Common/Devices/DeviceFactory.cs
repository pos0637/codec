using Common;
using System;
using System.Collections;
using System.IO;
using System.Reflection;

namespace Devices
{
    /// <summary>
    /// 设备管理器
    /// </summary>
    public sealed class DeviceFactory : Singleton<DeviceFactory>
    {
        /// <summary>
        /// 动态链接库路径
        /// </summary>
        private readonly string driverPath = Directory.GetCurrentDirectory();

        /// <summary>
        /// 设备类型哈希表
        /// </summary>
        private readonly Hashtable deviceTypeList = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// 构造函数 
        /// </summary>
        public DeviceFactory()
        {
            GetAssemblyDeviceType();
        }

        /// <summary>
        /// 获取设备
        /// </summary>
        /// <param name="id">设备索引</param>
        /// <param name="typeName">设备类名称</param>
        /// <param name="name">设备名称</param>
        /// <returns>设备对象</returns>
        public IDevice GetDevice(long id, string typeName, string name)
        {
            Type deviceType;
            lock (deviceTypeList) {
                if (!deviceTypeList.Contains(typeName)) {
                    return null;
                }

                deviceType = deviceTypeList[typeName] as Type;
            }

            try {
                IDevice device = (Activator.CreateInstance(deviceType)) as IDevice;
                device.Id = id;
                device.DeviceName = name;

                if (!device.Initialize()) {
                    device.Dispose();
                    return null;
                }

                return device;
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return null;
            }
        }

        /// <summary>
        /// 获取当前程序集下IDevice派生的设备类
        /// </summary>
        public void GetAssemblyDeviceType()
        {
            lock (deviceTypeList) {
                if (deviceTypeList.Count > 0) {
                    return;
                }

                Tracker.LogI($"Get all driver from path: {driverPath}");

                DirectoryInfo folder = new DirectoryInfo(driverPath);
                if (!folder.Exists) {
                    return;
                }

                foreach (FileInfo file in folder.GetFiles("*Device.dll")) {
                    try {
                        Assembly asm = Assembly.LoadFile(file.FullName);
                        foreach (Type type in asm.GetTypes()) {
                            if (type.IsSubclassOf(typeof(IDevice))) {
                                deviceTypeList.Add(type.Name, type);
                                continue;
                            }
                        }
                    }
                    catch (Exception e) {
                        Tracker.LogE(e);
                        continue;
                    }
                }
            }
        }
    }
}
