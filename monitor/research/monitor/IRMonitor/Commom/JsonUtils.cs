﻿using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Common
{
    /// <summary>
    /// Json序列化/反序列化辅助处理类
    /// </summary>
    public class JsonUtils
    {
        /// <summary>
        /// 将对象序列化成Json字节数组
        /// </summary>
        /// <typeparam name="T">需序列化的对象类型</typeparam>
        /// <param name="data">需序列化的对象</param>
        /// <returns>序列化对象结果</returns>
        public static Byte[] Serializer<T>(T data)
        {
            try {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                using(MemoryStream stream = new MemoryStream()) {
                    serializer.WriteObject(stream , data);
                    return stream.ToArray();
                }
            }
            catch (Exception ex) {
                Tracker.LogE(ex);
                return null;
            }
        }

        /// <summary>
        /// Json字节数组反序列化为对象
        /// </summary>
        /// <typeparam name="T">反序列化后的对象类型</typeparam>
        /// <param name="buffer">需反序列化的Json字节数组</param>
        /// <returns>反序列化后的对象</returns>
        public static T Deserializer<T>(Byte[] buffer)
        {
            try {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                using(MemoryStream stream = new MemoryStream(buffer)) {
                    return (T)serializer.ReadObject(stream);
                }
            }
            catch (Exception ex) {
                Tracker.LogE(ex);
                return default(T);
            }
        }

        /// <summary>
        /// 将对象序列化成Json，并解码为一个字符串
        /// </summary>
        /// <typeparam name="T">需序列化的对象类型</typeparam>
        /// <param name="data">需序列化的对象</param>
        /// <returns>Json字符串</returns>
        public static String ObjectToJson<T>(T data)
        {
            Byte[] buffer = Serializer(data);
            if(buffer != null) {
                try {
                    return Encoding.UTF8.GetString(buffer);
                }
                catch (Exception ex) {
                    Tracker.LogE(ex);
                }
            }

            return String.Empty;
        }

        /// <summary>
        /// Json字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T">需反序列化的对象类型</typeparam>
        /// <param name="json">需反序列化的Json数据</param>
        /// <returns>反序列化后的对象</returns>
        public static T ObjectFromJson<T>(String json)
        {
            try {
                Byte[] buffer = Encoding.UTF8.GetBytes(json);
                return Deserializer<T>(buffer);
            }
            catch (Exception ex) {
                Tracker.LogE(ex);
            }

            return default(T);
        }
    }
}