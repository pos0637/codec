using System;

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
    }
}
