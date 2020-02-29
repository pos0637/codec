using Common;
using System;
using System.Runtime.InteropServices;

namespace Miscs
{
    /// <summary>
    /// 性能工具
    /// </summary>
    public static class PerformanceUtils
    {
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        /// <summary>
        /// 获取高精度性能计数
        /// </summary>
        /// <returns>高精度性能计数</returns>
        public static long GetTickCount()
        {
            return !QueryPerformanceCounter(out long value) ? Environment.TickCount : value;
        }

        /// <summary>
        /// 计算两个高精度性能计数差值(ms)
        /// </summary>
        /// <param name="startTime">开始计数</param>
        /// <param name="stopTime">终止计数</param>
        /// <returns>两个高精度性能计数差值(ms)</returns>
        public static long CalcElapsed(long startTime, long stopTime)
        {
            return (!QueryPerformanceFrequency(out long freq) || (freq == 0)) ? stopTime - startTime : (stopTime - startTime) * 1000 / freq;
        }

        /// <summary>
        /// 计算两个高精度性能计数差值(精度us)
        /// </summary>
        /// <param name="startTime">开始计数</param>
        /// <param name="stopTime">终止计数</param>
        /// <returns>两个高精度性能计数差值(us)</returns>
        public static long CalcElapsed2(long startTime, long stopTime)
        {
            return (!QueryPerformanceFrequency(out long freq) || (freq == 0)) ? (stopTime - startTime) * 1000 : (stopTime - startTime) * 1000 * 1000 / freq;
        }

        /// <summary>
        /// 性能计数器
        /// </summary>
        public sealed class PerformanceCounter : IDisposable
        {
            /// <summary>
            /// 开始时间
            /// </summary>
            private long start = GetTickCount();

            /// <summary>
            /// 标签
            /// </summary>
            private readonly string tag;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="tag">标签</param>
            public PerformanceCounter(string tag)
            {
                this.tag = tag;
            }

            public void Dispose()
            {
                Tracker.LogD($"{tag}{CalcElapsed2(start, GetTickCount())}");
            }
        }
    }
}
