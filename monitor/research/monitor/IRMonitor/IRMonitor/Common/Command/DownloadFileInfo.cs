using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace IRMonitor.Common
{
    /// <summary>
    /// 下载文件请求相关文件信息
    /// </summary>
    [DataContract]
    public class DownloadFile
    {
        [DataMember(Name = "Type")]
        public Int32 mType;

        [DataMember(Name = "Id")]
        public Int64 mId;
    }

    /// <summary>
    /// 下载文件大小
    /// </summary>
    [DataContract]
    public class DownloadFileInfo
    {
        [DataMember(Name = "FileName")]
        public String mFileName;

        [DataMember(Name = "FileLength")]
        public Int64 mFileLength;
    }

    /// <summary>
    /// 下载文件请求
    /// </summary>
    [DataContract]
    public class DownloadRequestInfo
    {
        [DataMember(Name = "FileName")]
        public String mFileName;

        [DataMember(Name = "DownloadStartIndex")]
        public Int64 mDownloadStartIndex;

        [DataMember(Name = "DownloadLength")]
        public Int64 mDownloadLength;
    }

    /// <summary>
    /// 下载文件回复
    /// </summary>
    [DataContract]
    public class DownloadResponseInfo
    {
        [DataMember(Name = "FileName")]
        public String mFileName;

        [DataMember(Name = "DownloadStartIndex")]
        public Int64 mDownloadStartIndex;

        [DataMember(Name = "DownloadData")]
        public String mDownloadData;
    }
}
