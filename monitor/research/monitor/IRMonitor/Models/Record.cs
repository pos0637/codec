using System;
using System.Runtime.Serialization;

namespace Models
{
    [DataContract]
    public class Record
    {
        /// <summary>
        /// 录像类型
        /// 0:告警
        /// 1:手动
        /// </summary>
        [DataMember(Name = "Type")]
        public Int32 mType;

        /// <summary>
        /// ID
        /// </summary>
        [DataMember(Name = "Id")]
        public Int64 mId;

        /// <summary>
        /// 录像名称
        /// </summary>
        [DataMember(Name = "Name")]
        public String mName;

        /// <summary>
        /// 开始时间
        /// </summary>
        [DataMember(Name = "StartTime")]
        public DateTime mStartTime;

        /// <summary>
        /// 录像时间
        /// </summary>
        [DataMember(Name = "RecordTime")]
        public Double mRecordTime;

        /// <summary>
        /// 缩略图
        /// </summary>
        [DataMember(Name = "ImageData")]
        public String mImageData;

        [NonSerialized]
        public Int64 mStartId;

        [NonSerialized]
        public Int64 mEndId;
    }

    /// <summary>
    /// 录像信息
    /// </summary>
    public class RecoreInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public Int64 mId;

        /// <summary>
        /// video文件名称
        /// </summary>
        public String mVideoFileName;

        /// <summary>
        /// info文件名称
        /// </summary>
        public String mInfoFileName;
    }
}
