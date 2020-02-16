using Common;
using System;
using System.Collections.Generic;

namespace Miscs
{
    /// <summary>
    /// 数组工具类
    /// </summary>
    public static class Arrays
    {
        /// <summary>
        /// 获取子数组
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="data">原数组</param>
        /// <param name="index">子数组开始索引</param>
        /// <returns>子数组</returns>
        public static T[] SubArray<T>(this T[] data, int index)
        {
            if (index == 0) {
                return data;
            }

            T[] result = new T[data.Length - index];
            Array.Copy(data, index, result, 0, data.Length - index);

            return result;
        }

        /// <summary>
        /// 获取子数组
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="data">原数组</param>
        /// <param name="index">子数组开始索引</param>
        /// <param name="length">子数组长度</param>
        /// <returns>子数组</returns>
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            if ((index == 0) && ((length < 0) || (length == data.Length))) {
                return data;
            }

            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);

            return result;
        }

        /// <summary>
        /// 克隆数组
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="from">原数组</param>
        /// <param name="to">目标数组</param>
        /// <returns>数组</returns>
        public static T[] Clone<T>(T[] from, T[] to)
        {
            var result = to;
            if ((to == null) || (to.Length != from.Length)) {
                result = new T[from.Length];
            }

            lock (result) {
                Buffer.BlockCopy(from, 0, result, 0, from.Length);
            }

            return result;
        }

        /// <summary>
        /// 克隆数组
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="from">原数组</param>
        /// <param name="to">目标数组</param>
        /// <returns>数组</returns>
        public static PinnedBuffer<T> Clone<T>(T[] from, PinnedBuffer<T> to)
        {
            var result = to;
            if ((to == null) || (to.Length != from.Length)) {
                result = PinnedBuffer<T>.Alloc(from.Length);
            }

            lock (result) {
                Buffer.BlockCopy(from, 0, result.buffer, 0, from.Length);
            }

            return result;
        }

        /// <summary>
        /// 克隆列表
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="from">原列表</param>
        /// <returns>列表</returns>
        public static List<T> Clone<T>(this List<T> from)
        {
            lock (from) {
                return new List<T>(from);
            }
        }
    }
}
