using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Common
{
    /// <summary>
    /// 日志类型
    /// </summary>
    public static class Tracker
    {
        static Tracker()
        {
            // 打开日志配置文件
            string path = AppDomain.CurrentDomain.BaseDirectory + @"\log4net.config";
            ILoggerRepository repository = LogManager.CreateRepository("NETCoreRepository");
            XmlConfigurator.Configure(repository, new FileInfo(path));
            sLog = LogManager.GetLogger(repository.Name, "runtime");
            sLogDA = LogManager.GetLogger(repository.Name, "database");
            sLogEX = LogManager.GetLogger(repository.Name, "exception");
            sLogNW = LogManager.GetLogger(repository.Name, "network");
            sLogBU = LogManager.GetLogger(repository.Name, "business");
        }

        private static ILog sLog;
        private static readonly ILog sLogDA;
        private static readonly ILog sLogEX;
        private static readonly ILog sLogNW;
        private static readonly ILog sLogBU;

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
        public static void LogI(string message, [CallerFilePath] string path = "", [CallerLineNumber]int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            sLog.Info($"[{Path.GetFileNameWithoutExtension(path)}:{lineNumber} {memberName}] {message}\n");
        }

        /// <summary>
        /// 输出调试日志
        /// </summary>
        /// <param name="message">调试日志</param>
        public static void LogD(string message, [CallerFilePath] string path = "", [CallerLineNumber]int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            sLog.Debug($"[{Path.GetFileNameWithoutExtension(path)}:{lineNumber} {memberName}] {message}\n");
        }

        /// <summary>
        /// 输出错误日志
        /// </summary>
        /// <param name="message">错误日志</param>
        public static void LogE(string message, [CallerFilePath] string path = "", [CallerLineNumber]int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            sLogEX.Error($"[{Path.GetFileNameWithoutExtension(path)}:{lineNumber} {memberName}] {message}\n");
        }

        /// <summary>
        /// 输出错误日志
        /// </summary>
        /// <param name="e">异常</param>
        public static void LogE(Exception e, [CallerFilePath] string path = "", [CallerLineNumber]int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            sLogEX.Error($"[{Path.GetFileNameWithoutExtension(path)}:{lineNumber} {memberName}] {e.ToString()}\n {e.StackTrace.ToString()}\n");
        }

        /// <summary>
        /// 输出错误日志
        /// </summary>
        /// <param name="message">错误日志</param>
        /// <param name="e">异常</param>
        public static void LogE(string message, Exception e, [CallerFilePath] string path = "", [CallerLineNumber]int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            sLogEX.Error($"[{Path.GetFileNameWithoutExtension(path)}:{lineNumber} {memberName}] {message}: {e.ToString()}\n {e.StackTrace.ToString()}\n");
        }

        /// <summary>
        /// 输出数据库访问日志
        /// </summary>
        /// <param name="message">数据库访问日志</param>
        public static void LogDA(string message, [CallerFilePath] string path = "", [CallerLineNumber]int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            sLogDA.Info($"[{Path.GetFileNameWithoutExtension(path)}:{lineNumber} {memberName}] {message}\n");
        }

        /// <summary>
        /// 输出网络日志
        /// </summary>
        /// <param name="message">网络日志</param>
        public static void LogNW(string message, [CallerFilePath] string path = "", [CallerLineNumber]int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            sLogNW.Info($"[{Path.GetFileNameWithoutExtension(path)}:{lineNumber} {memberName}] {message}\n");
        }

        /// <summary>
        /// 输出网络日志
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="message">网络日志</param>
        public static void LogNW(string tag, string message)
        {
            sLogNW.Info("[" + tag + "] " + message);
        }

        /// <summary>
        /// 输出业务日志
        /// </summary>
        /// <param name="message">业务日志</param>
        public static void LogBU(string message, [CallerFilePath] string path = "", [CallerLineNumber]int lineNumber = 0, [CallerMemberName] string memberName = "")
        {
            sLogBU.Info($"[{Path.GetFileNameWithoutExtension(path)}:{lineNumber} {memberName}] {message}\n");
        }

        /// <summary>
        /// 输出业务日志
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="message">业务日志</param>
        public static void LogBU(string tag, string message)
        {
            sLogBU.Info("[" + tag + "] " + message);
        }
    }
}
