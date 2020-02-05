using System.Configuration;

namespace IRMonitor2.Common
{
    /// <summary>
    /// 全局变量类
    /// </summary>
    public static class Global
    {
        /// <summary>
        /// 客户端索引
        /// </summary>
        public static string gClientId = ConfigurationManager.AppSettings["ClientId"];

        /// <summary>
        /// 云端IP
        /// </summary>
        public static string gCloudIP = ConfigurationManager.AppSettings["CloudIP"];

        /// <summary>
        /// 云端端口
        /// </summary>
        public static int gCloudPort = int.Parse(ConfigurationManager.AppSettings["CloudPort"]);

        /// <summary>
        /// 云端RTMPIP
        /// </summary>
        public static string gCloudRtmpIP = ConfigurationManager.AppSettings["CloudRtmpIP"];

        /// <summary>
        /// 云端RTMP端口
        /// </summary>
        public static int gCloudRtmpPort = int.Parse(ConfigurationManager.AppSettings["CloudRtmpPort"]);

        /// <summary>
        /// 探测端口
        /// </summary>
        public static int gDiscoveryPort = int.Parse(ConfigurationManager.AppSettings["DiscoveryPort"]);

        /// <summary>
        /// 上网卡名称
        /// </summary>
        public static string gModemName = ConfigurationManager.AppSettings["ModemName"];

        /// <summary>
        /// 上网卡比特率
        /// </summary>
        public static int gBaudRate = int.Parse(ConfigurationManager.AppSettings["BaudRate"]);

        /// <summary>
        /// 是否Modem发送短信
        /// </summary>
        public static bool gIsModemSend = bool.Parse(ConfigurationManager.AppSettings["IsModemSend"]);

        /// <summary>
        /// 录像保存目录
        /// </summary>
        public static string gRecordingsFolder = ConfigurationManager.AppSettings["RecordingsFolder"];
    }
}
