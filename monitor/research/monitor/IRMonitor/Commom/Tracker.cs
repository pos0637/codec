using log4net;
using System;
using System.IO;

namespace Common
{
    /// <summary>
    /// 日志类型
    /// </summary>
    public class Tracker
    {
        static Tracker()
        {
            // 打开日志配置文件
            String path = AppDomain.CurrentDomain.BaseDirectory + @"\log4net.config";
            log4net.Config.XmlConfigurator.Configure(new FileInfo(path));
            sLog = LogManager.GetLogger("runtime");
            sLogDA = LogManager.GetLogger("database");
            sLogEX = LogManager.GetLogger("exception");
            sLogNW = LogManager.GetLogger("network");
            sLogBU = LogManager.GetLogger("business");
        }

        private static ILog sLog;
        private static ILog sLogDA;
        private static ILog sLogEX;
        private static ILog sLogNW;
        private static ILog sLogBU;

        /// <summary>
        /// 设置日志接口
        /// </summary>
        /// <param name="log">日志接口</param>
        public static void SetLogger(ILog log)
        {
            sLog = log;
        }

        /// <summary>
        /// 输出信息日志
        /// </summary>
        /// <param name="message">信息日志</param>
        public static void LogI(String message)
        {
            sLog.Info(message + "\n");
        }

        /// <summary>
        /// 输出调试日志
        /// </summary>
        /// <param name="message">调试日志</param>
        public static void LogD(String message)
        {
            sLog.Debug(message + "\n");
        }

        /// <summary>
        /// 输出错误日志
        /// </summary>
        /// <param name="message">错误日志</param>
        public static void LogE(String message)
        {
            sLogEX.Error(message + "\n");
        }

        /// <summary>
        /// 输出错误日志
        /// </summary>
        /// <param name="e">异常</param>
        public static void LogE(Exception e)
        {
            sLogEX.Error(e.ToString() + "\n" + e.StackTrace.ToString() + "\n");
        }

        /// <summary>
        /// 输出错误日志
        /// </summary>
        /// <param name="message">错误日志</param>
        /// <param name="e">异常</param>
        public static void LogE(String message, Exception e)
        {
            sLogEX.Error(message + ":" + e.ToString() + "\n" + e.StackTrace.ToString() + "\n");
        }

        /// <summary>
        /// 输出数据库访问日志
        /// </summary>
        /// <param name="message">数据库访问日志</param>
        public static void LogDA(String message)
        {
            sLogDA.Info(message);
        }

        /// <summary>
        /// 输出网络日志
        /// </summary>
        /// <param name="message">网络日志</param>
        public static void LogNW(String message)
        {
            sLogNW.Info(message);
        }

        /// <summary>
        /// 输出网络日志
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="message">网络日志</param>
        public static void LogNW(String tag, String message)
        {
            sLogNW.Info("[" + tag + "] " + message);
        }

        /// <summary>
        /// 输出业务日志
        /// </summary>
        /// <param name="message">业务日志</param>
        public static void LogBU(String message)
        {
            sLogBU.Info(message);
        }

        /// <summary>
        /// 输出业务日志
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="message">业务日志</param>
        public static void LogBU(String tag, String message)
        {
            sLogBU.Info("[" + tag + "] " + message);
        }
    }
}