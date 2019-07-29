using System;
using System.Runtime.Serialization;

namespace IRMonitor.Common
{
    [DataContract]
    public class Replay
    {
        /// <summary>
        /// 回放类型(0:告警 1:手动)
        /// </summary>
        [DataMember(Name = "Type")]
        public Int32 mType;

        [DataMember(Name = "Id")]
        public Int64 mId;
    }

    [DataContract]
    public class ReplayInfo
    {
        [DataMember(Name = "Width")]
        public Int32 mWidth;

        [DataMember(Name = "Height")]
        public Int32 mHeight;

        [DataMember(Name = "RecordTime")]
        public Double mRecordTime;
    }
}
