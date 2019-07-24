using System;
using System.Runtime.Serialization;

namespace Models
{
    [DataContract]
    public class Alarm
    {
        [DataMember(Name = "CellId")]
        public Int64 mCellId;

        [DataMember(Name = "Type")]
        public Int32 mType;

        [DataMember(Name = "Id")]
        public Int64 mId;

        [DataMember(Name = "AlarmId")]
        public Int64 mAlarmId;

        [DataMember(Name = "DeviceName")]
        public String mDeviceName;

        [DataMember(Name = "StartTime")]
        public String mStartTime;

        [DataMember(Name = "EndTime")]
        public String mEndTime;

        [DataMember(Name = "Reason")]
        public String mReason;

        [DataMember(Name = "Image")]
        public String mImage;

        [NonSerialized]
        public String mImagePath;
    }

    [DataContract]
    public class AlarmImage
    {
        [DataMember(Name = "AlarmId")]
        public Int64 mAlarmId;

        [DataMember(Name = "Image")]
        public String mImage;
    }

    [DataContract]
    public class UnresolvedAlarm
    {
        [DataMember(Name = "CellId")]
        public Int64 mCellId;

        [DataMember(Name = "Mode")]
        public Int32 mMode;

        [DataMember(Name = "Id")]
        public Int64 mId;

        [DataMember(Name = "Type")]
        public Int32 mType;

        [DataMember(Name = "AlarmLevel")]
        public Int32 mAlarmLevel;
    }
}
