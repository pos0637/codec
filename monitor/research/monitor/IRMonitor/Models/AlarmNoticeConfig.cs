using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Models
{
    [DataContract]
    public class AlarmNoticeConfig
    {
        /// <summary>
        /// 发送用户
        /// </summary>
        [DataMember(Name = "SendUser")]
        public List<String> mSendUser = new List<String>();

        /// <summary>
        /// 是否告警发送
        /// </summary>
        [DataMember(Name = "IsAlarmSend")]
        public Boolean mIsAlarmSend;

        /// <summary>
        /// 是否整点发送
        /// </summary>
        [DataMember(Name = "IsHourSend")]
        public Boolean mIsHourSend;

        /// <summary>
        /// 是否定时发送
        /// </summary>
        [DataMember(Name = "IsRegularTimeSend")]
        public Boolean mIsRegularTimeSend;

        /// <summary>
        /// 定时时间
        /// </summary>
        [DataMember(Name = "RegularTime")]
        public List<TimeSpan> mRegularTime = new List<TimeSpan>();

        /// <summary>
        /// 是否自动回复
        /// </summary>
        [DataMember(Name = "IsAutoReply")]
        public Boolean mIsAutoReply;

        /// <summary>
        /// 选区告警是否录像
        /// </summary>
        [DataMember(Name = "IsSelectionRecord")]
        public Boolean mIsSelectionRecord;

        /// <summary>
        /// 组选区告警是否录像
        /// </summary>
        [DataMember(Name = "IsGroupSelectionRecord")]
        public Boolean mIsGroupSelectionRecord;
    }
}
