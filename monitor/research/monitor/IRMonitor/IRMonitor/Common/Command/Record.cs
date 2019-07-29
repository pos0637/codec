using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IRMonitor.Common
{
    /// <summary>
    /// 删除记录
    /// </summary>
    [DataContract]
    public class RemoveRecord
    {
        /// <summary>
        /// 记录id列表
        /// </summary>
        [DataMember(Name = "RecordIdList")]
        public List<Int64> mRecordIdList;
    }

    [DataContract]
    public class SearchRecord
    {
        [DataMember(Name = "StartDateTime")]
        public DateTime mStartDateTime;

        [DataMember(Name = "EndDateTime")]
        public DateTime mEndDateTime;

        [DataMember(Name = "Name")]
        public String mName;

        [DataMember(Name = "Offset")]
        public Int32 mOffset;

        [DataMember(Name = "Count")]
        public Int32 mCount;
    }
}
