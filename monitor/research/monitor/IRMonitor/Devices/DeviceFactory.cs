using Common;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

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
        private readonly String mDevicePath = Application.StartupPath;

        /// <summary>
        /// 设备类型哈希表
        /// </summary>
        private Hashtable mDeviceTypeList = new Hashtable();

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
        public IDevice GetDevice(Int64 id, String typeName, String name)
        {
            Type deviceType;
            lock (mDeviceTypeList) {
                if (!mDeviceTypeList.Contains(typeName))
                    return null;

                deviceType = mDeviceTypeList[typeName] as Type;
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
            catch (Exception ex) {
                Tracker.LogE(ex);
                return null;
            }
        }

        /// <summary>
        /// 获取当前程序集下IDevice派生的设备类
        /// </summary>
        public void GetAssemblyDeviceType()
        {
            lock (mDeviceTypeList) {
                if (mDeviceTypeList.Count > 0)
                    return;

                Tracker.LogI(String.Format("Get All DeviceType From Path:({0})", mDevicePath));

                DirectoryInfo folder = new DirectoryInfo(mDevicePath);
                if (!folder.Exists)
                    return;

                foreach (FileInfo file in folder.GetFiles("*Device.dll")) {
                    try {
                        Assembly asm = Assembly.LoadFile(file.FullName);
                        foreach (Type type in asm.GetTypes()) {
                            if (type.IsSubclassOf(typeof(IDevice))) {
                                mDeviceTypeList.Add(type.Name, type);
                                continue;
                            }
                        }
                    }
                    catch (Exception ex) {
                        Tracker.LogE(ex);
                        continue;
                    }
                }
            }
        }
    }
}
