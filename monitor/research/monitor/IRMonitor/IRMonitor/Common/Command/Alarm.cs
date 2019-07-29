using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IRMonitor.Common
{
    [DataContract]
    public class RemoveAlarm
    {
        [DataMember(Name = "AlarmIdList")]
        public List<Int64> mAlarmIdList;
    }

    [DataContract]
    public class SearchAlarm
    {
        [DataMember(Name = "Mode")]
        public Int32 mMode;

        [DataMember(Name = "Type")]
        public Int32 mType;

        [DataMember(Name = "Reason")]
        public Int32 mReason;

        [DataMember(Name = "StartTime")]
        public DateTime mStartTime;

        [DataMember(Name = "EndTime")]
        public DateTime mEndTime;
    }
}
