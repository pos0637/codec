using System;
using System.Runtime.Serialization;

namespace IRMonitor.Common
{
    [DataContract]
    public class SearchCurve
    {
        [DataMember(Name = "Id")]
        public Int64 mId;

        [DataMember(Name = "PointCount")]
        public Int32 mPointCount;

        [DataMember(Name = "StartDateTime")]
        public DateTime mStartDateTime;

        [DataMember(Name = "EndDateTime")]
        public DateTime mEndDateTime;
    }
    
    [DataContract]
    public class TempCurve
    {
        [DataMember(Name = "Type")]
        public Int32 mType;

        [DataMember(Name = "Id")]
        public Int64 mId;

        [DataMember(Name = "DeviceSerialNumber")]
        public String mDeviceSerialNumber;

        [DataMember(Name = "DateTime")]
        public DateTime mDateTime;

        [DataMember(Name = "MaxTemp")]
        public Single mMaxTemp;

        [DataMember(Name = "MinTemp")]
        public Single mMinTemp;

        [DataMember(Name = "AvgTemp")]
        public Single mAvgTemp;
    }
}
