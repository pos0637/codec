using System;
using System.Runtime.Serialization;

namespace IRMonitor.Common
{
    /// <summary>
    /// 实时数据
    /// </summary>
    [DataContract]
    public class RealTimeInfo
    {
        /// <summary>
        /// 同步告警索引
        /// </summary>
        [DataMember(Name = "SyncAlarmId")]
        public Int32 mSyncAlarmId;

        /// <summary>
        /// 同步选区信息索引
        /// </summary>
        [DataMember(Name = "SyncSelectionId")]
        public Int32 mSyncSelectionId;

        /// <summary>
        /// 同步组选区信息索引
        /// </summary>
        [DataMember(Name = "SyncSelectionGroupId")]
        public Int32 mSyncSelectionGroupId;

        /// <summary>
        /// 选区温度数据
        /// </summary>
        [DataMember(Name = "SelectionTempData")]
        public String mSelectionTempData;

        /// <summary>
        /// 组选区温度数据
        /// </summary>
        [DataMember(Name = "GroupSelectionTempData")]
        public String mGroupSelectionTempData;
    }
}
