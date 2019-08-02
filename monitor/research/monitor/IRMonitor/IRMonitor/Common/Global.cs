using Models;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace IRMonitor.Common
{
    /// <summary>
    /// 全局变量类
    /// </summary>
    public static class Global
    {
        /// <summary>
        /// 全局Cell信息列表
        /// </summary>
        public static List<Cell> gCellList = new List<Cell>();

        /// <summary>
        /// 云端IP
        /// </summary>
        public static String gCloudIP = ConfigurationManager.AppSettings["CloudIP"];

        /// <summary>
        /// 云端端口
        /// </summary>
        public static Int32 gCloudPort = Int32.Parse(ConfigurationManager.AppSettings["CloudPort"]);

        /// <summary>
        /// 云端RTMP端口
        /// </summary>
        public static Int32 gCloudRtmpPort = Int32.Parse(ConfigurationManager.AppSettings["CloudRtmpPort"]);

        /// <summary>
        /// 探测端口
        /// </summary>
        public static Int32 gDiscoveryPort = Int32.Parse(ConfigurationManager.AppSettings["DiscoveryPort"]);

        /// <summary>
        /// 上网卡名称
        /// </summary>
        public static String gModemName = ConfigurationManager.AppSettings["ModemName"];

        /// <summary>
        /// 上网卡比特率
        /// </summary>
        public static Int32 gBaudRate = Int32.Parse(ConfigurationManager.AppSettings["BaudRate"]);

        /// <summary>
        /// 是否Modem发送短信
        /// </summary>
        public static Boolean gIsModemSend = Boolean.Parse(ConfigurationManager.AppSettings["IsModemSend"]);
    }
}
