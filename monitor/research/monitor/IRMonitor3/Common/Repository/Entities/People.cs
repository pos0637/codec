using System;

namespace Repository.Entities
{
    /// <summary>
    /// 人员记录
    /// </summary>
    public class People
    {
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
        /// 选区名称
        /// </summary>
        public string selectionName { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? startTime { get; set; }

        /// <summary>
        /// 快照地址
        /// </summary>
        public string url { get; set; }
    }
}
