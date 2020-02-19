using Common;
using Microsoft.EntityFrameworkCore;
using OpenCvSharp;
using Repository.Entities;
using System;
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
        private static Configuration configuration;

        static Repository()
        {
            configuration = LoadConfiguation();
        }

        /// <summary>
        /// 数据仓库
        /// </summary>
        public class RepositoyContext : DbContext
        {
            protected override void OnConfiguring(DbContextOptionsBuilder options)
                => options.UseSqlite("Data Source=ir.db");

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
                modelBuilder.Entity<Alarm>().ToTable("t_alarm");
            }

            /// <summary>
            /// 添加告警
            /// </summary>
            /// <param name="alarm">告警</param>
            public void AddAlarm(Alarm alarm)
            {
                Add(alarm);
                SaveChanges();
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
    }
}
