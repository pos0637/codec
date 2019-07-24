using System;
using System.Runtime.Serialization;

namespace Models
{
    [DataContract]
    public class SelectionTempCurve
    {
        [DataMember(Name = "SelectionId")]
        public Int64 mSelectionId;

        [DataMember(Name = "DateTime")]
        public DateTime mDateTime;

        [DataMember(Name = "MaxTemp")]
        public Single mMaxTemp;

        [DataMember(Name = "MinTemp")]
        public Single mMinTemp;

        [DataMember(Name = "AvgTemp")]
        public Single mAvgTemp;
    }

    [DataContract]
    public class GroupSelectionTempCurve
    {
        [DataMember(Name = "GroupSelectionId")]
        public Int64 mGroupSelectionId;

        [DataMember(Name = "DateTime")]
        public DateTime mDateTime;

        [DataMember(Name = "MaxTemp")]
        public Single mMaxTemp;

        [DataMember(Name = "TempDif")]
        public Single mTempDif;

        [DataMember(Name = "TempRise")]
        public Single mTempRise;
    }

    [DataContract]
    public class TempCurveSample
    {
        /// <summary>
        /// 普通选区采样频率
        /// </summary>
        [DataMember(Name = "SelectionSample")]
        public Int32 mSelectionSample;

        /// <summary>
        /// 选区组采样频率
        /// </summary>
        [DataMember(Name = "GroupSelectionSample")]
        public Int32 mGroupSelectionSample;
    }
}
