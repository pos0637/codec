using System;

namespace Repository.Entities
{
    /// <summary>
    /// 录像
    /// </summary>
    public class Recording
    {
        /// <summary>
        /// 录像类型
        /// </summary>
        public enum RecordingType
        {
            /// <summary>
            /// 本地录像
            /// </summary>
            Local = 0,

            /// <summary>
            /// 告警录像
            /// </summary>
            Alarm
        }

        /// <summary>
        /// 索引
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 设备单元名称
        /// </summary>
        public string cellName { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string deviceName { get; set; }

        /// <summary>
        /// 通道名称
        /// </summary>
        public string channelName { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? startTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? endTime { get; set; }

        /// <summary>
        /// 录像类型
        /// </summary>
        public RecordingType type { get; set; }

        /// <summary>
        /// 录像资源地址
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 快照资源地址
        /// </summary>
        public string snapshotUrl { get; set; }
    }
}
