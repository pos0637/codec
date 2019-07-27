using Common;
using System;
using System.Collections.Generic;

namespace Devices
{
    /// <summary>
    /// 定义设备驱动抽象类
    /// </summary>
    public abstract class IDriver
    {
        /// <summary>
        /// 加载驱动事件
        /// </summary>
        /// <param name="arguments">参数</param>
        /// <returns>是否成功,成功返回S_OK</returns>
        public abstract ARESULT OnLoad(String arguments);

        /// <summary>
        /// 卸载驱动事件
        /// </summary>
        /// <returns>是否成功,成功返回S_OK</returns>
        public abstract ARESULT OnUnload();

        /// <summary>
        /// 驱动是否匹配指定设备的URI
        /// </summary>
        /// <param name="uri">指定设备的URI</param>
        /// <returns>是否匹配</returns>
        public abstract Boolean Match(Common.Uri uri);

        /// <summary>
        /// 探测系统中匹配的设备
        /// </summary>
        /// <returns></returns>
        public abstract List<IDevice> Probe();

        /// <summary>
        /// 附着设备
        /// </summary>
        /// <param name="uri">指定设备的URI</param>
        /// <param name="configurations">指定设备的配置</param>
        /// <returns>设备对象</returns>
        public abstract IDevice AttachDevice(
            Common.Uri uri, String configurations);

        /// <summary>
        /// 去附着设备
        /// </summary>
        /// <param name="device">设备对象</param>
        /// <returns>是否成功</returns>
        public abstract Boolean DetachDevice(IDevice device);
    }
}
