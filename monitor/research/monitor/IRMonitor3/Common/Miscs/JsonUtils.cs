using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Common
{
    /// <summary>
    /// JSON序列化/反序列化辅助处理类
    /// </summary>
    public static class JsonUtils
    {
        /// <summary>
        /// 将对象序列化成JSON字节数组
        /// </summary>
        /// <typeparam name="T">需序列化的对象类型</typeparam>
        /// <param name="data">需序列化的对象</param>
        /// <returns>序列化对象结果</returns>
        public static byte[] Serialize<T>(T data)
        {
            try {
                var serializer = new DataContractJsonSerializer(typeof(T));
                using (var stream = new MemoryStream()) {
                    serializer.WriteObject(stream, data);
                    return stream.ToArray();
                }
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return null;
            }
        }

        /// <summary>
        /// JSON字节数组反序列化为对象
        /// </summary>
        /// <typeparam name="T">反序列化后的对象类型</typeparam>
        /// <param name="buffer">需反序列化的Json字节数组</param>
        /// <returns>反序列化后的对象</returns>
        public static T Deserialize<T>(byte[] buffer)
        {
            try {
                var serializer = new DataContractJsonSerializer(typeof(T));
                using (var stream = new MemoryStream(buffer)) {
                    return (T)serializer.ReadObject(stream);
                }
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return default;
            }
        }

        /// <summary>
        /// 将对象序列化成Json，并解码为一个字符串
        /// </summary>
        /// <typeparam name="T">需序列化的对象类型</typeparam>
        /// <param name="data">需序列化的对象</param>
        /// <returns>Json字符串</returns>
        public static string ObjectToJson<T>(T data)
        {
            var buffer = Serialize(data);
            if (buffer != null) {
                try {
                    return Encoding.UTF8.GetString(buffer);
                }
                catch (Exception e) {
                    Tracker.LogE(e);
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Json字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T">需反序列化的对象类型</typeparam>
        /// <param name="json">需反序列化的Json数据</param>
        /// <returns>反序列化后的对象</returns>
        public static T ObjectFromJson<T>(string json)
        {
            try {
                var buffer = Encoding.UTF8.GetBytes(json);
                return Deserialize<T>(buffer);
            }
            catch (Exception e) {
                Tracker.LogE(e);
            }

            return default;
        }
    }
}