using Common;
using OpenCvSharp;
using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.SQLite;
using System.Data.SQLite.EF6;
using System.IO;
using System.Linq;
using System.Text;
using YamlDotNet.Serialization;

namespace Repository
{
    /// <summary>
    /// 数据仓库
    /// </summary>
    public static class Repository
    {
        /// <summary>
        /// 应用配置文件路径
        /// </summary>
        private static readonly string AppConfigurationPath = AppDomain.CurrentDomain.BaseDirectory + @"\app.yml";

        /// <summary>
        /// 选区配置文件路径
        /// </summary>
        private static readonly string SelectionsConfigurationPath = AppDomain.CurrentDomain.BaseDirectory + @"\selections.json";

        /// <summary>
        /// 配置
        /// </summary>
        private static readonly Configuration configuration;

        static Repository()
        {
            configuration = LoadConfiguation();
        }

        /// <summary>
        /// 数据库配置
        /// </summary>
        private class SQLiteConfiguration : DbConfiguration
        {
            public SQLiteConfiguration()
            {
                SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);
                SetProviderFactory("System.Data.SQLite.EF6", SQLiteProviderFactory.Instance);
                SetProviderServices("System.Data.SQLite", (DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices)));
            }
        }

        /// <summary>
        /// 数据仓库
        /// </summary>
        public class RepositoyContext : DbContext
        {
            /// <summary>
            /// 告警信息列表
            /// </summary>
            public DbSet<Alarm> Alarm { get; set; }

            /// <summary>
            /// 获取数据库连接
            /// </summary>
            /// <returns>数据库连接</returns>
            private static DbConnection GetConnection()
            {
                var connection = SQLiteProviderFactory.Instance.CreateConnection();
                connection.ConnectionString = $"Data Source={AppDomain.CurrentDomain.BaseDirectory}ir.s3db";
                return connection;
            }

            public RepositoyContext()
                : base(GetConnection(), true)
            {
                DbConfiguration.SetConfiguration(new SQLiteConfiguration());
            }
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <returns>配置</returns>
        public static Configuration LoadConfiguation()
        {
            try {
                using (var sr = new StreamReader(AppConfigurationPath, Encoding.UTF8)) {
                    return new Deserializer().Deserialize<Configuration>(sr);
                }
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return null;
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="configuration">配置</param>
        public static void SaveConfiguation(Configuration configuration)
        {
            try {
                using (var sw = new StreamWriter(AppConfigurationPath, false, Encoding.UTF8)) {
                    var yaml = new Serializer().Serialize(configuration);
                    sw.Write(yaml);
                    sw.Flush();
                }
            }
            catch (Exception e) {
                Tracker.LogE(e);
                throw e;
            }
        }

        /// <summary>
        /// 读取选区配置
        /// </summary>
        /// <returns>配置</returns>
        public static Selections LoadSelections()
        {
            try {
                using (var sr = new StreamReader(SelectionsConfigurationPath, Encoding.UTF8)) {
                    return JsonUtils.ObjectFromJson<Selections>(sr.ReadToEnd());
                }
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return null;
            }
        }

        /// <summary>
        /// 保存选区配置
        /// </summary>
        /// <param name="selections">选区配置</param>
        public static void SaveSelections(Selections selections)
        {
            try {
                using (var sw = new StreamWriter(SelectionsConfigurationPath, false, Encoding.UTF8)) {
                    var data = JsonUtils.ObjectToJson(selections);
                    sw.Write(data);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception e) {
                Tracker.LogE(e);
                throw e;
            }
        }

        /// <summary>
        /// 读取告警温度矩阵
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>温度矩阵</returns>
        public static float[] LoadAlarmTemperature(string filename)
        {
            try {
                var content = File.ReadAllText(filename);
                var data = Convert.FromBase64String(content);
                return data.Select(value => Convert.ToSingle(value)).ToArray();
            }
            catch {
                return null;
            }
        }

        /// <summary>
        /// 保存告警温度矩阵
        /// </summary>
        /// <param name="temperature">温度矩阵</param>
        /// <returns>文件名</returns>
        public static string SaveAlarmTemperature(Buffer<float> temperature)
        {
            return SaveAlarmTemperature(temperature?.buffer);
        }

        /// <summary>
        /// 保存告警温度矩阵
        /// </summary>
        /// <param name="temperature">温度矩阵</param>
        /// <returns>文件名</returns>
        public static string SaveAlarmTemperature(float[] temperature)
        {
            if (temperature == null) {
                return null;
            }

            try {
                var filename = $"{configuration.information.saveImagePath}/{Guid.NewGuid().ToString("N")}.temp";
                byte[] data = temperature.Select(value => Convert.ToByte(value)).ToArray();
                File.WriteAllText(filename, Convert.ToBase64String(data));
                return filename;
            }
            catch {
                return null;
            }
        }

        /// <summary>
        /// 保存告警图像
        /// </summary>
        /// <param name="image">图像</param>
        /// <returns>是否成功</returns>
        public static string SaveAlarmYV12Image(Buffer<byte> image)
        {
            if (image == null) {
                return null;
            }

            return SaveAlarmYV12Image(image.width, image.height, image.buffer);
        }

        /// <summary>
        /// 保存告警图像
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="image">图像</param>
        /// <returns>是否成功</returns>
        public static string SaveAlarmYV12Image(int width, int height, byte[] image)
        {
            if (image == null) {
                return null;
            }

            try {
                var filename = $"{configuration.information.saveImagePath}/{Guid.NewGuid().ToString("N")}.png";
                var mat = new Mat(height + height / 2, width, MatType.CV_8UC1, image);
                return mat.SaveImage(filename) ? filename : null;
            }
            catch {
                return null;
            }
        }

        /// <summary>
        /// 保存告警图像
        /// </summary>
        /// <param name="image">图像</param>
        /// <returns>是否成功</returns>
        public static string SaveAlarmRGBAImage(Buffer<byte> image)
        {
            if (image == null) {
                return null;
            }

            return SaveAlarmRGBAImage(image.width, image.height, image.buffer);
        }

        /// <summary>
        /// 保存告警图像
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="image">图像</param>
        /// <returns>是否成功</returns>
        public static string SaveAlarmRGBAImage(int width, int height, byte[] image)
        {
            if (image == null) {
                return null;
            }

            try {
                var filename = $"{configuration.information.saveImagePath}/{Guid.NewGuid().ToString("N")}.png";
                var mat = new Mat(height, width, MatType.CV_8UC4, image);
                return mat.SaveImage(filename) ? filename : null;
            }
            catch {
                return null;
            }
        }

        /// <summary>
        /// 添加告警
        /// </summary>
        /// <param name="alarm">告警</param>
        /// <returns>是否成功</returns>
        public static bool AddAlarm(Alarm alarm)
        {
            try {
                using (var db = new RepositoyContext()) {
                    db.Set<Alarm>().Add(alarm);
                    db.SaveChanges();
                }

                return true;
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return false;
            }
        }

        /// <summary>
        /// 获取告警数量
        /// </summary>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        public static int GetAlarmsCount(DateTime start, DateTime end)
        {
            try {
                using (var db = new RepositoyContext()) {
                    if ((start != null) && (end != null)) {
                        return db.Set<Alarm>()
                            .Where(a => a.startTime >= start)
                            .Where(a => a.startTime <= end)
                            .Count();
                    }
                    else {
                        return db.Set<Alarm>().Count();
                    }
                }
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return 0;
            }
        }

        /// <summary>
        /// 获取最新告警
        /// </summary>
        /// <param name="count">数量</param>
        /// <returns>告警列表</returns>
        public static List<Alarm> GetLastAlarms(int count)
        {
            try {
                using (var db = new RepositoyContext()) {
                    return db.Set<Alarm>().OrderByDescending(alarm => alarm.startTime).Take(count).ToList();
                }
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return null;
            }
        }

        /// <summary>
        /// 获取告警
        /// </summary>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="page">页码</param>
        /// <param name="count">数量</param>
        /// <returns>告警列表</returns>
        public static List<Alarm> GetAlarms(DateTime start, DateTime end, int page, int count)
        {
            try {
                using (var db = new RepositoyContext()) {
                    if ((start == null) || (end == null)) {
                        return db.Set<Alarm>()
                            .Where(a => a.startTime >= start)
                            .Where(a => a.startTime <= end)
                            .OrderByDescending(alarm => alarm.startTime)
                            .Skip((page - 1) * count).Take(count)
                            .ToList();
                    }
                    else {
                        return db.Set<Alarm>()
                            .OrderByDescending(alarm => alarm.startTime)
                            .Skip((page - 1) * count).Take(count)
                            .ToList();
                    }
                }
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return null;
            }
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids">索引列表</param>
        /// <returns>是否成功</returns>
        public static bool DeleteAlarms(List<int> ids)
        {
            try {
                using (var db = new RepositoyContext()) {
                    var alarms = db.Set<Alarm>().Where(a => ids.Exists(id => id == a.id)).ToList();
                    db.Set<Alarm>().BulkDelete(alarms);
                    return true;
                }
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return false;
            }
        }
    }
}
